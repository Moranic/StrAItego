using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    abstract class BoardEvaluator : IBoardEvaluator
    {
        private string name = "Board Evaluator";

        public BoardEvaluator(string boardEvaluatorName)
        {
            name = boardEvaluatorName;
        }

        public override string ToString()
        {
            return name;
        }

        public abstract float EvaluateNode(Node n, Random r = null);
    }
}
