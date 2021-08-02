using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class NNEvaluator : IBoardEvaluator
    {
        TFLiteModel model = null;
        float[] binboard = new float[3312];
        public float EvaluateNode(Node n, Random r = null) {
            if (model == null)
                model = TFLiteManager.GetModel("StateEvaluator");
            
            Board b = n.Board;
            b.ToBinary(binboard, Team.Red);
            float score = model.Predict(binboard)[0];

            return score;
        }

        public override string ToString() {
            return "NN Evaluator";
        }
    }
}
