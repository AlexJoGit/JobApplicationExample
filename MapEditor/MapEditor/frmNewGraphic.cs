using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Input;
using OpenTK;
using System.Drawing;

namespace MapEditor
{
    class frmNewGraphic : Form
    {
        Button test = new Button()
        {
            Text = "test",
            Location = new Point(300, 0),
            Size = new Size(300, 100),
        };

        public frmNewGraphic()
        {
            Controls.Add(test);
            MouseLeave += new EventHandler(OnLost);
        }

        public void OnLost(object sender, EventArgs e)
        {
            Close();
        }

    }
}
