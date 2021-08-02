using StrAItego.Game.Agents.RandomAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.PeterNLewisAgent
{
    class PeterNLewisAgentParameters : RandomAgentParameters
    {
        public PeterNLewisAgentParameters() : base(){ }

        public override string ToString() {
            return "Peter N. Lewis Agent w." + setupProviderName;
        }
    }
}
