using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NeuralNetworkModels.NeuralNetworks;
using NeuralNetworkTrainer.Data;
using NumSharp;
using Tensorflow;
using Tensorflow.Keras;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace NeuralNetworkTrainer
{
    class Program
    {
        static Random r = new Random();
        static List<string[]> filedata;
        static float[][] _data;
        static NDArray x_train, y_train, x_test, y_test;
        static IDatasetV2 train_data;

        static int batch_size = 250;
        static int training_steps = 1000000;
        static int cache_multiplier = 25;
        static float learning_rate = 0.001f;


        static int autosave_interval = 500;
        static int display_step = 100;

        static void Main(string[] args) {
            Console.WindowWidth = 280;
            Console.WindowHeight = 46;

            //var config = new ConfigProto {
            //    GpuOptions = new GPUOptions {
            //        AllowGrowth = true,
            //        PerProcessGpuMemoryFraction = 0.9
            //    }
            //};
            //

            // Debug
            if (args.Length == 0)
                args = new string[1] { "D:\\Documenten\\Master Thesis\\StrAItego\\SetupEvaluatorNetworkLearner\\bin\\Debug\\netcoreapp3.1\\setuptoresult.txt" };

            if (args.Length == 0) {
                Console.WriteLine("No file given. Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            //tf.enable_eager_execution();

            string netname = "StateEvaluator.bnn";

            //ConvertToPyFile("StateEvaluator.bnn", 3312);
            //Console.ReadKey();

            // Prepare data
            //(NDArray data, NDArray labels) = ReadData(args[0]);
            //
            //int validationcutoff = (int)Math.Round(_data.Length * 0.9f);
            //
            //x_train = data[new Slice(0, validationcutoff)];
            //y_train = labels[new Slice(0, validationcutoff)];
            //x_test = data[new Slice(validationcutoff, _data.Length)];
            //y_test = labels[new Slice(validationcutoff, _data.Length)];
            //
            //train_data = tf.data.Dataset.from_tensor_slices(x_train, y_train);
            //train_data = train_data.repeat()
            //    .shuffle(_data.Length)
            //    .batch(batch_size)
            //    .prefetch(1)
            //    .take(training_steps);

            //IDataProvider dataProvider = new GravonMoveDataProvider(cache_multiplier * batch_size, batch_size);
            IDataProvider dataProvider = new GravonSWRDataProvider(cache_multiplier * batch_size, batch_size);

            // Build model
            // UNCOMMENT TO TRAIN SETUP EVALUATOR
            //NeuralNet neural_net = new NeuralNet(new NeuralNetArgs {
            //    NumClasses = 2,
            //
            //    LayerArgs = new NeuralNetworks.LayerArgs[] {
            //        new NeuralNetworks.LayerArgs(150, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(150, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(150, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(150, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(150, keras.activations.Relu)
            //    }
            //});
            //
            //// Load previously trained model instead
            //var neural_net = NeuralNet.Load("RandomOrHumanSetup4.bnn");


            // UNCOMMENT TO TRAIN MOVE PREDICTOR
            //NeuralNet neural_net = new NeuralNet(new NeuralNetArgs {
            //    NumClasses = 1368,
            //
            //    LayerArgs = new NeuralNetworks.LayerArgs[] {
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu)
            //    }
            //});


            // UNCOMMENT TO TRAIN DIRECT RANK ESTIMATOR
            //DirectRankEstimationNet neural_net = new DirectRankEstimationNet(new GeneralNetArgs {
            //    LayerArgs = new NeuralNetworks.LayerArgs[] {
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(240, keras.activations.Relu, null),
            //        new NeuralNetworks.LayerArgs(12, null)
            //
            //    }
            //}, 0, 6);

            // UNCOMMENT TO TRAIN STATE EVALUATOR
            //NeuralNet neural_net = new NeuralNet(new NeuralNetArgs {
            //    NumClasses = 2,
            //
            //    LayerArgs = new NeuralNetworks.LayerArgs[] {
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu),
            //        new NeuralNetworks.LayerArgs(300, keras.activations.Relu)
            //    }
            //});

            NeuralNet neural_net = NeuralNet.Load("temp.bnn");

            //ConvertToPyFile("DirectRankEstimator1.bnn", 480);


            // Cross-Entropy Loss.
            // Note that this will apply 'softmax' to the logits.
            Func<Tensor, Tensor, Tensor> cross_entropy_loss = (x, y) => {
                // Convert labels to int 64 for tf cross-entropy function.
                
                y = tf.reshape(tf.cast(y, tf.int64), shape: batch_size);
                //y = tf.cast(y, tf.int64);
                // Apply softmax to logits and compute cross-entropy.
                var loss = tf.nn.sparse_softmax_cross_entropy_with_logits(labels: y, logits: x);
                // Average loss across the batch.
                return tf.reduce_mean(loss);
            };

            // Accuracy metric for classes
            Func<Tensor, Tensor, Tensor> classaccuracy = (y_pred, y_true) => {
                // Predicted class is the index of highest score in prediction vector (i.e. argmax).
                var correct_prediction = tf.equal(tf.argmax(y_pred, 1), tf.cast(y_true, tf.int64));
                return tf.reduce_mean(tf.cast(correct_prediction, tf.float32), axis: -1);
            };


            // Stochastic Gradient Optimiser
            var optimiser = keras.optimizers.SGD(learning_rate);

            // Optimisation process
            Action<Tensor, Tensor> run_optimisation = (x, y) => {
                // Wrap computation in GradientTape
                using var g = tf.GradientTape();
                // Forward pass
                var pred = neural_net.Apply(x, is_training: true);
                //var loss = mse_loss(pred, y);
                var loss = cross_entropy_loss(pred, y);
                // Compute gradients
                var gradients = g.gradient(loss, neural_net.trainable_variables);

                // Update W and b following gradients
                optimiser.apply_gradients(zip(gradients, neural_net.trainable_variables.Select(x => x as ResourceVariable)));
            };

            // Direct Rank Estimator Optimisation Process
            //Action<Tensor, Tensor> dre_run_optimisation = (x, y) => {
            //    // Wrap computation in GradientTape
            //    using (var g = tf.GradientTape(persistent:true)) {
            //        // Forward pass
            //        var pred = neural_net.Apply(x, is_training: true);
            //        //var loss = mse_loss(pred, y);
            //        //Tensor[] unstacked_y = new Tensor[40];
            //        //for(int i = 0; i < unstacked_y.Length; i++) {
            //        //    float[] labels = new float[batch_size];
            //        //    for (int j = 0; j < batch_size; j++)
            //        //        labels[j] = (float)y[j][i];
            //        //    unstacked_y[i] = new Tensor(labels);
            //        //}
            //
            //
            //
            //        //foreach (Tensor t in unstacked_y)
            //        //    g.watch(t);
            //
            //        var unstacked_y = tf.split(y, 40, 1);
            //        //var unstacked_y = tf.unstack(y, batch_size, 0);
            //        Tensor[] losses = new Tensor[unstacked_y.Length];
            //        for (int i = 0; i < unstacked_y.Length; i++) {
            //            var loss = cross_entropy_loss(pred[i], unstacked_y[i]);
            //            losses[i] = loss;
            //        }
            //
            //        var totalloss = tf.reduce_sum(losses);
            //
            //        // Compute gradients
            //        Tensor[][] individualgradients = new Tensor[40][];
            //        for (int i = 0; i < 40; i++)
            //            individualgradients[i] = g.gradient(losses[i], neural_net.TrainableVariablesByOutput(i));
            //
            //
            //        Tensor[] gradients = new Tensor[neural_net.layers.Length * 2];
            //        Tensor[] tensorstouse = new Tensor[40];
            //        for (int i = 0; i < neural_net.SharedLayers * 2; i++) {
            //          for (int j = 0; j < 40; j++)
            //              tensorstouse[j] = individualgradients[j][i];
            //          gradients[i] = tf.div(tf.add_n(tensorstouse), tf.constant(40f));
            //      }
            //        for(int i = 0; i < 40; i++) {
            //            for(int j = 0; j < neural_net.SeparateLayers; j++) {
            //                gradients[neural_net.SharedLayers * 2 + 2 * i * neural_net.SeparateLayers + j * 2]     = individualgradients[i][neural_net.SharedLayers * 2 + j * 2];
            //                gradients[neural_net.SharedLayers * 2 + 2 * i * neural_net.SeparateLayers + j * 2 + 1] = individualgradients[i][neural_net.SharedLayers * 2 + j * 2 + 1];
            //            }
            //            //gradients[neural_net.SharedLayers * 2 + i] = individualgradients[i / (neural_net.SeparateLayers * 2)][10];
            //            //gradients[neural_net.SharedLayers * 2 + 1 + i] = individualgradients[i / (neural_net.SeparateLayers * 2)][11];
            //        }
            //        
            //
            //        //var gradienttest = g.gradient(losses[0], neural_net.trainable_variables.Take(12));
            //        
            //        //var gradients = g.gradient(totalloss, neural_net.trainable_variables);
            //
            //        // Update W and b following gradients
            //        optimiser.apply_gradients(zip(gradients, neural_net.trainable_variables.Select(x => x as ResourceVariable)));
            //    }
            //};


            Stopwatch sw = new Stopwatch();
            Stopwatch tsw = new Stopwatch();
            sw.Start();
            tsw.Start();
            TimeSpan avgdsteptime = new TimeSpan(0);
            float lastloss = 0;
            float lastacc = 0;
            // Run training for the given number of steps.
            int totalsteps = 0;

            Directory.CreateDirectory(netname + " checkpoints");

            // UNCOMMENT TO TRAIN WITH DATAPROVIDER
            //while (totalsteps < training_steps) {
            //    train_data = dataProvider.GetNextSet();
            //    dataProvider.BeginPreparingNextSet();
            //    foreach (var (step, (batch_x, batch_y)) in enumerate(train_data, 0, 1)) {
            //        // Run the optimization to update W and b values.
            //        dre_run_optimisation(batch_x, batch_y);
            //        totalsteps++;
            //
            //        if (totalsteps % display_step == 0) {
            //            var pred = neural_net.Apply(batch_x, is_training: true);
            //
            //            // Calculate loss
            //            var unstacked_y = tf.split(batch_y, 40, 1);
            //            
            //            Tensor[] losses = new Tensor[unstacked_y.Length];
            //            for (int i = 0; i < unstacked_y.Length; i++) {
            //                var loss = cross_entropy_loss(pred[i], unstacked_y[i]);
            //                losses[i] = loss;
            //            }
            //
            //            var totalloss = tf.reduce_sum(losses);
            //            
            //            // Calculate accuracy
            //            Tensor[] accuracies = new Tensor[unstacked_y.Length];
            //            for (int i = 0; i < unstacked_y.Length; i++) {
            //                var accuracy = classaccuracy(pred[i], unstacked_y[i]);
            //                accuracies[i] = accuracy;
            //            }
            //
            //            var totalaccuracy = tf.reduce_mean(accuracies);
            //
            //            int currdisplaystep = (totalsteps / display_step);
            //            avgdsteptime = new TimeSpan((long)(avgdsteptime.Ticks * ((currdisplaystep - 1f) / currdisplaystep) + sw.Elapsed.Ticks * (1f / currdisplaystep)));
            //            TimeSpan remaining = new TimeSpan(((training_steps - totalsteps) / display_step) * avgdsteptime.Ticks);
            //
            //            print($"step: {totalsteps,+6} (sub: {step,+6}), loss: {(float)totalloss,-10}, accuracy: {(float)totalaccuracy,-5}, remaining: {remaining}");
            //            sw.Restart();
            //            lastloss = (float)totalloss;
            //            lastacc = (float)totalaccuracy;
            //        }
            //
            //        if (totalsteps % autosave_interval == 0) {
            //            neural_net.Save($"{netname} checkpoints\\checkpoint s {totalsteps} l {lastloss} a {lastacc}.bnn");
            //        }
            //
            //        if (step == cache_multiplier - 1)
            //            break;
            //    }
            //    train_data = null;
            //    GC.Collect();
            //}

            // UNCOMMENT TO TRAIN WITH DATAPROVIDER
            while (totalsteps < training_steps) {
                train_data = dataProvider.GetNextSet();
                dataProvider.BeginPreparingNextSet();
                foreach (var (step, (batch_x, batch_y)) in enumerate(train_data, 0, 1)) {
                    // Run the optimization to update W and b values.
                    run_optimisation(batch_x, batch_y);
                    totalsteps++;
            
                    if (totalsteps % display_step == 0) {
                        var pred = neural_net.Apply(batch_x, is_training: true);
                        var loss = cross_entropy_loss(pred, batch_y);
                        var acc = classaccuracy(pred, batch_y);
                        int currdisplaystep = (totalsteps / display_step);
                        avgdsteptime = new TimeSpan((long)(avgdsteptime.Ticks * ((currdisplaystep - 1f) / currdisplaystep) + sw.Elapsed.Ticks * (1f / currdisplaystep)));
                        TimeSpan remaining = new TimeSpan(((training_steps - totalsteps) / display_step) * avgdsteptime.Ticks);
            
                        print($"step: {totalsteps,+6} (sub: {step,+6}), loss: {(float)loss,-10}, accuracy: {(float)acc,-5}, remaining: {remaining}");
                        sw.Restart();
                        lastloss = (float)loss;
                        lastacc = (float)acc;
                    }
            
                    if (totalsteps % autosave_interval == 0) {
                        neural_net.Save($"{netname} checkpoints\\checkpoint s {totalsteps} l {lastloss} a {lastacc}.bnn");
                    }
            
                    if (step == cache_multiplier - 1)
                        break;
                }
                train_data = null;
                GC.Collect();
            }

            //foreach (var (step, (batch_x, batch_y)) in enumerate(train_data, 0, 1)) {
            //    // Run the optimization to update W and b values.
            //    run_optimisation(batch_x, batch_y);
            //    totalsteps++;
            //
            //    if (totalsteps % display_step == 0) {
            //        var pred = neural_net.Apply(batch_x, is_training: true);
            //        var loss = cross_entropy_loss(pred, batch_y);
            //        var acc = classaccuracy(pred, batch_y);
            //        int currdisplaystep = (totalsteps / display_step);
            //        avgdsteptime = new TimeSpan((long)(avgdsteptime.Ticks * ((currdisplaystep - 1f) / currdisplaystep) + sw.Elapsed.Ticks * (1f / currdisplaystep)));
            //        TimeSpan remaining = new TimeSpan(((training_steps - totalsteps) / display_step) * avgdsteptime.Ticks);
            //
            //        print($"step: {totalsteps,+6} (sub: {step,+6}), loss: {(float)loss,-10}, accuracy: {(float)acc,-5}, remaining: {remaining}");
            //        sw.Restart();
            //        lastloss = (float)loss;
            //        lastacc = (float)acc;
            //    }
            //
            //    if(totalsteps % autosave_interval == 0){
            //        neural_net.Save($"{netname} checkpoints\\checkpoint s {totalsteps} l {lastloss} a {lastacc}.bnn");
            //    }
            //
            //    if (step == training_steps - 1)
            //        break;
            //}

            tsw.Stop();
            print($"Total elapsed time: {tsw.Elapsed}");

            //DEBUG: PRINT NEURAL NETWORK
            //foreach (var x in neural_net.trainable_variables) {
            //    print(x);
            //}
            //print(neural_net.trainable_variables.Count);
            //print(neural_net.non_trainable_variables.Count);

            //Console.ReadKey();
            //var vars = neural_net.trainable_variables;
            //var testvar = vars[0].AsTensor(TF_DataType.TF_FLOAT);

            neural_net.Save(netname);

            // Reload network for testing
            var test_net = NeuralNet.Load(netname);

            // Test model on validation set. These results should be identical for both sets.
            {
                //TestNet(neural_net, 1337);
                //print("----------------------------------------------");
                //TestNet(test_net, 1337);
            }

            Console.WriteLine("Done, press ESCAPE to exit.");
            while (Console.ReadKey().Key != ConsoleKey.Escape) {
            }

        }

        private static void ConvertToPyFile(string netname, int inputlength) {
            Console.WriteLine($"Converting {netname} to .py file");
            NeuralNet toprint = NeuralNet.Load(netname);
            Console.WriteLine($"Loaded {netname}");
            NDArray t = new NDArray(new float[inputlength], shape: (1, inputlength));
            toprint.predict(t);
            using (StreamWriter printer = new StreamWriter($"{netname}.py")) {
                printer.WriteLine(
                    "import tensorflow as tf\r\n" +
                    "from tensorflow import keras\r\n" +
                    "import numpy as np\r\n" +
                    "import os\r\n");
                for (int i = 0; i < toprint.layers.Length; i++) {
                    ILayer layer = toprint.layers[i];
                    float[] Kernel = layer.trainable_variables[0].AsTensor(TF_DataType.TF_FLOAT).ToArray<float>();
                    float[] Bias = layer.trainable_variables[1].AsTensor(TF_DataType.TF_FLOAT).ToArray<float>();
                    printer.WriteLine($"def kernel{i + 1}(shape, dtype=None):");
                    printer.Write($"    return np.array([{Kernel[0]}");
                    for(int j = 1; j < Kernel.Length; j++) {
                        printer.Write($", {Kernel[j]}");
                    }
                    printer.WriteLine("]).reshape(shape)");
                    printer.WriteLine();

                    printer.WriteLine($"def bias{i + 1}(shape, dtype=None):");
                    printer.Write($"    return np.array([{Kernel[0]}");
                    for (int j = 1; j < Bias.Length; j++) {
                        printer.Write($", {Bias[j]}");
                    }
                    printer.WriteLine("]).reshape(shape)");
                    printer.WriteLine();
                }
            }
            Console.WriteLine($"Finished writing to .py file, press any key to continue");
            Console.ReadKey();
        }

        public static void TestNet(NeuralNet neural_net, int seed = 0) {
            if (seed != 0)
                r = new Random(seed);
            // Accuracy metric for classes
            Func<Tensor, Tensor, Tensor> classaccuracy = (y_pred, y_true) => {
                // Predicted class is the index of highest score in prediction vector (i.e. argmax).
                var correct_prediction = tf.equal(tf.argmax(y_pred, 1), tf.cast(y_true, tf.int64));
                return tf.reduce_mean(tf.cast(correct_prediction, tf.float32), axis: -1);
            };

            var pred = neural_net.Apply(x_test, is_training: false);
            float acc = (float)classaccuracy(pred, y_test);
            print($"Test Accuracy: {acc}");

            float avgCorrectConfidence = 0f;
            float avgIncorrectConfidence = 0f;
            int correctGuesses = 0;
            int incorrectGuesses = 0;
            float[,] avgConfidence = new float[3, 2];
            int[,] guesses = new int[3, 2];

            for (int i = 0; i < pred[0].dims[0]; i++) {
                float[] results = pred[0][i].ToArray<float>();
                int guess = results[0] > results[1] ? 0 : 1;
                //int guess = results[0] > results[1] ?
                //                                    results[0] > results[2] ?
                //                                        0 :
                //                                        2 :
                //                                        results[1] > results[2] ?
                //                                            1 :
                //                                            2;
                int actual = (int)Math.Round((float)y_test[new Slice(i, i + 1)].GetAtIndex(0));
                string result = actual == guess ? "Correct" : "Incorrect";
                int ind = result == "Correct" ? 0 : 1;
                guesses[actual, ind]++;
                avgConfidence[actual, ind] = avgConfidence[actual, ind] * ((guesses[actual, ind] - 1f) / guesses[actual, ind]) + results[guess] * (1f / guesses[actual, ind]);

                if (result == "Correct") {
                    correctGuesses++;
                    avgCorrectConfidence = avgCorrectConfidence * ((correctGuesses - 1f) / correctGuesses) + results[guess] * (1f / correctGuesses);
                }
                else {
                    incorrectGuesses++;
                    avgIncorrectConfidence = avgIncorrectConfidence * ((incorrectGuesses - 1f) / incorrectGuesses) + results[guess] * (1f / incorrectGuesses);
                }
                if (i < 20)
                    print($"Test: {y_test[new Slice(i, i + 1)]} | Pred: [" +
                        $"{results[0]}" +
                        $", {results[1]}" +
                        //$", {results[2]}" +
                        $"], guess: {guess}, {result}");
            }
            print($"Avg. correct confidence:   {avgCorrectConfidence}");
            print($"Avg. incorrect confidence: {avgIncorrectConfidence}");
            print($"Avg. 0 correct: {guesses[0, 0] / (float)(guesses[0, 0] + guesses[0, 1])}");
            print($"Avg. 1 correct: {guesses[1, 0] / (float)(guesses[1, 0] + guesses[1, 1])}");
            //print($"Avg. 2 correct: {guesses[2, 0] / (float)(guesses[2, 0] + guesses[2, 1])}");
            print($"Avg. 0 correct confidence: {avgConfidence[0, 0]}");
            print($"Avg. 1 correct confidence: {avgConfidence[1, 0]}");
            //print($"Avg. 2 correct confidence: {avgConfidence[2, 0]}");
            print($"Avg. 0 incorrect confidence: {avgConfidence[0, 1]}");
            print($"Avg. 1 incorrect confidence: {avgConfidence[1, 1]}");
            //print($"Avg. 2 incorrect confidence: {avgConfidence[2, 1]}");

            NDArray randomSetups = new NDArray(CreateRandomSetups(10000), shape: (10000, 480));
            var randompred = neural_net.Apply(randomSetups, is_training: false);
            float[] avgRandConfidence = new float[3];
            int[] randGuesses = new int[3];
            for (int i = 0; i < randompred[0].dims[0]; i++) {
                float[] results = randompred[0][i].ToArray<float>();
                int guess = results[0] > results[1] ? 0 : 1;
                //int guess = results[0] > results[1] ?
                //                                    results[0] > results[2] ?
                //                                        0 :
                //                                        2 :
                //                                        results[1] > results[2] ?
                //                                            1 :
                //                                            2;
                randGuesses[guess]++;
                avgRandConfidence[guess] = avgRandConfidence[guess] * ((randGuesses[guess] - 1f) / randGuesses[guess]) + results[guess] * (1f / randGuesses[guess]);
                if (i < 5)
                    print($"Random Test Pred: [" +
                        $"{results[0]}" +
                        $", {results[1]}" +
                        //$", {results[2]}" +
                        $"], guess: {guess}");
            }
            print($"Guessed 0: {randGuesses[0]}");
            print($"Guessed 1: {randGuesses[1]}");
            //print($"Guessed 2: {randGuesses[2]}");
            print($"Avg. 0 confidence: {avgRandConfidence[0]}");
            print($"Avg. 1 confidence: {avgRandConfidence[1]}");
            //print($"Avg. 2 confidence: {avgRandConfidence[2]}");
        }

        static float[][] CreateRandomSetups(int num) {
            // Prepare data
            float[][] _data = new float[num][];
            for (int i = 0; i < num; i++) {
                string setup = "FMBBBBBB12222222233333444455556666777889";
                _data[i] = SetupStringToBinary(new string(setup.OrderBy(x => r.Next()).ToArray()));
            }
            return _data;
        }

        static float[][] CreateAdversarySetups(int num, NeuralNet nn) {
            print("Creating adversary setups...");
            int made = 0;
            float[][] _data = new float[num][];
            while(made < num) {
                float[][] tryset = CreateRandomSetups(1000);
                NDArray tryarr = new NDArray(tryset, shape: (1000, 480));
                var pred = nn.Apply(tryarr);
                for (int i = 0; i < pred[0].dims[0]; i++) {
                    float[] results = pred[0][i].ToArray<float>();
                    if (results[0] > 0.9 && made < num) {
                        _data[made++] = tryset[i];
                    }
                }
                print($"Generated {made}/{num}...");
            }
            print("Done!");
            print("Writing to file...");
            using (FileStream fs = new FileStream("AdversarySetups", FileMode.OpenOrCreate)) {
                using (BinaryWriter bw = new BinaryWriter(fs)) {
                    foreach(float[] setup in _data) {
                        foreach (float value in setup)
                            bw.Write(value);
                    }
                }
            }
            return _data;
        }

        static (NDArray, NDArray) ReadData(string path) {
            // Read file to memory
            filedata = new List<string[]>();
            Console.WriteLine("Opening: " + path);
            using (StreamReader sr = new StreamReader(path)) {
                string line;
                line = sr.ReadLine();
                while (line != null) {
                    //if(!line.Contains(";Tie"))
                    filedata.Add(line.Split(';'));
                    line = sr.ReadLine();
                }
            }
            Console.WriteLine("Finished reading file.");


            // Randomise order
            filedata = filedata.OrderBy(x => r.Next()).ToList();

            // Prepare data
            _data = new float[filedata.Count * 2][];
            for (int i = 0; i < filedata.Count; i++) {
                _data[i] = SetupStringToBinary(filedata[i][0]);
            }

            // Load network for creating adversary setups and generate random ones
            var advnn = NeuralNet.Load("RandomOrHumanSetup4.bnn");
            //var adversarysetups = CreateAdversarySetups(filedata.Count / 2, advnn);

            //Load from file
            float[][] adversarysetups = new float[filedata.Count / 2][];
            using(FileStream fs = new FileStream("AdversarySetups", FileMode.Open)) {
                using (BinaryReader br = new BinaryReader(fs)) {
                    for (int i = 0; i < filedata.Count / 2; i++) {
                        float[] setup = new float[480];
                        for (int j = 0; j < 480; j++) {
                            setup[j] = br.ReadSingle();
                        }
                        adversarysetups[i] = setup;
                    }
                }
            }
            var randomsetups = CreateRandomSetups(filedata.Count / 2); // Div by 2 iff adversary included
            randomsetups.CopyTo(_data, filedata.Count);  // Random
            adversarysetups.CopyTo(_data, filedata.Count + (filedata.Count / 2)); // Adversary



            // Prepare labels
            float[] _labels = new float[filedata.Count * 2];
            for (int i = 0; i < filedata.Count; i++) {
                _labels[i] = ResultToLabel(filedata[i][2]);
                _labels[i + filedata.Count] = 1;                // Random
            }
            // Randomise order
            int seed = r.Next();
            Random rt = new Random(seed);
            _data = _data.OrderBy(x => rt.Next()).ToArray();
            rt = new Random(seed);
            _labels = _labels.OrderBy(x => rt.Next()).ToArray();


            NDArray data = new NDArray(_data, shape: (_data.Length, 480));
            NDArray labels = new NDArray(_labels);

            return (data, labels);
        }

        static long ResultToLabel(string result) {
            switch (result) {
                case "Victory": return 0;
                case "Tie": return 0;
                case "Defeat": return 0;
                default: throw new ArgumentException("Unknown result type! " + result);
            }
        }

        static float[] SetupStringToBinary(string setup) {
            float[] binary = new float[480];
            for (int i = 0; i < setup.Length; i++) {
                char c = setup[i];
                switch (c) {
                    case 'F': binary[12 * i + 0] = 1; break;
                    case '1': binary[12 * i + 1] = 1; break;
                    case '2': binary[12 * i + 2] = 1; break;
                    case '3': binary[12 * i + 3] = 1; break;
                    case '4': binary[12 * i + 4] = 1; break;
                    case '5': binary[12 * i + 5] = 1; break;
                    case '6': binary[12 * i + 6] = 1; break;
                    case '7': binary[12 * i + 7] = 1; break;
                    case '8': binary[12 * i + 8] = 1; break;
                    case '9': binary[12 * i + 9] = 1; break;
                    case 'M': binary[12 * i + 10] = 1; break;
                    case 'B': binary[12 * i + 11] = 1; break;
                    default: throw new ArgumentException("Unknown character " + c + " in setup string.");
                }
            }
            return binary;
        }
    }
}