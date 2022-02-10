using System;
using System.Windows.Forms;

namespace StrAItego.Game.Agents
{
    public interface IAgentParameters : IDisposable
    {

        Panel GetControls();

        bool IsValid();

        void ResetRandom();
    }
}
