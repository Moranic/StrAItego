using System;
using System.Collections.Generic;

namespace StrAItego.Game.Agents.MCTSAgents.UCB
{
    class UCBAgent : MCTSAgent
    {
        float c;
        float[] logLookupTable;

        public UCBAgent() : base() {
            name = "UCB Agent";
        }

        public override IAgentParameters GetParameters() {
            return new UCBAgentParameters();
        }

        protected override Move SelectMove(Node n) {
            List<Move> moves = n.GetMoves();
            if (!n.IsLeaf()) {
                (Move?, float) move = (null, -1f);
                foreach (Move m in moves) {
                    float ucb = CalcUCBValue(n, m);
                    if (move.Item2 < ucb)
                        move = (m, ucb);
                }

                if (move.Item1 is null)
                    throw new ArgumentException("Could not select a move to explore!");
                return (Move)move.Item1;
            }
            // Return random if this node has no children yet.
            return moves[r.Next(moves.Count)];
        }

        float CalcUCBValue(Node parent, Move m) {
            //(float, int) score = parent.GetScoreOfMove(m);
            int visits = parent.GetVisitsOfMove(m);
            if (visits == 0)
                return (10000) + r.Next(10000);
            float value = parent.GetValueOfMove(m);
            float ucb = (parent.TurnOfTeam == Team.Red ? (value / visits) : 1 - (value / visits)) + c * (float)Math.Sqrt(logLookupTable[parent.Visits] / visits);
            return ucb;
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            UCBAgentParameters parameters = (UCBAgentParameters)agentParameters;
            c = parameters.Confidence;
            name = parameters.ToString();
            logLookupTable = new float[rollouts];
            for (int i = 0; i < rollouts; i++)
                logLookupTable[i] = (float)Math.Log(i);
        }

        public override void Dispose() {
            logLookupTable = null;
            base.Dispose();
        }
    }
}
