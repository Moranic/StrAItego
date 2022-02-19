using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace NeuralNetworkModels.NeuralNetworks
{
    class DirectRankEstimationNet : Model
    {
        /* Layout of network is as follows:
         * Feed current information about pieces in a onedimensional tensor of 480 floats (40 pieces x 12 possible ranks)
         * Input layer is followed by 5 hidden layers
         * Lastly, the output layers: 40 layers of 12 neurons each, softmaxed.
         */

        public ILayer[] layers;
        public GeneralNetArgs usedArgs;
        List<IVariableV1>[] trainable_variables_per_output = new List<IVariableV1>[40];
        bool initialised = false;
        public int SharedLayers = 1;
        public int SeparateLayers = 3;

        public DirectRankEstimationNet(GeneralNetArgs args, int SharedLayers, int SeparateLayers) :
            base(args) {
            this.SharedLayers = SharedLayers;
            this.SeparateLayers = SeparateLayers;
            var layersAPI = keras.layers;
            if (args.LayerArgs.Length != SharedLayers + SeparateLayers * 40)
                throw new ArgumentException("Not the right amount of layers!");
            layers = new ILayer[args.LayerArgs.Length];

            // Create hidden layers
            for (int i = 0; i < args.LayerArgs.Length; i++) {
                if(args.LayerArgs[i].Activation == null)
                    layers[i] = layersAPI.Dense(
                        args.LayerArgs[i].Neurons,
                        kernel_initializer: args.LayerArgs[i].KernelInitializer,
                        bias_initializer: args.LayerArgs[i].BiasInitializer);
                else
                    layers[i] = layersAPI.Dense(
                        args.LayerArgs[i].Neurons,
                        activation: args.LayerArgs[i].Activation,
                        kernel_initializer: args.LayerArgs[i].KernelInitializer,
                        bias_initializer: args.LayerArgs[i].BiasInitializer);
            }


            StackLayers(layers);
            usedArgs = args;
            
            
        }

        public List<IVariableV1> TrainableVariablesByOutput(int i) {
            return trainable_variables_per_output[i];
        }

        // Set forward pass.
        protected override Tensors Call(Tensors inputs, Tensor state = null, bool is_training = false) {
            for (int i = 0; i < SharedLayers; i++)
                inputs = layers[i].Apply(inputs);

            Tensor[] outputTensors = new Tensor[40];
            for (int i = 0; i < 40; i++) {
                outputTensors[i] = layers[SharedLayers + i * SeparateLayers].Apply(inputs);
                for (int j = 1; j < SeparateLayers; j++)
                    outputTensors[i] = layers[SharedLayers + i * SeparateLayers + j].Apply(outputTensors[i]);
            }

            if (!is_training)
                for (int i = 0; i < 40; i++)
                    outputTensors[i] = tf.nn.softmax(outputTensors[i]);

            if (!initialised) {
                for (int i = 0; i < 40; i++) {
                    trainable_variables_per_output[i] = new List<IVariableV1>((SharedLayers + SeparateLayers) * 2);
                    trainable_variables_per_output[i].AddRange(trainable_variables.Take(2 * SharedLayers));
                    for(int j = 0; j < SeparateLayers; j++) {
                        trainable_variables_per_output[i].Add(trainable_variables[(SharedLayers + i * SeparateLayers + j) * 2]);
                        trainable_variables_per_output[i].Add(trainable_variables[(SharedLayers + i * SeparateLayers + j) * 2 + 1]);
                    }
                    
                }
                initialised = true;
            }

            //Tensors output = tf.stack(outputTensors, axis: 1);
            return outputTensors;
        }

        public void Save(string path) {
            new SerialisableGeneralNet(layers, usedArgs).ToFile(path);
        }
        
        public static DirectRankEstimationNet Load(string path) {
            return new DirectRankEstimationNet(SerialisableGeneralNet.FromFile(path).ToGeneralNetArgs(), 0, 4);
        }
    }

    public class GeneralNetArgs : ModelArgs
    {
        // Use this if you're using a fixed model definition and are certain what the model will look like.
        public LayerArgs[] LayerArgs { get; set; }
    }

    [Serializable]
    public class SerialisableGeneralNet
    {
        public SerialisableLayer[] serialisableLayers;

        public SerialisableGeneralNet() { }

        public SerialisableGeneralNet(ILayer[] layers, GeneralNetArgs usedArgs) {
            serialisableLayers = new SerialisableLayer[layers.Length];
            for (int i = 0; i < serialisableLayers.Length; i++) {
                // Regular layer
                LayerArgs la = usedArgs.LayerArgs[i];
                Layer l = (Layer)layers[i];
                serialisableLayers[i] = new SerialisableLayer(l, la);
            }
        }

        public GeneralNetArgs ToGeneralNetArgs() {
            LayerArgs[] hiddenLayers = new LayerArgs[serialisableLayers.Length];
            for (int i = 0; i < hiddenLayers.Length; i++) {
                hiddenLayers[i] = serialisableLayers[i].ToLayerArgs();
            }

            GeneralNetArgs args = new GeneralNetArgs {
                LayerArgs = hiddenLayers
            };

            return args;
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

        public static SerialisableGeneralNet FromFile(string path) {
            FileInfo f = new FileInfo(path);
            SerialisableGeneralNet net = null;
            bool success = false;
            while (!success) {
                try {
                    using (FileStream fs = f.Open(FileMode.Open, FileAccess.Read, FileShare.Read)) {

                        //System.Xml.Serialization.XmlSerializer xmlwriter = new System.Xml.Serialization.XmlSerializer(typeof(SerialisableNet));
                        //SerialisableNet net = (SerialisableNet)xmlwriter.Deserialize(fs);

                        BinaryFormatter bw = new BinaryFormatter();
                        net = (SerialisableGeneralNet)bw.Deserialize(fs);
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
}
