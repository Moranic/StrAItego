using StrAItego.Game.Agents.SetupProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.RandomAgent
{
    class RandomAgentParameters : IAgentParameters
    {
        protected int rseed, tseed, increment = 0;
        protected Label seedLabel;
        protected CheckBox randomSeed;
        protected TextBox seed;
        protected ToolTip seedToolTip;

        protected Panel panel;

        Label setupProviderLabel;
        ComboBox setupProviderBox;
        Type selectedSetupProvider;
        protected string setupProviderName;

        static Dictionary<string, Type> setupProviderTypes = new Dictionary<string, Type>();

        static RandomAgentParameters () {
            List<Type> setupProviders = new List<Type>();
            Type ti = typeof(ISetupProvider);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in asm.GetTypes()) {
                    if (ti.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract) {
                        setupProviders.Add(t);
                    }
                }
            }

            foreach (Type t in setupProviders) {
                ISetupProvider sp = (ISetupProvider)Activator.CreateInstance(t);
                setupProviderTypes.Add(sp.ToString(), t);
            }
        }

        public RandomAgentParameters() {
            rseed = (new Random()).Next();
            tseed = 0;
            randomSeed = new CheckBox {
                Checked = false
            };
            randomSeed.CheckedChanged += OnCheckedChanged;
            randomSeed.AutoSize = true;
            randomSeed.Text = "Use random seed?";
            seed = new TextBox();
            seedLabel = new Label {
                Text = "Use seed:"
            };
            seed.TextChanged += OnTextChanged;
            seedToolTip = new ToolTip();
            seedToolTip.SetToolTip(seedLabel, "Click to copy the seed.");
            seedLabel.Click += OnSeedLabelClicked;

            setupProviderLabel = new Label();
            setupProviderLabel.Text = "Select board setup provider:";

            setupProviderBox = new ComboBox();
            setupProviderBox.Sorted = true;
            setupProviderBox.Items.AddRange(setupProviderTypes.Keys.ToArray());
            setupProviderBox.SelectedValueChanged += OnSetupProviderChanged;


            randomSeed.Dock = DockStyle.Top;
            seedLabel.Dock = DockStyle.Top;
            seed.Dock = DockStyle.Top;
            setupProviderBox.Dock = DockStyle.Top;
            setupProviderLabel.Dock = DockStyle.Top;
            
            panel = new Panel();
            panel.Padding = new Padding(4);
            panel.Controls.Add(setupProviderBox);
            panel.Controls.Add(setupProviderLabel);
            panel.Controls.Add(seed);
            panel.Controls.Add(seedLabel);
            panel.Controls.Add(randomSeed);


            panel.Name = "ParametersPanel";
            panel.Tag = this;

            randomSeed.Checked = true;
        }

        public int GetSeed() {
            if (randomSeed.Checked)
                return rseed + increment++;
            return tseed + increment++;
        }

        public void SetSeed(int seed) {
            rseed = seed;
        }

        public Panel GetControls() {
            return panel;
        }

        public virtual bool IsValid() {
            if (selectedSetupProvider != null) {
                if (randomSeed.Checked)
                    return true;
                return Int32.TryParse(seed.Text, out _);
            }
            return false;
        }

        void OnTextChanged(object sender, EventArgs e) {
            if (Int32.TryParse(seed.Text, out _))
                Int32.TryParse(seed.Text, out tseed);
        }

        protected void OnCheckedChanged(object sender, EventArgs e) {
            seed.Visible = !randomSeed.Checked;
            seedToolTip.Active = randomSeed.Checked;
            rseed = (new Random()).Next();
            seedLabel.Text = "Use seed: ";
            if (randomSeed.Checked)
                seedLabel.Text += rseed;

            panel.Controls.SetChildIndex(setupProviderLabel, 1);
            panel.Controls.SetChildIndex(setupProviderBox, 0);
            panel.Controls.SetChildIndex(seed, 2);
            panel.Controls.SetChildIndex(seedLabel, 3);
            panel.Controls.SetChildIndex(randomSeed, 4);
        }

        public void ResetRandom() {
            increment = 0;
        }

        void OnSeedLabelClicked(object sender, EventArgs e) {
            if(randomSeed.Checked)
                Clipboard.SetText(rseed.ToString());
        }

        void OnSetupProviderChanged(object sender, EventArgs e) {
            selectedSetupProvider = setupProviderTypes[(string)setupProviderBox.SelectedItem];
            setupProviderName = (string)setupProviderBox.SelectedItem;
        }

        public ISetupProvider GetSetupProvider {
            get { return selectedSetupProvider is null ? null : (ISetupProvider)Activator.CreateInstance(selectedSetupProvider); }
        }

        public override string ToString() {
            return "Random Agent (seed: " + (randomSeed.Checked ? rseed : tseed) + ") w. " + setupProviderName;
        }

        public void Dispose() {
            for (int i = panel.Controls.Count - 1; i >= 0; i--)
                panel.Controls[i].Dispose();
            panel.Dispose();
        }
    }
}
