using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication07_3
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            golobal_value.AddPointX = double.Parse(textBox1.Text);
            golobal_value.AddPointY = double.Parse(textBox2.Text);
            this.Close();
        }
    }
}
