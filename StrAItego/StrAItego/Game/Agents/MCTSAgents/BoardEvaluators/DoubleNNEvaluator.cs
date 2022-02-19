using System;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class DoubleNNEvaluator : BoardEvaluator
    {
        TFLiteModel model = null;
        float[] binboard = new float[3312];

        public DoubleNNEvaluator() : base("Double NN Evaluator") { }

        public override float EvaluateNode(Node n, Random r = null) {
            if (model == null)
                model = TFLiteManager.GetModel("StateEvaluator");
            
            Board b = n.Board;
            b.ToBinary(binboard, Team.Red);
            float scoreR = model.Predict(binboard)[0];

            b.ToBinary(binboard, Team.Blue);
            float scoreB = model.Predict(binboard)[0];

            float score = (1f + scoreR - scoreB) / 2f;

            return score;
        }
    }
}
