using System;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class DoubleNNNUCEvaluator : BoardEvaluator
    {
        TFLiteModel model = null;
        float[] binboard = new float[3312];

        NaiveUnitCountEvaluator nuc = new NaiveUnitCountEvaluator();

        public DoubleNNNUCEvaluator() : base("Double NN NUC Evaluator") { }

        public override float EvaluateNode(Node n, Random r = null) {
            if (model == null)
                model = TFLiteManager.GetModel("StateEvaluator");
            
            Board b = n.Board;
            b.ToBinary(binboard, Team.Red);
            float scoreR = model.Predict(binboard)[0];

            b.ToBinary(binboard, Team.Blue);
            float scoreB = model.Predict(binboard)[0];

            float scoreNN = (1f + scoreR - scoreB) / 2f;

            float scoreNUC = nuc.EvaluateNode(n, r);

            float score = (scoreNN + scoreNUC) / 2f;

            return score;
        }
    }
}
