using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RunPL
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
                        ControlPaint.DrawBorder(e.Graphics,
                                this.ClientRectangle,
                                Color.DeepSkyBlue,
                                1,
                                ButtonBorderStyle.Solid,
                                Color.DeepSkyBlue,
                                1,
                                ButtonBorderStyle.Solid,
                                Color.DeepSkyBlue,
                                1,
                                ButtonBorderStyle.Solid,
                                Color.DeepSkyBlue,
                                1,
                                ButtonBorderStyle.Solid); 
        }

        public void button1_Click(object sender, EventArgs e)
        {
            this.Owner.Close();
            this.Close();
           
        }
        
    }
}
