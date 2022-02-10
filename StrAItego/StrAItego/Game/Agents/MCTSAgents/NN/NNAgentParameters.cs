namespace StrAItego.Game.Agents.MCTSAgents.NN
{
    class NNAgentParameters : MCTSAgentParameters
    {
        public NNAgentParameters() : base() { }

        public override string ToString() {
            return "NN Agent (est: " + estimatorName + " (" + Estimations + "), eval: " + evaluatorName + " (" + Rollouts + "), seed: " + (randomSeed.Checked ? rseed : tseed) + ") w. " + setupProviderName;
        }
    }
}
