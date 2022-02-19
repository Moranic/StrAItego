using System;

namespace StrAItego.Game.Agents.SetupProviders
{
    class PeterNLewisSetup : ISetupProvider
    {
        public void Dispose() {
        }

        public Rank[] GetSetup(Random r) {
            //"8;<;67;7;7"
            //"48;3862;89"
            //"6359954865"
            //"997159:499"
            //1 = Marshal, 2 = General, 3 = Colonel, 4 = Major, 5 = Captain, 6 = Lieutenant, 7 = Sergeant, 8 = Miner, 9 = Scout, : = Spy, ; = Bomb, < = Flag
            Rank[] units = {
            Rank.Miner, Rank.Bomb, Rank.Flag, Rank.Bomb, Rank.Lieutenant, Rank.Sergeant, Rank.Bomb, Rank.Sergeant, Rank.Bomb, Rank.Sergeant,
            Rank.Major, Rank.Miner, Rank.Bomb, Rank.Colonel, Rank.Miner, Rank.Lieutenant, Rank.General, Rank.Bomb, Rank.Miner, Rank.Scout,
            Rank.Lieutenant, Rank.Colonel, Rank.Captain, Rank.Scout, Rank.Scout, Rank.Captain, Rank.Major, Rank.Miner, Rank.Lieutenant, Rank.Captain,
            Rank.Scout, Rank.Scout, Rank.Sergeant, Rank.Marshal, Rank.Captain, Rank.Scout, Rank.Spy, Rank.Major, Rank.Scout, Rank.Scout
            };

            return units;
        }

        public override string ToString() {
            return "Peter N. Lewis Setup";
        }
    }
}
