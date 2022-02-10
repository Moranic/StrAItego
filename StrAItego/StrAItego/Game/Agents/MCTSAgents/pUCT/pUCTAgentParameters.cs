using StrAItego.Game.Agents.MCTSAgents.NN;
using System;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.MCTSAgents.pUCT
{
    class pUCTAgentParameters : NNAgentParameters
    {
        float c1;
        NumericUpDown c1Value;
        Label c1Label;

        float c2;
        NumericUpDown c2Value;
        Label c2Label;

        public pUCTAgentParameters() : base() {
            c1Value = new NumericUpDown();
            c1Value.ValueChanged += OnC1Change;
            c1Value.Minimum = 0M;
            c1Value.Maximum = 10M;
            c1Value.Increment = 0.05M;
            c1Value.Value = 1.15M;
            c1Value.DecimalPlaces = 2;
            c1Value.RightToLeft = RightToLeft.Yes;
            c1Value.UpDownAlign = LeftRightAlignment.Left;

            c1Label = new Label() {
                Text = "C1 value:"
            };

            c1Value.Dock = DockStyle.Top;
            c1Label.Dock = DockStyle.Top;

            panel.Controls.Add(c1Value);
            panel.Controls.Add(c1Label);

            c2Value = new NumericUpDown();
            c2Value.ValueChanged += OnC2Change;
            c2Value.Minimum = 0M;
            c2Value.Maximum = 1000000M;
            c2Value.Increment = 1000M;
            c2Value.Value = 19000M;
            c2Value.DecimalPlaces = 0;
            c2Value.RightToLeft = RightToLeft.Yes;
            c2Value.UpDownAlign = LeftRightAlignment.Left;

            c2Label = new Label() {
                Text = "C2 value:"
            };

            c2Value.Dock = DockStyle.Top;
            c2Label.Dock = DockStyle.Top;

            panel.Controls.Add(c2Value);
            panel.Controls.Add(c2Label);
        }

        public override string ToString() {
            return "pUCT Agent (est: " + estimatorName + " (" + Estimations + "), eval: " + evaluatorName + " (" + Rollouts + "), c1: " + c1 +", c2: " + c2 + ", seed: " + (randomSeed.Checked ? rseed : tseed) + ") w. " + setupProviderName;
        }

        void OnC1Change(object sender, EventArgs e) {
            c1 = (float)c1Value.Value;
        }

        public float C1 {
            get { return c1; }
        }

        void OnC2Change(object sender, EventArgs e) {
            c2 = (float)c2Value.Value;
        }

        public float C2 {
            get { return c2; }
        }
    }
}
