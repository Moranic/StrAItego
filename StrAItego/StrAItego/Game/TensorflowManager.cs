using NumSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tensorflow;
using static Tensorflow.Binding;
using Tensorflow.Graphs;
using NeuralNetworkModels.NeuralNetworks;
using System.Diagnostics;
using System.Windows.Forms;

namespace StrAItego.Game
{
    public static class TensorflowManager
    {
        static ConcurrentQueue<PredictionRequest> requestQueue = new ConcurrentQueue<PredictionRequest>();
        static SemaphoreSlim QueueNotification = new SemaphoreSlim(0);
        static Thread[] QueueHandlers;
        static int threadcount = 3;
        static int totalCalls = 0;

        static TensorflowManager() {
            //tf.enable_eager_execution();
            QueueHandlers = new Thread[threadcount];
            for (int i = 0; i < threadcount; i++) {
                int k = i;
                Thread QueueHandler = new Thread(() => QueueHandling(k));
                QueueHandler.IsBackground = true;
                QueueHandler.Priority = ThreadPriority.AboveNormal;
                QueueHandlers[i] = QueueHandler;
                QueueHandler.Start();
            }

        }

        public static void Initialise() { }

        static void QueueHandling(int id) {
            Dictionary<string, NeuralNet> AvailableNetworks = new Dictionary<string, NeuralNet>();
            Dictionary<Shape, NDArray> Input = new Dictionary<Shape, NDArray>();
            Stopwatch SW = new Stopwatch();
            long ticksActive = 0;
            long ticksInactive = 0;
            int calls = 0;
            bool loadednet;
            //tf.compat.v1.disable_eager_execution();
            //FuncGraph graph = new FuncGraph($"G:{id}");
            //graph.as_default();

            var config = new ConfigProto {
                GpuOptions = new GPUOptions {
                    AllowGrowth = false,
                    PerProcessGpuMemoryFraction = 0.9 / threadcount
                }
            };
            tf.Context.Config = config;
            //tf.Context.ensure_initialized();

            //NDArray input = new NDArray(NPTypeCode.Float);

            while (true) {
                QueueNotification.Wait();
                ticksInactive += SW.ElapsedTicks;
                loadednet = false;
                SW.Restart();
                PredictionRequest request;
                if (requestQueue.TryDequeue(out request)) {
                    string networkName = request.NetworkName;
                    //Tensor input = new NDArray(request.Input, request.Shape);
                    //input.ReplaceData(request.Input);
                    //input.Shape = request.Shape;
                    NeuralNet nn;
                    if(!AvailableNetworks.TryGetValue(networkName, out nn)){
                        nn = NeuralNet.Load($"Resources/Neural Networks/{networkName}.bnn");
                        AvailableNetworks.Add(networkName, nn);
                        loadednet = true;
                    }

                    NDArray input;
                    if(!Input.TryGetValue(request.Shape, out input)) {
                        input = new NDArray(request.Input, request.Shape);
                        Input.Add(request.Shape, input);
                    }
                    input.ReplaceData(request.Input);
                    //input.Shape = request.Shape;

                    //if (tf.executing_eagerly())
                    //    MessageBox.Show("Eager!");
                    Tensors result = nn.Apply(input);
                    //Tensors result = nn.predict(input, 1000);
                    //unsafe {
                    //    // Load a single value to make sure the NN has executed properly.
                    //    float r = *(float*)result[0][0][0].buffer;
                    //}
                    float[][] r = new float[result[0].dims[0]][];
                    for (int i = 0; i < result[0].dims[0]; i++) {
                        r[i] = result[0][i].ToArray<float>();
                    }
                    foreach (Tensor t in result) {
                        t.Dispose();
                    }
                    result.Dispose();
                    request.Result = r;
                    request.OnFinished.Set();
                }
                else {
                    throw new Exception("Signalled to handle prediction request, but no request could be retrieved.");
                }
                if(!loadednet)
                    ticksActive += SW.ElapsedTicks;
                SW.Restart();
                calls++;
                int total = Interlocked.Increment(ref totalCalls);
                //if (calls % 100 == 0)
                //    Trace.TraceInformation($"T{id}:{(float)ticksActive / (ticksInactive + ticksActive)}");
                if (total % 300 == 0)
                    GC.Collect(1);
            }
        }

        public static float[][] RequestPrediction(float[] input, Shape shape, string networkName) {
            PredictionRequest request = new PredictionRequest(input, shape, networkName);
            requestQueue.Enqueue(request);
            QueueNotification.Release();
            request.OnFinished.WaitOne();
            return request.Result;
        }

    }

    public class PredictionRequest
    {
        public float[] Input { get; set; }
        public Shape Shape { get; set; }
        public string NetworkName { get; set; }

        public AutoResetEvent OnFinished { get; }

        public float[][] Result { get; set; }

        public PredictionRequest(float[] input, Shape shape, string networkName) {
            Input = input;
            Shape = shape;
            NetworkName = networkName;
            OnFinished = new AutoResetEvent(false);
        }
    }
}
