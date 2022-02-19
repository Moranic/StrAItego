using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    abstract class BoardEstimator : IBoardEstimator
    {
        private string name = "Board Estimator";
        public BoardEstimator(string boardEstimatorName)
        {
            name = boardEstimatorName;
        }

        public override string ToString() => name;

        public virtual void Dispose() { }

        public abstract Board EstimateBoard(Board fromBoard, Random r = null);
    }
}
