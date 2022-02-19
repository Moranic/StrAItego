namespace StrAItego.Game.Agents.RandomAgent.RandomAvoidDefeats
{
    class RandomAvoidsUnitLossAgentParameters : RandomAgentParameters
    {
        public override string ToString() {
            return "Random (Avoids Attacker Loss) Agent (seed: " + (randomSeed.Checked ? rseed : tseed) + ")";
        }
    }
}
