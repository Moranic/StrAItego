using System.Collections.Concurrent;
using System.IO;
using Emgu.TF.Lite;

namespace StrAItego.Game.TFLite
{
    public static class TFLiteManager
    {
        static ConcurrentDictionary<string, byte[]> byteBufferStorage = new ConcurrentDictionary<string, byte[]>();

        static TFLiteManager() {
            TfLiteInvoke.Init();
        }

        /// <summary>
        /// Empty method to initialise the static class.
        /// </summary>
        public static void Init() {
        }

        /// <summary>
        /// Gets a unique TFLiteModel object from a network name. Don't use this model concurrently, instead request a new model and use that one.
        /// </summary>
        /// <param name="name">The name of the model to request</param>
        /// <returns>A TFLiteModel object that can invoke the tflite model</returns>
        public static TFLiteModel GetModel(string name) {
            byte[] fileBuffer;
            if (!byteBufferStorage.TryGetValue(name, out fileBuffer)) {
                string filename = $"Resources/Neural Networks/{name}.tflite";
                FileInfo info = new FileInfo(filename);
                BinaryReader br = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
                fileBuffer = br.ReadBytes((int)info.Length);
                br.Close();
                byteBufferStorage.TryAdd(name, fileBuffer);
            }

            return new TFLiteModel(fileBuffer);
        }

    }
}
