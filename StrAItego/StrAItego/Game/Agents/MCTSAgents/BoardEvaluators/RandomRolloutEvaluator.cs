﻿using StrAItego.Game.Agents.RandomAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class RandomRolloutEvaluator : IBoardEvaluator
    {
        bool init = false;
        IAgent red, blue;

        public float EvaluateNode(Node n, Random r = null) {
            if (!init) {
                RandomAgentParameters redparams = new RandomAgentParameters();
                RandomAgentParameters blueparams = new RandomAgentParameters();

                redparams.SetSeed(r.Next());
                blueparams.SetSeed(r.Next());

                red = new RandomAgent.RandomAgent();
                blue = new RandomAgent.RandomAgent();

                red.SetParameters(redparams);
                blue.SetParameters(blueparams);

                redparams.Dispose();
                blueparams.Dispose();

                init = true;
            }
            
            Game g = new Game(red, blue, n.Board, n.TurnOfTeam);

            Team result = g.PlayGame(null, null, true, false);

            int movesMade = Math.Max(1, g.MovesMade);

            return result == Team.Red ? (0.8f + (0.2f / movesMade)) : (0.2f - (0.2f / movesMade));
        }

        public override string ToString() {
            return "Random Rollouts";
        }
    }
}