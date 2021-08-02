using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.MCTSAgents.eGreedy
{
    class eGreedyAgentParameters : MCTSAgentParameters
    {
        float e;
        NumericUpDown epsilon;
        Label epsilonLabel;

        public eGreedyAgentParameters() : base() {
            epsilon = new NumericUpDown();
            epsilon.ValueChanged += OnEpsilonChange;
            epsilon.Minimum = 0M;
            epsilon.Maximum = 1M;
            epsilon.Increment = 0.05M;
            epsilon.Value = 0.8M;
            epsilon.DecimalPlaces = 2;
            epsilon.RightToLeft = RightToLeft.Yes;
            epsilon.UpDownAlign = LeftRightAlignment.Left;

            epsilonLabel = new Label() { 
                Text = "Epsilon value (e):"
            };

            epsilon.Dock = DockStyle.Top;
            epsilonLabel.Dock = DockStyle.Top;

            panel.Controls.Add(epsilon);
            panel.Controls.Add(epsilonLabel);
        }

        public override string ToString() {
            return "eGreedy Agent (est: " + estimatorName + " (" + Estimations + "), eval: " + evaluatorName + " (" + Rollouts + "), e: " + e + ", seed: " + (randomSeed.Checked ? rseed : tseed) + ") w. " + setupProviderName;
        }

        void OnEpsilonChange(object sender, EventArgs e) {
            this.e = (float)epsilon.Value;
        }

        public float Epsilon {
            get { return e; }
        }
    }
}
