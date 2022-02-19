using System;

namespace StrAItego.Game.Agents.SetupProviders.GravonSetups
{
    partial class GravonSetup : ISetupProvider
    {

        public Rank[] GetSetup(Random r) {
            int i = r.Next(102676);
            string setupdata = data.Substring(40 * i, 40);
            Rank[] setup = new Rank[40];
            for (int j = 0; j < 40; j++)
                setup[j] = CharToRank(setupdata[j]);
            return setup;
        }

        public void Dispose() {

        }

        public override string ToString() {
            return "Gravon Setup";
        }
        static Rank CharToRank(char c) {
            return c switch {
                'B' => Rank.Bomb,
                'F' => Rank.Flag,
                '1' => Rank.Spy,
                '2' => Rank.Scout,
                '3' => Rank.Miner,
                '4' => Rank.Sergeant,
                '5' => Rank.Lieutenant,
                '6' => Rank.Captain,
                '7' => Rank.Major,
                '8' => Rank.Colonel,
                '9' => Rank.General,
                'M' => Rank.Marshal,
                _ => throw new ArgumentException("Unknown character! " + c)
            };
        }
    }
}
