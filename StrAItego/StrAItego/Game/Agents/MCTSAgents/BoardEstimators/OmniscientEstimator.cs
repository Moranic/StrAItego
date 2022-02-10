using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    class OmniscientEstimator : BoardEstimator
    {
        public OmniscientEstimator() : base("Omniscient Estimator") { }


        public override Board EstimateBoard(Board fromBoard, Random r = null) {
            return new Board(fromBoard);
        }
    }
}
