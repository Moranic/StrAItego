using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    interface IBoardEstimator : IDisposable
    {
        public Board EstimateBoard(Board fromBoard, Random r = null);
    }
}
