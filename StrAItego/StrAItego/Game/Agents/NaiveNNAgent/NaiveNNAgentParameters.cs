using StrAItego.Game.Agents.RandomAgent;

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
