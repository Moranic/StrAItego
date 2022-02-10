using System;
using System.Runtime.InteropServices;
using Emgu.TF.Lite;

namespace StrAItego.Game.TFLite
{
    public class TFLiteModel : IDisposable
    {
        Interpreter interpreter;
        FlatBufferModel model;

        public TFLiteModel(byte[] fileBuffer) {
            //FileInfo info = new FileInfo(filename);
            //BinaryReader br = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
            //fileBuffer = br.ReadBytes((int)info.Length);
            //br.Close();
            model = new FlatBufferModel(fileBuffer);
            interpreter = new Interpreter(model);
            interpreter.AllocateTensors();
        }

        /// <summary>
        /// Takes the inputs set in the input tensor and applies the neural network to it.
        /// </summary>
        public void Invoke() {
            Status status = interpreter.Invoke();
            if (status == Status.Error)
                throw new Exception("Invoke returned Error status!");
        }

        /// <summary>
        /// The full prediction in one method.
        /// </summary>
        /// <param name="input">The model input</param>
        /// <returns>The predicted output</returns>
        public float[] Predict(float[] input) {
            SetInput(input);
            Invoke();
            return ApplySoftmax(GetOutputArray());
        }

        public float[][] PredictMultipleOutputs(float[] input, int noOfOutputs) {
            SetInput(input);
            Invoke();
            float[][] output = new float[noOfOutputs][];
            for (int i = 0; i < output.Length; i++)
                output[i] = ApplySoftmax(GetOutputArray(i));
            return output;
        }

        /// <summary>
        /// Applies in-place softmax to an array 
        /// </summary>
        /// <param name="array">The input array</param>
        /// <returns>The modified input array</returns>
        static float[] ApplySoftmax(float[] array) {
            float expsum = 0;
            float val;
            for(int i = 0; i < array.Length; i++) {
                val = (float)Exp(array[i]);
                expsum += val;
                array[i] = val;
            }

            val = 1f / expsum;

            for(int i = 0; i < array.Length; i++) {
                array[i] *= val;
            }

            return array;
        }

        /// <summary>
        /// Set the input values to the input tensor.
        /// </summary>
        /// <param name="values"></param>
        public void SetInput(float[] values) {
            //unsafe {
            //    float* arrpointer = (float*)interpreter.Inputs[0].DataPointer;
            //    foreach (float value in values) {
            //        *arrpointer = value;
            //        arrpointer++;
            //    }
            //}
            Marshal.Copy(values, 0, interpreter.Inputs[0].DataPointer, values.Length);
        }


        /// <summary>
        /// For testing purposes only, to check if input is set correctly.
        /// </summary>
        /// <returns>A float[] that represents the currently set input</returns>
        public float[] GetInputArray() {
            return (float[])interpreter.Inputs[0].Data;
        }

        /// <summary>
        /// Get the output values of the neural network.
        /// </summary>
        /// <returns></returns>
        public float[] GetOutputArray(int outputLayer = 0) {
            return (float[])interpreter.Outputs[outputLayer].Data;
        }

        /// <summary>
        /// Dispose this object and the underlying interpreter.
        /// </summary>
        public void Dispose() {
            interpreter.Dispose();
            model.Dispose();
        }

        /// <summary>
        /// Fast Math.Exp() method
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double Exp(double val) {
            long tmp = (long)(1512775 * val + 1072632447);
            return BitConverter.Int64BitsToDouble(tmp << 32);
        }
    }
}
