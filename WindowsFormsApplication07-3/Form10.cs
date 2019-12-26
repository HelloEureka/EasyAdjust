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
    public partial class Form10 : Form
    {
        public Form10()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            golobal_value.AddAngD = double.Parse(textBox1.Text);
            golobal_value.AddAngM = double.Parse(textBox2.Text);
            golobal_value.AddAngS = double.Parse(textBox3.Text);
            this.Close();
        }
    }
}
