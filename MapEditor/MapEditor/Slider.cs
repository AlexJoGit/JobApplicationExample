using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MapEditor
{
    class Slider
    {
        public Label lbl = new Label();
        public TrackBar trckbr = new TrackBar();

        public float value { get => trckbr.Value; set => trckbr.Value = (int)value; }
        public string text;

        public Slider(Point location, string txt)
        {
            text = txt;
            lbl.Text = text + ": " + value;
            lbl.Location = location;
            trckbr.Location = new Point(location.X, location.Y + 30);

            trckbr.Maximum = 255;
            trckbr.Minimum = 1;

            trckbr.ValueChanged += new EventHandler(trckbrValueChanged);

        }

        private void trckbrValueChanged(object sender, EventArgs e)
        {
            lbl.Text = text + ": " + value;
        }
    }
}
