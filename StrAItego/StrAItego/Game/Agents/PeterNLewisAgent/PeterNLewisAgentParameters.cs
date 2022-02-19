using StrAItego.Game.Agents.RandomAgent;

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
