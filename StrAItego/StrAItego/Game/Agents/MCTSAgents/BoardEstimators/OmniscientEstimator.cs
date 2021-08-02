using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    class OmniscientEstimator : IBoardEstimator
    {
        public void Dispose() {
        }

        public Board EstimateBoard(Board fromBoard, Random r = null) {
            return new Board(fromBoard);
        }

        public override string ToString() {
            return "Omniscience (cheating)";
        }
    }
}
