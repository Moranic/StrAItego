using System;

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
