using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogManager
{
    public partial class IWASnotif : Form
    {
        public IWASnotif()
        {
            InitializeComponent();
        }
        int a = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            a++;
            if (a >= 5)
            {
                a = 0;
                this.Hide();
            }
        }

        private void IWASnotif_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height - 110);
        }
    }
}
