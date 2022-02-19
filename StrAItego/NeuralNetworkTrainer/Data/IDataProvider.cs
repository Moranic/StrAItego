using Tensorflow;

namespace NeuralNetworkTrainer.Data
{
    interface IDataProvider
    {
        public IDatasetV2 GetNextSet();

        public void BeginPreparingNextSet();
    }
}
