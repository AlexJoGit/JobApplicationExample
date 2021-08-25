using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            Form1 Tools = new Form1()
            {
                Size = Screen.PrimaryScreen.WorkingArea.Size,

            };

            Application.Run(Tools);
        }
    }
}
