using NumSharp;
using NumSharp.Utilities;
using StrAItego.Game.TFLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    class NaiveNeuralNetworkEstimator : IBoardEstimator
    {
        RandomEstimator randomEstimator = new RandomEstimator();
        static int noOfEstimations = 1000;
        static float shortCircuit = 0.99f;
        Board bestBoard;
        Board currBoard;
        TFLiteModel model;

        public Board EstimateBoard(Board fromBoard, Random r = null) {
            if (model == null) {
                model = TFLiteManager.GetModel("RandomOrHumanSetup8");
                bestBoard = new Board();
                currBoard = new Board();
            }
            // Get a number of random estimations
            float[] binsetup = new float[480];
            float bestValue = 0f;
            for (int i = 0; i < noOfEstimations; i++) {
                randomEstimator.MakeEstimationOnBoard(fromBoard, currBoard, r);
                currBoard.SetupToBinary(Team.Blue, binsetup);
                float score = model.Predict(binsetup)[0];

                if(score > bestValue) {
                    Board t = bestBoard;
                    bestBoard = currBoard;
                    currBoard = t;
                    bestValue = score;
                }

                if (bestValue > shortCircuit) {
                    //MessageBox.Show(""+i);
                    return new Board(bestBoard);
                }
            }

            // Return copy of the best board we found.
            return new Board(bestBoard);
        }

        public static unsafe float Int64BitsToSingle(long value) {
            return *(float*)(&value);
        }

        public override string ToString() {
            return "Naive RvH NN Estimator";
        }

        public void Dispose() {
            model?.Dispose();
        }
    }
}
