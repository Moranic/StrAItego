using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.UI
{
    public class RunResults
    {
        Dictionary<(string, string), (int, int)> results = new Dictionary<(string, string), (int, int)>();
        public RunResults(Game.Game[] games) {
            foreach(Game.Game g in games) {
                string red = g.RedPlayer.ToString();
                string blue = g.BluePlayer.ToString();
                (string, string) matchup = (red, blue);
                int addred = g.GetWinner == Game.Team.Red ? 1 : 0;
                int addblue = g.GetWinner == Game.Team.Blue ? 1 : 0;
                if (!results.ContainsKey(matchup)) {
                    results.Add(matchup, (addred, addblue));
                }
                else {
                    (int, int) prevresult = results[matchup];
                    results[matchup] = (prevresult.Item1 + addred, prevresult.Item2 + addblue);
                }
            }
        }

        public Dictionary<(string, string), (int, int)> Results {
            get { return results; }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Red;Blue;Win;Loss");
            foreach (KeyValuePair<(string, string), (int, int)> x in results)
                sb.AppendLine(x.Key.Item1 + ";" + x.Key.Item2 + ";" + x.Value.Item1 + ";" + x.Value.Item2);
            return sb.ToString();
        }
    }
}
