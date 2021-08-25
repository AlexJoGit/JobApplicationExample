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
using System.IO;

namespace MapEditor
{
    class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        Window window;

        ContextMenu cm = new ContextMenu();

        Panel pnl = new Panel()
        {
            Size = new Size(1280, 720),
            AutoSize = true,
            Location = new Point(100, 100),
            BorderStyle = BorderStyle.FixedSingle

        };

        Button btnStartSim = new Button()
        {
            Text = "StartSimulating",
            Location = new Point(0, 0),
            Size = new Size(300, 100)
        };

        Button btnAddNewGraphicsObject = new Button()
        {
            Text = "add new graphics object",
            Location = new Point(300, 0),
            Size = new Size(300, 100),
        };


        Slider selected = new Slider(new Point(0, 100), "Selected");

        Slider sizeX, sizeY, sizeZ;


        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);


            btnStartSim.Click += new EventHandler(btnStartSim_Click);
            Controls.Add(btnStartSim);

            pnl.Click += new EventHandler(Pnl_Click);
            Controls.Add(pnl);

            sizeX = new Slider(new Point(0, 200), "sizeX");
            sizeY = new Slider(new Point(0, 300), "sizeY");
            sizeZ = new Slider(new Point(0, 400), "sizeZ");

            sizeX.trckbr.ValueChanged += new EventHandler(trckbr_ValueChanged);
            Controls.Add(sizeX.trckbr);
            Controls.Add(sizeX.lbl);
            sizeY.trckbr.ValueChanged += new EventHandler(trckbr_ValueChanged);
            Controls.Add(sizeY.trckbr);
            Controls.Add(sizeY.lbl);
            sizeZ.trckbr.ValueChanged += new EventHandler(trckbr_ValueChanged);
            Controls.Add(sizeZ.trckbr);
            Controls.Add(sizeZ.lbl);
            selected.trckbr.ValueChanged += new EventHandler(trckbr_ValueChanged);
            Controls.Add(selected.trckbr);
            Controls.Add(selected.lbl);

            btnAddNewGraphicsObject.Click += new EventHandler(AddNewGraphicsObject);
            Controls.Add(btnAddNewGraphicsObject);

            cm.MenuItems.Add("Item 1");
            cm.MenuItems.Add("Item 2");



            pnl.ContextMenu = cm;


            FormClosed += new FormClosedEventHandler(Form1_FormClosed);

        }


        private void trckbr_ValueChanged(object sender, EventArgs e)
        {
            window.ChangeValueX(sizeX.value);
            window.ChangeValueY(sizeY.value);
            window.ChangeValueZ(sizeZ.value);

            window.selectedID = (int)selected.value / 10;
        }

        private void Window_TitleChanged(object sender, EventArgs e)
        {
            btnStartSim.Text = window.Title;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(window != null)
            {
                window.Exit();
            }
           
        }

        private void Pnl_Click(object sender, EventArgs e)
        {
            //Panel panel = sender as Panel;
            //ActiveControl = panel;

        }

        void btnStartSim_Click(object sender, EventArgs e)
        {
            StartSimulation();
        }

        void StartSimulation()
        {
            if(window != null)
            {
                window.unloadd();
                window.Exit();
                window.Dispose();
                window = null;
            }

            if(window == null)
            {
                window = new Window(1280, 720, "LadderGame")
                {
                    Location = new Point(0, 0),
                    //WindowBorder = WindowBorder.Hidden,
                };

                window.TitleChanged += new EventHandler<EventArgs>(Window_TitleChanged);
                window.MouseDown += new EventHandler<MouseButtonEventArgs>(windowMouseDown);

                window.WindowState = OpenTK.WindowState.Fullscreen;

                SetParent(window.WindowInfo.Handle, pnl.Handle);

                window.Run(60.0);
            }
        }

        

        private void windowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.Button == MouseButton.Right)
            {
                cm.Show(pnl, e.Position);
            }
            
        }

        private void AddNewGraphicsObject(object sender, EventArgs e)
        {
            frmNewGraphic frm = new frmNewGraphic();
            frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
