using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents
{
    public abstract  class BaseAgent : IAgent
    {
        protected string name = "Unnamed Agent";

        public BaseAgent(string name)
        {
            this.name = name;
        }

        public abstract void Dispose();

        public abstract Move? GetMove(Board board, GameLogger gameLogger);

        public abstract IAgentParameters GetParameters();

        public abstract Rank[] GetSetup(Board board);

        public virtual bool IsAI() => true;

        public abstract void SetParameters(IAgentParameters agentParameters);

        public override string ToString() => name;
    }
}
