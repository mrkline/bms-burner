using System;
using System.Windows.Forms;

namespace bms_burner
{
    public partial class wndAfterburner : Form
    {
        private readonly AfterburnerOverlay ABIndicator;

        static public AfterburnerOverlay ShowAfterburnerWindow(AfterburnerOverlay ABIndicator)
        {
            wndAfterburner ownWindow = new wndAfterburner(ABIndicator);
            ownWindow.ShowDialog();
            return ownWindow.ABIndicator;
        }

        public wndAfterburner(AfterburnerOverlay ABIndicator)
        {
            InitializeComponent();
            this.ABIndicator = ABIndicator;
            this.nudWidth.Text = ABIndicator.Width.ToString();
            this.nudHeight.Text = ABIndicator.Height.ToString();
            this.cmbLocation.SelectedIndex = cmbLocation.FindString(ABIndicator.getScreenLocation());
        }

        private void Save_Click(object sender, EventArgs e)
        {
            ABIndicator.Width = (int)nudWidth.Value;
            ABIndicator.Height = (int)nudHeight.Value;
            switch (this.cmbLocation.SelectedItem.ToString())
            {
                case "Top Left":
                    ABIndicator.ScreenLocation = ScreenLocation.Top_Left;
                    break;
                case "Top Right":
                    ABIndicator.ScreenLocation = ScreenLocation.Top_Right;
                    break;
                case "Bottom Left":
                    ABIndicator.ScreenLocation = ScreenLocation.Bottom_Left;
                    break;
                case "Bottom Right":
                    ABIndicator.ScreenLocation = ScreenLocation.Bottom_Right;
                    break;
            }

            ABIndicator.Save();
            this.Close();
        }
    }
}
