using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
