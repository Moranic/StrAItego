using System;

namespace StrAItego.Game.Agents.SetupProviders
{
    interface ISetupProvider : IDisposable
    {
        public Rank[] GetSetup(Random r);
    }
}
