using System;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.MCTSAgents.UCB
{
    class UCBAgentParameters : MCTSAgentParameters
    {
        float c;
        NumericUpDown cValue;
        Label epsilonLabel;

        public UCBAgentParameters() : base() {
            cValue = new NumericUpDown();
            cValue.ValueChanged += OnConfidenceChange;
            cValue.Minimum = 0M;
            cValue.Maximum = 1M;
            cValue.Increment = 0.05M;
            cValue.Value = 0.05M;
            cValue.DecimalPlaces = 2;
            cValue.RightToLeft = RightToLeft.Yes;
            cValue.UpDownAlign = LeftRightAlignment.Left;

            epsilonLabel = new Label() {
                Text = "Confidence value (c):"
            };

            cValue.Dock = DockStyle.Top;
            epsilonLabel.Dock = DockStyle.Top;

            panel.Controls.Add(cValue);
            panel.Controls.Add(epsilonLabel);
        }

        public override string ToString() {
            return "UCB Agent (est: " + estimatorName + " (" + Estimations + "), eval: " + evaluatorName + " (" + Rollouts + "), c: " + c + ", seed: " + (randomSeed.Checked ? rseed : tseed) + ") w. " + setupProviderName;
        }

        void OnConfidenceChange(object sender, EventArgs e) {
            c = (float)cValue.Value;
        }

        public float Confidence {
            get { return c; }
        }
    }
}
