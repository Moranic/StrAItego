using System;
using System.Linq;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.SetupProviders
{
    class NaiveNeuralNetworkProvider : ISetupProvider
    {
        static int noOfRandomSetups = 10000;
        static float shortCircuit = 0.99f;
        TFLiteModel model;

        public Rank[] GetSetup(Random r) {
            if(model == null) {
                model = TFLiteManager.GetModel("RandomOrHumanSetup8");
            }


            float[] bestSetup = new float[480];
            float bestValue = 0f;
            for (int i = 0; i < noOfRandomSetups; i++) {
                float[] rsetup = CreateRandomSetup(r);
                float score = model.Predict(rsetup)[0];
                if(score > bestValue) {
                    bestSetup = rsetup;
                    bestValue = score;
                }
                if (bestValue > shortCircuit) {
                    //MessageBox.Show("" + i);
                    return BinarySetupToUnitArray(bestSetup);

                }
            }

            return BinarySetupToUnitArray(bestSetup);
        }

        static Rank[] BinarySetupToUnitArray(float[] binarySetup) {
            Rank[] setup = new Rank[40];

            for(int i = 0; i < 40; i++) {
                for(int j = 0; j < 12; j++) {
                    if(binarySetup[i * 12 + j] == 1f) {
                        setup[i] = (Rank)(j + 1);
                        continue;
                    }
                }
            }

            return setup;
        }

        static float[] CreateRandomSetup(Random r) {
            string setup = "FMBBBBBB12222222233333444455556666777889";
            return SetupStringToBinary(new string(setup.OrderBy(x => x == 'F' ? r.Next() / 2f : r.Next()).ToArray()));
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

        public override string ToString() {
            return "Naive RvH Neural Network Provider";
        }

        public void Dispose() {
            model?.Dispose();
        }
    }
}
