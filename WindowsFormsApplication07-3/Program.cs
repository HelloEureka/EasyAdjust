using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication07_3
{

    public class golobal_value
    {
        private static object lllll;
        private static object llll4;
        private static object lllll5;
        public static double[,] AC = new double[100, 1];    //AddConnection
        public static double[,] AP = new double[100, 2];    //AddPoint
        public static double[,] AAng = new double[100, 3];
        public static object l
        {
            get
            {
                return lllll;
            }
            set
            {
                lllll = value;
            }
        }
        public static object kp
        {
            get
            {
                return llll4;
            }
            set
            {
                llll4 = value;
            }
        }
        public static double AddCon
        {
            get
            {
                return AC[int.Parse(lllll.ToString()), 0];
            }
            set
            {
                AC[int.Parse(lllll.ToString()), 0] = value;
            }
        }
        public static double AddPointX
        {
            get
            {
                return AP[int.Parse(llll4.ToString()), 0];
            }
            set
            {
                AP[int.Parse(llll4.ToString()), 0] = value;
            }
        }
        public static double AddPointY
        {
            get
            {
                return AP[int.Parse(llll4.ToString()), 1];
            }
            set
            {
                AP[int.Parse(llll4.ToString()), 1] = value;
            }
        }

        public static object Ang
        {
            get
            {
                return lllll5;
            }

            set
            {
                lllll5 = value;
            }
        }
        public static double AddAngD
        {
            get
            {
                return AAng[int.Parse(lllll5.ToString()), 0];
            }
            set
            {
                AAng[int.Parse(lllll5.ToString()), 0] = value;
            }
        }
        public static double AddAngM
        {
            get
            {
                return AAng[int.Parse(lllll5.ToString()), 1];
            }
            set
            {
                AAng[int.Parse(lllll5.ToString()), 1] = value;
            }
        }
        public static double AddAngS
        {
            get
            {
                return AAng[int.Parse(lllll5.ToString()), 2];
            }
            set
            {
                AAng[int.Parse(lllll5.ToString()), 2] = value;
            }
        }

    }     
    static class Program
    {
       
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {         
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form f8 = new Form8();
            Application.Run(f8);
        }
    }
}
