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
    public partial class Form7 : Form
    {
        
        public Form7()
        {
            InitializeComponent();
        }

       public void button1_Click(object sender, EventArgs e)
        {
            golobal_value.AddCon = double.Parse(textBox1.Text);            
            this.Close();
        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }
    }
}
    