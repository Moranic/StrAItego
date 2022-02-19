using System;

namespace StrAItego.Game.Agents.SetupProviders
{


    class VincentdeBoerSetup : ISetupProvider
    {
        const string data =
            // Setup 1 (famous, of dubious quality, predictable, all-rounder)
            "23B23BFB33" +
            "4B4785B564" +
            "54B1927782" +
            "6225263M26" +
            // Setup 2 (all-rounder, difficult flag defense)
            "3B4B4233BF" +
            "4813B2655B" +
            "5275B27783" +
            "6249622M26" +
            // Setup 3 (aggresive, fast, less predictable, works against strong players)
            "73334BFBB3" +
            "727165B452" +
            "4288924BB5" +
            "M653262226" +
            // Setup 4 (good all-rounder)
            "32334BFB32" +
            "647175B4B4" +
            "M27826B5B5" +
            "2852629326" +
            // Setup 5 (flag in position against strong players, often changes pieces)
            "423B433BFB" +
            "B6175265B4" +
            "3287B5M758" +
            "9624222362" +
            // Setup 6 (old setup, weak defensively, good when unexpected)
            "3B423B33BF" +
            "54B273565B" +
            "72B1842784" +
            "2659262M62";

        public Rank[] GetSetup(Random r) {
            int i = r.Next(6);
            string setupdata = data.Substring(40 * i, 40);
            Rank[] setup = new Rank[40];
            for (int j = 0; j < 40; j++)
                setup[j] = CharToRank(setupdata[j]);
            return setup;
        }

        public override string ToString() {
            return "Vincent de Boer Setup";
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

        public void Dispose() {
        }
    }
}
