using AccessDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using INetWork;
using IMath;
using Adjustment;

namespace WindowsFormsApplication07_3
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }
        
        int i = 0;                      //记录点的点击次数
        int j = 0;                      //记录线的点击次数
        int ang = 0;                    //记录角的点击次数
        int c = 0;                      //记录已知点的个数
        int[] RecNum = new int[100];    //记录已知的点在AllPoint中的位置
        Graphics gra;                 //画布
        Point[] AllPoint   = new Point[100];
        private Pen pen;
        int[,] Con = new int[100, 2];//相连线的两个点号
        int[,] ConAng = new int[100, 3];//相连的角的3个点号
        int[] re = new int[2]; //线的中间信息     
        int[] re2 = new int[3];//角的中间信息 

        private void Form8_MouseDown(object sender, MouseEventArgs e)
        {            
        }
        
        private void Form8_Load(object sender, EventArgs e)
        {
            gra = pictureBox1.CreateGraphics();
            // 画框架                       
            listBox3.Items.Add("点 x, y");
            listBox1.Items.Add("边 距离");
            listBox2.Items.Add("  角  °  ′  ″");
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // 画框架
            

            if (radioButton1.Checked||radioButton2.Checked)
            {                
                // 画图
                Point p1 = new Point(e.X - 20, e.Y - 20);
                //PictureBox pbox = new PictureBox();
                //pbox.Location = p1;
                if (radioButton1.Checked)
                {
                   pen = new Pen(Color.Blue, 1);
                    // 记录点
                    AllPoint[i].X = e.X;
                    AllPoint[i].Y = e.Y;
                    RecNum[c] = i;
                    c = c + 1;
                    string str = i.ToString();
                    Brush B =  Brushes.Blue; 
                    gra.DrawString(str, new Font("宋体", 15),B,e.X,e.Y);


                }
                else
                {                   
                    pen = new Pen(Color.Black, 1);
                    // 记录点
                    string str = i.ToString();
                    Brush B = Brushes.Black;
                    gra.DrawString(str, new Font("宋体", 15), B, e.X, e.Y);
                    AllPoint[i].X = e.X;
                    AllPoint[i].Y = e.Y;
                }

                if (e.Button == MouseButtons.Left)
                {
                   
                    Rectangle rect = new Rectangle(e.X-5, e.Y-5, 10 , 10);
                    

                    gra.DrawEllipse(pen, rect);
                  
                   // pictureBox1.Controls.Add(pbox);                 
                    i = i + 1;
                   
                }

            }
            else if (radioButton3.Checked)
            {
               
                if (JudgeP(e.X, e.Y) != -1)
                {

                    re[j % 2] = JudgeP(e.X, e.Y);
                    
                    //如果为偶数次点击连接点
                    if (j % 2 == 1)
                    {
                        gra.DrawLine(Pens.Black, AllPoint[re[j % 2]], AllPoint[re[j % 2 - 1]]);

                        Con[(j + 1) / 2 - 1,0] = re[j % 2 - 1];
                        Con[(j + 1) / 2 - 1, 1] = re[j % 2];
                        //Angle[re[j % 2],re[j % 2 - 1]] = 1;
                        //S[re[j % 2], re[j % 2 - 1]] = 1;
                        string str1 = Con[(j + 1) / 2 - 1, 0].ToString();
                        string str2 = Con[(j + 1) / 2 - 1, 1].ToString();
                        // MessageBox.Show(str1+","+str2+" connected");
                    }

                    j++;

                }

            }
            else if(radioButton4.Checked)
            {

                //string str = JudgeL(e.X, e.Y).ToString();
                //MessageBox.Show(str);

                if (JudgeL(e.X, e.Y) != -1)
                {
                    int lllll= JudgeL(e.X, e.Y);
                    string str1 = Con[lllll, 0].ToString();
                    string str2 = Con[lllll, 1].ToString();
               
                    gra.DrawLine(Pens.Red, AllPoint[Con[lllll, 0]], AllPoint[Con[lllll, 1]]);

                    golobal_value.l = lllll;
                    Form7 f7 = new Form7();
                    f7.Text = "请输入" + str1 + "," + str2 + "的距离";
                    f7.Show();

                    
                }
            }
            else if(radioButton5.Checked)
            {
               // RecNum[0-c]中放的已知点在AllPoint中的位置，
               if( JudgePKnow(e.X, e.Y)!=-1)
                {
                    int kc = JudgePKnow(e.X, e.Y);
                    golobal_value.kp = kc;
                    Form f9 = new Form9();
                    f9.Text = "输入控制点坐标";
                    f9.Show();
                }
            }
            else if(radioButton6.Checked)
            {
                if (JudgeP(e.X, e.Y) != -1)
                {
                    re2[ang % 3] = JudgeP(e.X, e.Y);

                //如果点击次数为3的倍速连接角
                if (ang% 3 == 2)
                {
                    gra.DrawLine(Pens.Green, AllPoint[re2[0]], AllPoint[re2[1]]);
                    gra.DrawLine(Pens.Green, AllPoint[re2[2]], AllPoint[re2[1]]);

                    ConAng[(ang + 1) / 3 - 1, 0] = re2[0];
                    ConAng[(ang + 1) / 3 - 1, 1] = re2[1];
                    ConAng[(ang + 1) / 3 - 1, 2] = re2[2];
                    golobal_value.Ang= (ang + 1) / 3 - 1;
                    Form f10= new Form10();
                    f10.Text = "输入∠"+ re2[0].ToString()+ re2[1].ToString() + re2[2].ToString() ;
                    f10.Show();

                }
                    ang++;
                }


            }
        }
        public int JudgeP(int x, int y)
        {
            for (int kkkkkk = 0; kkkkkk <i; kkkkkk++)
            {
                if (x <= AllPoint[kkkkkk].X + 10 && x >= AllPoint[kkkkkk].X-10 && 
                    y <= AllPoint[kkkkkk].Y + 10 && y >= AllPoint[kkkkkk].Y-10)
                    return kkkkkk;
            }
            return -1;
        }
        public int JudgePKnow(int x, int y)
        {
            for (int kkkkkk = 0; kkkkkk<=c; kkkkkk++)
            {
                if (x <= AllPoint[RecNum[kkkkkk]].X + 10 && x >= AllPoint[RecNum[kkkkkk]].X - 10 &&
                    y <= AllPoint[RecNum[kkkkkk]].Y + 10 && y >= AllPoint[RecNum[kkkkkk]].Y - 10)
                    return kkkkkk;
            }
            return -1;

        }
        public int JudgeL(int x,int y)
        {
            for (int kkkkk=0; kkkkk<((j+1)/2); kkkkk++)
            {
                double slope;
                double intercept;
                int x1 = AllPoint[Con[kkkkk, 0]].X;
                int x2 = AllPoint[Con[kkkkk, 1]].X;
                int y1 = AllPoint[Con[kkkkk, 0]].Y;
                int y2 = AllPoint[Con[kkkkk, 1]].Y;
                int dy = y2 - y1;
                int dx = x2 - x1;
                double Doubley = double.Parse(dy.ToString());
                double Doublex = double.Parse(dx.ToString());

                if (dx == 0)
                {
                    slope = dy / 0.0000001;
                }
                else
                {
                    slope = Doubley / Doublex;
                }
                intercept = y1 - x1 * slope;

                string str1 = x1.ToString();
                string str2 = x2.ToString();
                string str3 = dx.ToString();
                string str4 = dy.ToString();
                string str5 = slope.ToString();            
                if (x1>x2)
                {
                    int temp = x2;
                    x2 = x1;
                    x1 = temp;

                }
                // 画出线的选择范围
                //int y_1 = int.Parse((slope * x1 + intercept + 5).ToString());
                //int y_2 = int.Parse((slope * x2 + intercept + 5).ToString());
                //Point p1=new Point(x1,y_1);
                //Point p2 = new Point(x2, y_2);
                //gra.DrawLine(Pens.Yellow,p1,p2);

                // y_1 = int.Parse((slope * x1 + intercept - 5).ToString());
                // y_2 = int.Parse((slope * x2 + intercept - 5).ToString());
                //p1 = new Point(x1, y_1);
                //p2 = new Point(x2, y_2);
                //gra.DrawLine(Pens.Yellow, p1, p2);
                if (x2-x1>=10)
                {
                    if (y <= slope * x + intercept + 5 && y >= slope * x + intercept - 5 &&
                    x > x1 + 4 && x < x2 - 4)
                    {
                        return kkkkk;
                    }
                }
                else
                {
                    if (y <= slope * x + intercept + 5 && y >= slope * x + intercept - 5 &&
                    x > x1 -5 && x < x2 +4)
                    {
                        return kkkkk;
                    }
                }            


            }
            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {



            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox3.Items.Add("点 x, y");
            listBox1.Items.Add("边 距离");
            listBox2.Items.Add("  角  °  ′  ″");

          
            for (int rr = 0; rr < (j + 1) / 2; rr ++)
            {                
                string str1 = Con[rr, 0].ToString()+ Con[rr, 1].ToString() + ",  ";
                string str2 = golobal_value.AC[rr, 0].ToString();
                if (golobal_value.AC[rr, 0] == 0)
                    continue;
                else
                    listBox1.Items.Add( str1 + str2);                
            }
          
            for (int rr = 0; rr < c; rr++)
            {
                string str1 = RecNum[rr].ToString() + ", ";
                string str2 = golobal_value.AP[rr, 0].ToString() + ", " + golobal_value.AP[rr, 1].ToString();
                listBox3.Items.Add(str1 + str2);                
            }
          
            for (int rr = 0; rr < (ang+1)/3; rr++)
            {
                string str1 = ConAng[rr, 0].ToString() +  ConAng[rr, 1].ToString() +   ConAng[rr, 2].ToString() + ", ";
                string str2 = golobal_value.AAng[rr, 0].ToString() + ", " + golobal_value.AAng[rr, 1].ToString() + ", " +
                    golobal_value.AAng[rr, 2].ToString();
                listBox2.Items.Add("∠" + str1 + str2);
                
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string path = @".\data";


            // Determine whether the directory exists.
            if (!Directory.Exists(path))
            {
                // Create the directory it does not exist.
                Directory.CreateDirectory(path);
            }

            path = @".\result";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string pathLengths = @".\data\Lengths.txt";
            string pathAngles = @".\data\Angles.txt";
            string pathKnownPoints = @".\data\knownPoints.txt";

            
            StreamWriter textWriter = new StreamWriter(pathLengths);

            for (int rr = 0; rr < (j + 1) / 2; rr++)
            {
                string str1 = Con[rr, 0].ToString() + "," + Con[rr, 1].ToString() + ",";
                string str2 = golobal_value.AC[rr, 0].ToString();

                if (golobal_value.AC[rr, 0] == 0)
                    continue;
                else
                {
                    if (rr < (j + 1) / 2 - 1)
                        textWriter.WriteLine(str1 + str2);
                    else
                        textWriter.Write(str1 + str2);
                }
            }
            textWriter.Close();
            StreamWriter textWriter2 = new StreamWriter(pathKnownPoints);
            for (int rr = 0; rr < c; rr++)
            {
                string str1 = RecNum[rr].ToString() + ",";
                string str2 = golobal_value.AP[rr, 0].ToString() + "," + golobal_value.AP[rr, 1].ToString();
                if (rr < c - 1)
                    textWriter2.WriteLine(str1 + str2);
                else
                    textWriter2.Write(str1 + str2);                
            }
            textWriter2.Close();

            StreamWriter textWriter3 = new StreamWriter(pathAngles);

            for (int rr = 0; rr < (ang + 1) / 3; rr++)
            {
                string str1 = ConAng[rr, 0].ToString() + "," + ConAng[rr, 1].ToString() + "," + ConAng[rr, 2].ToString() + ",";
                string str2 = golobal_value.AAng[rr, 0].ToString() + "," + golobal_value.AAng[rr, 1].ToString() + "," +
                    golobal_value.AAng[rr, 2].ToString();

                if (rr < (ang + 1) / 3 - 1)
                    textWriter3.WriteLine(str1 + str2);
                else
                    textWriter3.Write(str1 + str2);     
            }
            textWriter3.Close();
        }       

        private void 画图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rectangle rect0 = new Rectangle(1, 1, 430, 263);
            pen = new Pen(Color.Blue, 1);
            gra.DrawRectangle(pen, rect0);
        }

       

        private void 状态栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rectangle rect0 = new Rectangle(1, 1, 430, 263);
            pen = new Pen(Color.Blue, 1);
            gra.DrawRectangle(pen, rect0);
            if (groupBox1.Visible == false)
                groupBox1.Visible = true;
            else
                groupBox1.Visible = false;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        
        private void Indirect_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const double PI = Math.PI;
            string pathLengths = @".\data\Lengths.txt";
            string pathAngles = @".\data\Angles.txt";

            string pathLengthsAdjust =
            @".\result\LengthsAdjust.txt";
            string pathAnglesAdjust =
            @".\result\AnglesAdjust.txt";
            string pathPointsAdjust =
            @".\result\PointsAdjust.txt";


            int adjustNum = 1;
            double d0 = 0;
            TraverseNetwork net;
            Indirect indirect;
            bool changeRoute = false;
            for (adjustNum = 1; adjustNum <= 1; adjustNum++)
            {
                if (adjustNum <= 10 || adjustNum % 10 == 0)
                {

                }


                if (adjustNum == 1)
                {
                    net = new TraverseNetwork(pathLengths, pathAngles, false, false);
                    d0 = net.deltaAngle;
                }
                else
                {
                    changeRoute = !changeRoute;
                    net = new TraverseNetwork(pathLengthsAdjust, pathAnglesAdjust, true, changeRoute);

                }

                indirect = new Indirect(net);
                indirect.Calculate_observe();
                indirect.CalculatePointsAppro();
                indirect.Calculate_paramAppro();
                indirect.CalculateObserveAppro();
                indirect.Calculate_B();
                indirect.Calculate_P(d0);
                indirect.Compute_l();
                indirect.Compute_x();
                indirect.Compute_V();
                indirect.Compute_observeAdjust();
                indirect.Compute_paramAdjust();
                d0 = indirect.Accuracy();//此步平差的d0平差值的中误差,下一步的初始d0

                indirect.WriteObserAdjust();

                indirect.WritePointsAdjust();

                IGraph graph = new IGraph(net.pointNum, net.lengthNum);
                graph.UndirectedGraph(pathLengths, false);
                graph.adjMatrix.value[net.startKnownPoint, net.endKnownPoint] = 1;
                graph.adjMatrix.value[net.endKnownPoint, net.startKnownPoint] = 1;
                List<List<int>> circles = graph.DFS_FindCircle();
                for (int i = 0; i < circles.Count; i++)
                {
                    List<int> circle = circles[i];
                    double angleClosure = indirect.CalculateAngleClosure(circle);
                    //Console.WriteLine("angle closure: {0}''", angleClosure);
                }

                for (int i = 0; i < net.lengthNum; i++)
                {
                    // Console.WriteLine("{0}mm", indirect.V.value[i, 0] * 1000);
                }
                for (int i = net.lengthNum; i < net.lengthNum + net.angleNum - 1; i++)
                {
                    // Console.WriteLine("{0}", indirect.V.value[i, 0] / PI * 180 * 3600);
                }
            }
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 控制点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }

        private void 控制点ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }

        private void 未知点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
        }

        private void 已知点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
        }

        private void 水准路线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButton3.Checked = true;
        }

        private void 导线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButton3.Checked = true;
        }

        private void 控制点ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            radioButton5.Checked = true;
        }

        private void 边ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButton4.Checked = true;
        }

        private void 观测角ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButton6.Checked = true;
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("chrome.exe", "https://localhost:44384/EasyAdjust.html");
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            for (int rr = 0; rr < (j + 1) / 2; rr++)
            {
                string str2 = golobal_value.AC[rr, 0].ToString();
                if (golobal_value.AC[rr, 0] == 0)
                    continue;
                else
                {
                    string L = Con[rr, 0].ToString() + Con[rr, 1].ToString();
                    double Dis=golobal_value.AC[rr, 0];
                    var db = new AccessDb();
                    db.InsertLine(L,Dis);
                }
                
            }

            for (int rr = 0; rr < c; rr++)
            {                
                double p = RecNum[rr];
                double x = golobal_value.AP[rr, 0];
                double y = golobal_value.AP[rr, 1];
                var db = new AccessDb();
                db.InsertPoint(p, x, y);
                 
            }
            for (int rr = 0; rr < (ang + 1) / 3; rr++)
                {
                    string str1 = ConAng[rr, 0].ToString() + ConAng[rr, 1].ToString() + ConAng[rr, 2].ToString();                  
                    string Ang = "∠"+str1;
                    double d = golobal_value.AAng[rr, 0];
                    double m = golobal_value.AAng[rr, 1];
                    double s = golobal_value.AAng[rr, 2];
                    var db = new AccessDb();
                    db.InsertAng(Ang, d, m, s);     
                }
                
                         
           
           
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
