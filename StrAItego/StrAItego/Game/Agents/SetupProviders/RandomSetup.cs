using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.SetupProviders
{
    class RandomSetup : ISetupProvider
    {
        public void Dispose() {
        }

        public Rank[] GetSetup(Random r) {
            Rank[] units = {
            Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb,
            Rank.Marshal,
            Rank.General,
            Rank.Colonel, Rank.Colonel,
            Rank.Major, Rank.Major, Rank.Major,
            Rank.Captain, Rank.Captain, Rank.Captain, Rank.Captain,
            Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant,
            Rank.Sergeant, Rank.Sergeant, Rank.Sergeant, Rank.Sergeant,
            Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner,
            Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout,
            Rank.Spy,
            Rank.Flag
            };

            return units.OrderBy(x => r.Next()).ToArray();
        }

        public override string ToString() {
            return "Random Setup";
        }
    }
}
