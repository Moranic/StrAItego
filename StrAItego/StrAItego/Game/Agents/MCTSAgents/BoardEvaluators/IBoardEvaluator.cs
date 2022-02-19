using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    interface IBoardEvaluator
    {
        float EvaluateNode(Node n, Random r = null);
    }
}
