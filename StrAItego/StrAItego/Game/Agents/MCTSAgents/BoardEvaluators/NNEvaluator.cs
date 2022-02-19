using System;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class NNEvaluator : BoardEvaluator
    {
        TFLiteModel model = null;
        float[] binboard = new float[3312];

        public NNEvaluator() : base("NN Evaluator") { }

        public override float EvaluateNode(Node n, Random r = null) {
            if (model == null)
                model = TFLiteManager.GetModel("StateEvaluator");
            
            Board b = n.Board;
            b.ToBinary(binboard, Team.Red);
            float score = model.Predict(binboard)[0];

            return score;
        }
    }
}
