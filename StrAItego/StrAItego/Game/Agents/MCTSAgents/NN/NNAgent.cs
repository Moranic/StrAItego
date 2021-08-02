using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.TF.Lite;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.MCTSAgents.NN
{
    class NNAgent : MCTSAgent
    {
        TFLiteModel model;

        protected float[] calcBuffer = new float[3312];
        public NNAgent() : base() {
            name = "NN Agent";
            nodeImplementation = typeof(NNNode);
        }

        public override Move? GetMove(Board board, GameLogger gameLogger) {
            if (model == null) {
                model = TFLiteManager.GetModel("HumanMovePredictor2");
            }
            return base.GetMove(board, gameLogger);
        }

        protected override Move SelectMove(Node n) {
            NNNode node = (NNNode)n;
            float randValue = (float)r.NextDouble();
            float cumulativeTotal = 0f;
            foreach(Move m in node.GetMoves()) {
                float prob = node.GetProbabilityOfChild(m, calcBuffer);
                cumulativeTotal += prob;
                if (randValue < cumulativeTotal)
                    return m;
            }
            // We don't expect to get here very often, but due to rounding errors it might be possible. Maybe.
            return node.GetMoves().Last();
        }

        public override IAgentParameters GetParameters() {
            return new NNAgentParameters();
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            NNAgentParameters parameters = (NNAgentParameters)agentParameters;
            name = parameters.ToString();
        }

        public TFLiteModel PredictionModel {
            get { return model; }
        }

        public override void Dispose() {
            base.Dispose();
            model?.Dispose();
        }
    }


    class NNNode : Node
    {
        float[] childProbabilites;
        float probabilityMultiplier;
        bool calculatedProbabilities = false;
        public NNNode(MCTSAgent agent) : base(agent) {
            childProbabilites = new float[Board.maxBranchingFactor];
        }

        public override void MakeTopNode(Board board, Team team) {
            base.MakeTopNode(board, team);
            calculatedProbabilities = false;
        }

        public override void MakeChildNode(Node parent, Move m) {
            base.MakeChildNode(parent, m);
            calculatedProbabilities = false;
        }

        public float GetProbabilityOfChild(Move m, float[] calcBuffer) {
            if (!calculatedProbabilities) {
                //PredictProbabilities(calcBuffer);
                PredictProbabilitiesLite(calcBuffer, ((NNAgent)agent).PredictionModel);
            }
            return childProbabilites[m.ID] * probabilityMultiplier;
        }

        /// <summary>
        /// Uses the deprecated TensorflowManager class to get the predictions.
        /// </summary>
        /// <param name="calcBuffer"></param>
        //void PredictProbabilities(float[] calcBuffer) {
        //    float[] binBoard = Board.ToBinary(calcBuffer, TurnOfTeam);
        //    float[] pred = TensorflowManager.RequestPrediction(binBoard, shape: (1, binBoard.Length), "HumanMovePredictor")[0];
        //    float total = 0f;
        //    for (int i = 0; i < nextMoves.Count; i++) {
        //        childProbabilites[i] = pred[Board.GetIndexOfMove(TurnOfTeam == Team.Red ? nextMoves[i] : nextMoves[i].GetInvertedMove())];
        //        total += childProbabilites[i];
        //    }
        //    probabilityMultiplier = 1f / total;
        //    calculatedProbabilities = true;
        //}

        void PredictProbabilitiesLite(float[] calcBuffer, TFLiteModel model) {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            
            float[] pred = model.Predict(Board.ToBinary(calcBuffer, TurnOfTeam));
            float total = 0f;
            for (int i = 0; i < nextMoves.Count; i++) {
                childProbabilites[i] = pred[Board.GetIndexOfMove(TurnOfTeam == Team.Red ? nextMoves[i] : nextMoves[i].GetInvertedMove())];
                total += childProbabilites[i];
            }
            probabilityMultiplier = 1f / total;
            calculatedProbabilities = true;
            //sw.Stop();
            //MessageBox.Show(""+sw.ElapsedTicks);
        }
    }
}
