using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class DoubleNNEvaluator : IBoardEvaluator
    {
        TFLiteModel model = null;
        float[] binboard = new float[3312];
        public float EvaluateNode(Node n, Random r = null) {
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

        public override string ToString() {
            return "Double NN Evaluator";
        }
    }
}
