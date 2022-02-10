using System;

namespace StrAItego.Game.Agents.SetupProviders
{


    class AccoladeSetup : ISetupProvider
    {
        const string data = 
            // Cyclone defense
            "74B4225252" +
            "1BFB395246" +
            "M6B38633B4" +
            "273265728B" +
            // Tempest defense
            "BFB4253237" +
            "7B5B63B154" +
            "B2238653M2" +
            "2796246284" +
            // Triple threat
            "BFBBB252B3" +
            "5B72337643" +
            "465226M174" +
            "2986253824" +
            // Scout's gambit
            "FB43235252" +
            "B634B7513B" +
            "7B482MB296" +
            "2584263672" +
            // On guard
            "4BFB224462" +
            "97B3536523" +
            "25813B5BB8" +
            "243M726672" +
            // Shoreline bluff
            "2346223B62" +
            "175B53B576" +
            "M484362435" +
            "2BFB927B82" +
            // Corner fortress
            "F4B734635B" +
            "4B85B61B23" +
            "B276223M37" +
            "6295282452" +
            // Shield defense
            "363BFB2734" +
            "6312B4345B" +
            "7846287B26" +
            "52M22B5925" +
            // Corner blitz
            "FB2B34B2B3" +
            "B747134563" +
            "52687223BM" +
            "2596256842" +
            // Wheel of danger
            "4243B78123" +
            "68MBFB6346" +
            "5B45B56B57" +
            "2237229232" +
            // Blitzkrieg
            "2BFB632544" +
            "72B2734B15" +
            "5332B2627B" +
            "2864M96583" +
            // Early warning
            "24BFB35663" +
            "6B5B342635" +
            "7B17487932" +
            "24M22B2852" +
            // Man the barricades
            "FM24232832" +
            "4463543623" +
            "BB57BB71BB" +
            "7569228526";

        public Rank[] GetSetup(Random r) {
            int i = r.Next(13);
            string setupdata = data.Substring(40 * i, 40);
            Rank[] setup = new Rank[40];
            for (int j = 0; j < 40; j++)
                setup[j] = CharToUnit(setupdata[j]);
            return setup;
        }

        public override string ToString() {
            return "Accolade Setup";
        }
        static Rank CharToUnit(char c) {
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
