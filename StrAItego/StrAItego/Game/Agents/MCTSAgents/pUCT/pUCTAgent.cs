using System;
using System.Collections.Generic;
using StrAItego.Game.Agents.MCTSAgents.NN;

namespace StrAItego.Game.Agents.MCTSAgents.pUCT
{
    class pUCTAgent : NNAgent
    {
        float c1, c2;
        float[] logLookupTable;
        float[] rootLookupTable;

        public pUCTAgent() : base() {
            name = "pUCT Agent";
        }

        protected override Move SelectMove(Node n) {
            List<Move> moves = n.GetMoves();

            if (!n.IsLeaf()) {
                NNNode node = (NNNode)n;
                int Nb = node.Visits;
                int Na;
                float Qa;
                float Pa;
                Move? bestMove = null;
                float bestValue = -1f;
                foreach (Move m in moves) {
                    Na = node.GetVisitsOfMove(m);
                    Qa = Na == 0 ? 1 : node.GetValueOfMove(m) / Na;
                    Pa = node.GetProbabilityOfChild(m, calcBuffer);

                    float ucb = Qa + Pa * (rootLookupTable[Nb] / (1 + Na)) * logLookupTable[Na];
                    if (bestValue < ucb) {
                        bestMove = m;
                        bestValue = ucb;
                    }
                }

                if (bestMove is null)
                    throw new ArgumentException("Could not select a move to explore!");
                return (Move)bestMove;
            }
            // Return random if this node has no children yet.
            return moves[r.Next(moves.Count)];
        }

        public override IAgentParameters GetParameters() {
            return new pUCTAgentParameters();
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            pUCTAgentParameters parameters = (pUCTAgentParameters)agentParameters;
            name = parameters.ToString();
            c1 = parameters.C1;
            c2 = parameters.C2;
            logLookupTable = new float[rollouts];
            rootLookupTable = new float[rollouts];
            for(int i = 0; i < rollouts; i++) {
                logLookupTable[i] = c1 + (float)Math.Log((i + c2 + 1) / c2);
                rootLookupTable[i] = (float)Math.Sqrt(i);
            }
        }
    }
}
