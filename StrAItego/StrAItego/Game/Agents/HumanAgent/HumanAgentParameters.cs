using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.HumanAgent
{
    class HumanAgentParameters : IAgentParameters
    {
        public Team AsTeam { get; set; }
        public UI.Board Board { get; set; }
        Label noParams, playingAs;
        Panel panel;

        public HumanAgentParameters() {
            noParams = new Label {
                Text = "No parameters required."
            };
            playingAs = new Label();

            playingAs.Dock = DockStyle.Top;
            noParams.Dock = DockStyle.Top;

            panel = new Panel();
            panel.Padding = new Padding(4);
            panel.Controls.Add(playingAs);
            panel.Controls.Add(noParams);
            panel.ParentChanged += SetTeam;
            panel.Tag = this;
            panel.Name = "ParametersPanel";
        }

        public Panel GetControls() {
            return panel;
        }

        public bool IsValid() {
            return true;
        }

        void SetTeam(object sender, EventArgs e) {
            if (panel.Parent is null)
                return;
            AsTeam = (Team)panel.Parent.Parent.Tag;
            playingAs.Text = "A human will control " + AsTeam.ToString() + ".";
            Board = ((SplitContainer)panel.Parent.Parent.Parent.Parent.Parent.Parent.Parent).Panel1.Tag as UI.Board;
        }

        public void ResetRandom() {
            return;
        }

        public override string ToString() {
            return "Human Agent (Team: " + AsTeam + ")";
        }

        public void Dispose() {
            for (int i = panel.Controls.Count - 1; i >= 0; i--)
                panel.Controls[i].Dispose();
            panel.Dispose();
        }
    }
}
