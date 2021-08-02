using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    interface IBoardEvaluator
    {
        float EvaluateNode(Node n, Random r = null);
    }
}
