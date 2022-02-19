using System;

namespace StrAItego.Game.Agents
{
    /// <summary>
    /// Base class for all Agent types. Inheriting from this class will also automatigically add it to the UI for selection.
    /// </summary>
    public abstract  class BaseAgent : IAgent, IDisposable
    {
        /// <summary>
        /// Visible name of the Agent.
        /// </summary>
        protected string name = "Unnamed Agent";

        /// <summary>
        /// Base class for all Agent types. Inheriting from this class will also automatigically add it to the UI for selection.
        /// </summary>
        /// <param name="name">Visible name of the agent.</param>
        public BaseAgent(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Dispose of any resources the Agent might use here.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Return the next move to make, as determined by the Red player.
        /// </summary>
        /// <param name="board">The board to make a move on.</param>
        /// <param name="gameLogger">Logging object to write debug information to.</param>
        /// <returns>The next Red move.</returns>
        public abstract Move? GetMove(Board board, GameLogger gameLogger);

        /// <summary>
        /// Get a new instance of an IAgentParameters object for this Agent type.
        /// </summary>
        /// <returns>An instance of IAgentParameters for this Agent type</returns>
        public abstract IAgentParameters GetParameters();

        /// <summary>
        /// Returns a starting setup for this Agent to use.
        /// </summary>
        /// <param name="board">A board to apply the setup to, only used for the HumanAgent.</param>
        /// <returns>A starting setup, defined by a Rank[40] starting with the bottom-left and ending with the top-right (as seen from Red).</returns>
        public abstract Rank[] GetSetup(Board board);

        /// <summary>
        /// Determines if this Agent is an AI.
        /// </summary>
        /// <returns>True if this Agent type is AI controlled, False if it is Human controlled.</returns>
        public virtual bool IsAI() => true;

        /// <summary>
        /// Sets the parameters for this Agent from an IAgentParameters object.
        /// </summary>
        /// <param name="agentParameters">The IAgentParameters object to take parameters from.</param>
        public abstract void SetParameters(IAgentParameters agentParameters);

        /// <summary>
        /// Provides a string representation of this Agent.
        /// </summary>
        /// <returns>A string representation of this Agent.</returns>
        public override string ToString() => name;
    }
}
