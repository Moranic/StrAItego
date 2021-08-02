using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    interface IBoardEstimator : IDisposable
    {
        public Board EstimateBoard(Board fromBoard, Random r = null);
    }
}
