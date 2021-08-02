using StrAItego.Game.Agents.RandomAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.NaiveNNAgent
{
    class NaiveNNAgentParameters : RandomAgentParameters
    {
        public NaiveNNAgentParameters() : base() { }

        public override string ToString() {
            return "Naive NN Agent w." + setupProviderName;
        }
    }
}
