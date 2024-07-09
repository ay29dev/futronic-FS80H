using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool x2 = false;
            IntPtr x = IntPtr.Zero;
            Futronic f = new Futronic();
            //if (x == device)
            //{
            //    x2 = true;
            //    device = ftrScanOpenDevice();
            //    Bitmap b = ExportBitMap();
            //}
            x2 = f.Init();
            if (x2 == true)
            {
                var v = f.ExportBitMap();
                pictureBox1.Image = v;
                var b = f.IsFinger();
            }
        }
    }
}
