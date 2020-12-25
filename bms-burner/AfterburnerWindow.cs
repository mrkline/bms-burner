using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bms_burner
{
    public partial class AfterburnerWindow : Form
    {
        private AfterburnerIndicator ABIndicator;

        static public AfterburnerIndicator ShowAfterburnerWindow(AfterburnerIndicator ABIndicator)
        {
            AfterburnerWindow ownWindow = new AfterburnerWindow(ABIndicator);
            ownWindow.ShowDialog();
            return ownWindow.ABIndicator;
        }
        public AfterburnerWindow(AfterburnerIndicator ABIndicator)
        {
            InitializeComponent();
            this.ABIndicator = ABIndicator;
            this.enabledCheckBox.Checked = ABIndicator.isEnabled;
            this.widthTextBox.Text = ABIndicator.width.ToString();
            this.heightTextBox.Text = ABIndicator.height.ToString();
            this.comboBox1.SelectedIndex = comboBox1.FindString(ABIndicator.getScreenLocation());
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Save_Click(object sender, EventArgs e)
        {
            ABIndicator.isEnabled = enabledCheckBox.Checked;
            ABIndicator.width = (int)widthTextBox.Value;
            ABIndicator.height = (int)heightTextBox.Value;
            switch (this.comboBox1.SelectedItem.ToString())
            {
                case "Top Left":
                    ABIndicator.scrLoc = ScreenLocation.Top_Left;
                    break;
                case "Top Right":
                    ABIndicator.scrLoc = ScreenLocation.Top_Right;
                    break;
                case "Bottom Left":
                    ABIndicator.scrLoc = ScreenLocation.Bottom_Left;
                    break;
                case "Bottom Right":
                    ABIndicator.scrLoc = ScreenLocation.Bottom_Right;
                    break;
            }
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AfterburnerIndicator));
            StreamWriter sw = new StreamWriter("ABWindow.xml", false, new System.Text.UTF8Encoding(false));
            serializer.Serialize(sw, ABIndicator);
            sw.Close();
            this.Close();
        }
    }
}
