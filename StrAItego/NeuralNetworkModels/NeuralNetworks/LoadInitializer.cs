using Tensorflow;
using Tensorflow.Eager;
using NumSharp;

namespace NeuralNetworkModels.NeuralNetworks
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
