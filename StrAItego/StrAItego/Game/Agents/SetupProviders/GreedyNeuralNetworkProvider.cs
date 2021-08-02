/*
using NumSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;

namespace StrAItego.Game.Agents.SetupProviders
{
    class GreedyNeuralNetworkProvider : ISetupProvider
    {
        int numOfSwaps = 1000;

        static (int, int)[] swapTable;

        static GreedyNeuralNetworkProvider() {
            // Build swaptable
            swapTable = new (int, int)[780];
            int ind = 0;
            for(int i = 0; i < 39; i++) {
                for(int j = i + 1; j < 40; j++) {
                    swapTable[ind] = (i, j);
                    ind++;
                }
            }
        }

        public Rank[] GetSetup(Random r) {
            string startstring = new string("FMBBBBBB12222222233333444455556666777889".OrderBy(x => r.Next()).ToArray());
            float[] current = SetupStringToBinary(startstring);

            float[] setupsToTry = new float[780 * 480];
            for (int i = 0; i < (setupsToTry.Length / 480); i++)
                SetupStringToBinary(startstring).CopyTo(setupsToTry, 480 * i);

            int[] activeIndices = SetupStringToActiveIndices(startstring);
            // Setup initial setups
            for(int i = 0; i < 780; i++) {
                (int from, int to) = swapTable[i];
                // Disable indices
                setupsToTry[i * 480 + 12 * from + activeIndices[from]] = 0;
                setupsToTry[i * 480 + 12 * to   + activeIndices[to  ]] = 0;
                // Enable swapped indices
                setupsToTry[i * 480 + 12 * from + activeIndices[to  ]] = 1;
                setupsToTry[i * 480 + 12 * to +   activeIndices[from]] = 1;
            }

            float bestscore = 0f;
            // Main loop
            for(int i = 0; i < numOfSwaps; i++) {
                //Tensors pred = TensorflowManager.RequestPrediction(new NDArray(setupsToTry, shape:(780, 480)), "RandomOrHumanSetup2");
                float[][] pred = TensorflowManager.RequestPrediction(setupsToTry, (780, 480), "RandomOrHumanSetup5");
                // Find most human-like setup.
                int bestIndex = -1;
                float bestValue = 0f;
                for (int j = 0; j < pred.Length; j++) {
                    float[] results = pred[j];
                    if (results[0] > bestValue) {
                        bestValue = results[0];
                        bestIndex = j;
                    }
                }
                if (bestValue > bestscore) {
                    bestscore = bestValue;
                    (int bestfrom, int bestto) = swapTable[bestIndex];

                    if (i != numOfSwaps - 1) {
                        // Setup next round
                        // Order: Undo original swap, apply best swap, apply new swap
                        for (int j = 0; j < 780; j++) {
                            (int from, int to) = swapTable[j];
                            // Undo original swap
                            // Disable swapped indices
                            setupsToTry[j * 480 + 12 * from + activeIndices[to]] = 0;
                            setupsToTry[j * 480 + 12 * to + activeIndices[from]] = 0;
                            // Enable original indices
                            setupsToTry[j * 480 + 12 * from + activeIndices[from]] = 1;
                            setupsToTry[j * 480 + 12 * to + activeIndices[to]] = 1;
                                          
                            // Apply best swap
                            // Disable indices
                            setupsToTry[j * 480 + 12 * bestfrom + activeIndices[bestfrom]] = 0;
                            setupsToTry[j * 480 + 12 * bestto + activeIndices[bestto]] = 0;
                            // Enable swapped indices
                            setupsToTry[j * 480 + 12 * bestfrom + activeIndices[bestto]] = 1;
                            setupsToTry[j * 480 + 12 * bestto + activeIndices[bestfrom]] = 1;

                            // Apply new swap
                            // Disable indices
                            setupsToTry[j * 480 + 12 * from + activeIndices[from]] = 0;
                            setupsToTry[j * 480 + 12 * to + activeIndices[to]] = 0;
                            // Enable swapped indices
                            setupsToTry[j * 480 + 12 * from + activeIndices[to]] = 1;
                            setupsToTry[j * 480 + 12 * to + activeIndices[from]] = 1;
                        }
                    }
                    // Swap activeIndices
                    int t = activeIndices[bestfrom];
                    activeIndices[bestfrom] = activeIndices[bestto];
                    activeIndices[bestto] = t;
                }
                else {
                    break;
                }
            }

            Rank[] setup = ActiveIndicesToUnits(activeIndices);
            return setup;
        }


        static Rank[] ActiveIndicesToUnits(int[] active) {
            Rank[] setup = new Rank[40];
            for(int i = 0; i < 40; i++) {
                setup[i] = (Rank)(active[i] + 1);
            }
            return setup;
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

        static int[] SetupStringToActiveIndices(string setup) {
            int[] active = new int[480];
            for (int i = 0; i < setup.Length; i++) {
                char c = setup[i];
                switch (c) {
                    case 'F': active[i] = 0; break;
                    case '1': active[i] = 1; break;
                    case '2': active[i] = 2; break;
                    case '3': active[i] = 3; break;
                    case '4': active[i] = 4; break;
                    case '5': active[i] = 5; break;
                    case '6': active[i] = 6; break;
                    case '7': active[i] = 7; break;
                    case '8': active[i] = 8; break;
                    case '9': active[i] = 9; break;
                    case 'M': active[i] = 10; break;
                    case 'B': active[i] = 11; break;
                    default: throw new ArgumentException("Unknown character " + c + " in setup string.");
                }
            }
            return active;
        }

        public override string ToString() {
            return "Greedy RvH Neural Network Provider";
        }
    }
}
*/