using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;

namespace NeuralNetworkTrainer.Data
{
    interface IDataProvider
    {
        public IDatasetV2 GetNextSet();

        public void BeginPreparingNextSet();
    }
}
