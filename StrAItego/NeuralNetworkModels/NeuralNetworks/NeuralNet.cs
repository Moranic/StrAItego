using System;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using static Tensorflow.KerasApi;
using static Tensorflow.Binding;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

namespace NeuralNetworkModels.NeuralNetworks
{
    public class NeuralNet : Model
    {
        public ILayer[] layers;
        public Layer output;
        public NeuralNetArgs usedArgs;

        public NeuralNet(NeuralNetArgs args) :
            base(args) {
            var layersAPI = keras.layers;
            layers = new ILayer[args.LayerArgs.Length + 1];

            // Create hidden layers
            for(int i = 0; i < args.LayerArgs.Length; i++) {
                layers[i] = layersAPI.Dense(
                    args.LayerArgs[i].Neurons,
                    activation: args.LayerArgs[i].Activation,
                    kernel_initializer: args.LayerArgs[i].KernelInitializer,
                    bias_initializer: args.LayerArgs[i].BiasInitializer);
            }

            // Create output layer
            output = layersAPI.Dense(args.NumClasses, kernel_initializer: args.KernelInitializerOutput, bias_initializer: args.BiasInitializerOutput);
            layers[layers.Length - 1] = output;

            StackLayers(layers);
            usedArgs = args;
        }

        // Set forward pass.
        protected override Tensors Call(Tensors inputs, Tensor state = null, bool is_training = false) {
            foreach (Layer l in layers)
                inputs = l.Apply(inputs);

            if (!is_training)
                inputs = tf.nn.softmax(inputs);
            return inputs;
        }

        public void Save(string path) {
            new SerialisableNet(this).ToFile(path);
        }

        public static NeuralNet Load(string path) {
            return SerialisableNet.FromFile(path).ToNeuralNet();
        }

    }

    public class LayerArgs
    {
        public LayerArgs(int neurons, Activation activation, IInitializer kernelInitializer = null, IInitializer biasInitializer = null) {
            Neurons = neurons;
            Activation = activation;
            KernelInitializer = kernelInitializer;
            BiasInitializer = biasInitializer;
        }

        public int Neurons { get; set; }
        public Activation Activation { get; set; }
        public IInitializer KernelInitializer { get; set; }
        public IInitializer BiasInitializer { get; set; }

    }


    /// <summary>
    /// Network parameters.
    /// </summary>
    public class NeuralNetArgs : ModelArgs
    {
        public LayerArgs[] LayerArgs { get; set; }
        public int NumClasses { get; set; }
        public IInitializer KernelInitializerOutput { get; set; }
        public IInitializer BiasInitializerOutput { get; set; }
    }

    [Serializable]
    public class SerialisableNet
    {
        public SerialisableLayer[] serialisableLayers;

        public SerialisableNet() { }

        public SerialisableNet(NeuralNet neuralNetwork) {
            serialisableLayers = new SerialisableLayer[neuralNetwork.layers.Length];
            for(int i = 0; i < serialisableLayers.Length; i++) {
                if(i != serialisableLayers.Length - 1) {
                    // Regular layer
                    LayerArgs la = neuralNetwork.usedArgs.LayerArgs[i];
                    Layer l = (Layer)neuralNetwork.layers[i];
                    serialisableLayers[i] = new SerialisableLayer(l, la);
                }
                else {
                    // Output layer
                    Layer o = (Layer)neuralNetwork.layers[i];
                    int numClasses = neuralNetwork.usedArgs.NumClasses;
                    serialisableLayers[i] = new SerialisableLayer(o, numClasses);
                }
            }
        }

        public NeuralNet ToNeuralNet() {
            LayerArgs[] hiddenLayers = new LayerArgs[serialisableLayers.Length - 1];
            for(int i = 0; i < hiddenLayers.Length; i++) {
                hiddenLayers[i] = serialisableLayers[i].ToLayerArgs();
            }
            LayerArgs outputLayer = serialisableLayers[serialisableLayers.Length - 1].ToLayerArgs();

            NeuralNet nn = new NeuralNet(new NeuralNetArgs {
                LayerArgs = hiddenLayers,
                NumClasses = outputLayer.Neurons,
                KernelInitializerOutput = outputLayer.KernelInitializer,
                BiasInitializerOutput = outputLayer.BiasInitializer
            });

            return nn;
        }

        public void ToFile(string path) {
            FileInfo f = new FileInfo(path);
            FileStream fs = f.Open(FileMode.OpenOrCreate);

            //System.Xml.Serialization.XmlSerializer xmlwriter = new System.Xml.Serialization.XmlSerializer(typeof(SerialisableNet));
            //xmlwriter.Serialize(fs, this);

            BinaryFormatter bw = new BinaryFormatter();
            bw.Serialize(fs, this);
            fs.Flush();
            fs.Close();
        }

        public static SerialisableNet FromFile(string path) {
            FileInfo f = new FileInfo(path);
            SerialisableNet net = null;
            bool success = false;
            while (!success) {
                try {
                    using (FileStream fs = f.Open(FileMode.Open, FileAccess.Read, FileShare.Read)) {

                        //System.Xml.Serialization.XmlSerializer xmlwriter = new System.Xml.Serialization.XmlSerializer(typeof(SerialisableNet));
                        //SerialisableNet net = (SerialisableNet)xmlwriter.Deserialize(fs);

                        BinaryFormatter bw = new BinaryFormatter();
                        net = (SerialisableNet)bw.Deserialize(fs);
                        success = true;
                    }
                }
                catch {
                    Thread.Sleep(100);
                };
            }
            return net;
        }

    }


    [Serializable]
    public class SerialisableLayer
    {
        public SerialisableLayer() { }

        public SerialisableLayer(Layer output, int numClasses) {
            Neurons = numClasses;
            Kernel = output.trainable_variables[0].AsTensor(TF_DataType.TF_FLOAT).ToArray<float>();
            Bias = output.trainable_variables[1].AsTensor(TF_DataType.TF_FLOAT).ToArray<float>();
        }

        public SerialisableLayer(Layer layer, LayerArgs args) {
            Neurons = args.Neurons;
            Activation = args.Activation == keras.activations.Linear ? ActivationType.Linear :
                         args.Activation == keras.activations.Relu ? ActivationType.Relu :
                         args.Activation == keras.activations.Sigmoid ? ActivationType.Sigmoid :
                         args.Activation == keras.activations.Tanh ? ActivationType.Tanh :
                         ActivationType.Unknown;
            Kernel = layer.trainable_variables[0].AsTensor(TF_DataType.TF_FLOAT).ToArray<float>();
            Bias = layer.trainable_variables[1].AsTensor(TF_DataType.TF_FLOAT).ToArray<float>();
        }

        public LayerArgs ToLayerArgs() {
            return new LayerArgs(
                Neurons, 
                Activation == ActivationType.Linear ? keras.activations.Linear :
                Activation == ActivationType.Relu ? keras.activations.Relu :
                Activation == ActivationType.Sigmoid ? keras.activations.Sigmoid :
                Activation == ActivationType.Tanh ? keras.activations.Tanh : 
                null,
                new LoadInitializer(new Tensor(Kernel)),
                new LoadInitializer(new Tensor(Bias)));

        }

        public int Neurons { get; set; }
        public ActivationType Activation { get; set; } = ActivationType.Unknown;
        public float[] Kernel { get; set; }
        public float[] Bias { get; set; }

        public enum ActivationType { Relu, Tanh, Linear, Sigmoid, Unknown};

    }
}
