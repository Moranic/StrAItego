using System;
using System.IO;
using System.Threading;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;

namespace NeuralNetworkTrainer.Data
{
    class GravonSDPDataProvider : IDataProvider
    {
        static byte[][] data = new byte[17123744][];
        static byte[][] labels = new byte[17123744][];
        float[] input;
        float[] output;
        int threadCount = 4;
        Thread[] threads;
        int currIndex = 0;
        int toFillIndex = 0;
        int filledIndex = 0;
        int cacheSize = 1;
        int batchSize = 1;
        NDArray x, y;
        IDatasetV2 train_data;
        EventWaitHandle nextSetReadyHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        EventWaitHandle nextSetCanBePrepared = new EventWaitHandle(true, EventResetMode.ManualReset);
        public GravonSDPDataProvider(int cache = 50000, int batch = 1000) {
            cacheSize = cache;
            batchSize = batch;
            Console.WriteLine("Initialising Gravon SDP Database, please wait...");

            using (BinaryReader br = new BinaryReader(File.OpenRead("ShuffledSDPDatabase"))) {
                for (int i = 0; i < 17123744; i++) { // add 4
                    data[i] = br.ReadBytes(60);
                    labels[i] = br.ReadBytes(40);
                }
            }

            Console.WriteLine("Loaded Gravon SDP Database");


            input = new float[cacheSize * 60 * 8];
            output = new float[cacheSize * 40];
            //for (int i = 0; i < input.Length; i++)
            //    input[i] = new float[3312];

            threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++) {
                threads[i] = new Thread(() => FillFloatArrays());
                threads[i].IsBackground = true;
                threads[i].Start();
            }


            nextSetCanBePrepared.Set();
        }

        public void BeginPreparingNextSet() {
            nextSetCanBePrepared.Set();
        }

        public IDatasetV2 GetNextSet() {
            // Wait until the next set is actually ready.
            nextSetReadyHandle.WaitOne();
            //cacheSize = size;


            //input = new float[size][];
            //output = new float[size];
            //for(int i = 0; i < size; i++) {
            //    short label = labels[currIndex];
            //    float[] item = BytesToData(data[currIndex++]);
            //    input[i] = item;
            //    output[i] = label;
            //    if (currIndex >= 17123744)
            //        currIndex = 0;
            //}




            //IDatasetV2 train_data = tf.data.Dataset.from_tensor_slices(x, y);
            //train_data = train_data.batch(batchSize).prefetch(1).take(cacheSize);

            // Signal that we need another dataset prepared
            //nextSetCanBePrepared.Set();
            return train_data;
        }

        void FillFloatArrays() {
            while (true) {
                nextSetCanBePrepared.WaitOne();
                int i = Interlocked.Increment(ref toFillIndex) - 1;
                while (i < cacheSize) {
                    // Fill input/output at i
                    byte[] label = labels[(currIndex + i) % 17123744];
                    BytesToData(data[(currIndex + i) % 17123744], i * 60 * 8);
                    //input[i * 414 * 8] = item;
                    for(int j = 0; j < 40; j++)
                        output[i * 40 + j] = label[j];


                    // Get next index to do
                    i = Interlocked.Increment(ref toFillIndex) - 1;

                    // Update on progress
                    int progress = Interlocked.Increment(ref filledIndex);

                    if (progress == cacheSize) {
                        // Put data in dataset
                        x = new NDArray(input, shape: (cacheSize, 60 * 8));
                        y = new NDArray(output, shape: (cacheSize, 40));
                        train_data = tf.data.Dataset.from_tensor_slices(x, y);
                        train_data = train_data.batch(batchSize).prefetch(1).take(cacheSize);
                        // We're done! Time to reset
                        input = new float[cacheSize * 60 * 8];
                        output = new float[cacheSize * 40];
                        toFillIndex = 0;
                        filledIndex = 0;
                        nextSetCanBePrepared.Reset();
                        nextSetReadyHandle.Set();
                        currIndex = (currIndex + cacheSize) % 17123744;
                    }
                }
            }
        }

        void BytesToData(byte[] data, int startingIndex) {
            //float[] conv = new float[414 * 8];
            for (int i = 0; i < 60; i++) {
                byte b = data[i];
                for (int j = 0; j < 8; j++) {
                    input[startingIndex + (i * 8 + j)] = ((b & (1 << (7 - j))) > 0) ? 1 : 0;
                }
            }
        }
    }
}
