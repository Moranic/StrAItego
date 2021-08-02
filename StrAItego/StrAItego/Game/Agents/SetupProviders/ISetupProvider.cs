using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.SetupProviders
{
    interface ISetupProvider : IDisposable
    {
        public Rank[] GetSetup(Random r);
    }
}
