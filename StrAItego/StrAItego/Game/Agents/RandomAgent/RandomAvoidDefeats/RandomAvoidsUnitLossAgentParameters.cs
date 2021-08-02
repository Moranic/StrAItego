using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.RandomAgent.RandomAvoidDefeats
{
    class RandomAvoidsUnitLossAgentParameters : RandomAgentParameters
    {
        public override string ToString() {
            return "Random (Avoids Attacker Loss) Agent (seed: " + (randomSeed.Checked ? rseed : tseed) + ")";
        }
    }
}
