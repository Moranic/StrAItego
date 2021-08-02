using System;
using System.Collections.Generic;
using System.Text;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using static Tensorflow.KerasApi;
using static Tensorflow.Binding;
using Tensorflow.Eager;
using NumSharp;

namespace NeuralNetworkTrainer.NeuralNetworks
{
    class LoadInitializer : IInitializer
    {
        float[] toLoad;

        public LoadInitializer(Tensor toLoad) {
            this.toLoad = toLoad.ToArray<float>();
        }

        public Tensor Apply(InitializerArgs args) {
            NDArray data = new NDArray(toLoad, args.Shape);
            //var et = new Tensor(data);
            var et = new EagerTensor(data, "from_file");
            return et;
        }
    }
}
