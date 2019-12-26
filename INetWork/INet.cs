using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IMath;


namespace INetWork
{
    public class NetPoint
    {
        public double x, y;

        public NetPoint()
        {
            x = 0;
            y = 0;

        }

        public static double Azimuth(NetPoint startPt, NetPoint endPt)
        {
            double azimuth;
            double dx = endPt.x - startPt.x;
            double dy = endPt.y - startPt.y;
            azimuth = IMathNet.Atan_2PI(dy, dx);
            return azimuth;

        }

        public static double Distance(NetPoint p1, NetPoint p2)
        {
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public class NetAngle
    {
        public int parent;
        public int me;
        public int child;
        public double value;
    }


    public class NetTraverse
    {
        public int start, end;
        public double value;

    }


    public class TraverseNetwork
    {
        #region data
        const double PI = Math.PI;


        public int lengthNum, angleNum, pointNum;
        public double deltaLength, deltaAngle;
        public double delta0;
        public int startKnownPoint, endKnownPoint;
        public Mat adjMatrix;
        public Mat observe;

        public NetPoint[] netPoints;
        public NetTraverse[] netLengths;
        public NetAngle[] netAngles;


        #endregion
        public TraverseNetwork(string pathLengths, string pathAngles, bool isAdjusted = false, bool changeRoute = false)
        {
            deltaLength = 1.0 / 2000;
            deltaAngle = 12.0 / 3600 / 180 * PI;
            lengthNum = 9;
            angleNum = 13;
            pointNum = 9;
            netPoints = new NetPoint[pointNum];

            netLengths = new NetTraverse[lengthNum];
            netAngles = new NetAngle[angleNum];
            observe = new Mat(lengthNum + angleNum, 1);

            string pathKnownPoints = @".\data\knownPoints.txt";
            adjMatrix = new Mat(pointNum, pointNum, false);
            ReadPoints(pathKnownPoints, changeRoute);
            UndirectedGraph(pathLengths);
            ReadAngles(pathAngles);
        }
        public void ReadPoints(string filepathKnownPoints, bool changeRoute = false)
        {
            var reader = new StreamReader(filepathKnownPoints);
            int i;

            for (i = 0; i < pointNum; i++)
            {
                netPoints[i] = new NetPoint();
            }

            var line = reader.ReadLine();
            var buf = line.Split(',');
            startKnownPoint = Int32.Parse(buf[0]);
            netPoints[startKnownPoint].x = double.Parse(buf[1]);
            netPoints[startKnownPoint].y = double.Parse(buf[2]);

            line = reader.ReadLine();
            buf = line.Split(',');
            endKnownPoint = Int32.Parse(buf[0]);
            netPoints[endKnownPoint].x = double.Parse(buf[1]);
            netPoints[endKnownPoint].y = double.Parse(buf[2]);

            if (changeRoute)
            {
                int temp = startKnownPoint;
                startKnownPoint = endKnownPoint;
                endKnownPoint = temp;
            }

            reader.Close();
        }
        public void UndirectedGraph(string filepathLengths)//读取导线长度 并 生成无向图
        {

            var reader = new StreamReader(filepathLengths);
            int i, j;
            int k = 0;
            double value;

            for (i = 0; i < lengthNum; i++)
            {
                netLengths[i] = new NetTraverse();
            }

            while (!reader.EndOfStream)
            {

                var line = reader.ReadLine();
                var buf = line.Split(',');
                i = Int32.Parse(buf[0]);
                j = Int32.Parse(buf[1]);
                value = double.Parse(buf[2]);
                netLengths[k].start = i;
                netLengths[k].end = j;
                netLengths[k].value = value;
                adjMatrix.value[i, j] = value;
                adjMatrix.value[j, i] = value;
                k++;
            }
            reader.Close();
        }
        public void ReadAngles(string filepathAngles)
        {
            var reader = new StreamReader(filepathAngles);
            int i;
            int degree, minute;
            double second;
            int k = 0;
            double value;

            for (i = 0; i < angleNum; i++)
            {
                netAngles[i] = new NetAngle();
            }

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var buf = line.Split(',');
                netAngles[k].parent = Int32.Parse(buf[0]);
                netAngles[k].me = Int32.Parse(buf[1]);
                netAngles[k].child = Int32.Parse(buf[2]);
                degree = Int32.Parse(buf[3]);
                minute = Int32.Parse(buf[4]);
                second = double.Parse(buf[5]);
                value = degree * 3600 + minute * 60 + second;
                value = IMathNet.ToRad(value);
                netAngles[k].value = value;
                k++;
            }
            reader.Close();
        }

        public void BFS()   //广度优先搜索，生成有向图
        {
            int i, u, v = 0;
            Queue<int> q = new Queue<int>();
            int n = 0;
            for (i = 0; i < pointNum; i++)
            {
                adjMatrix.value[i, i] = 0;


            }
            adjMatrix.value[startKnownPoint, startKnownPoint] = 1;
            q.Enqueue(startKnownPoint);

            while (q.Count != 0)
            {
                u = q.Dequeue();

                for (i = 0; i < pointNum; i++)
                {
                    if (adjMatrix.value[u, i] > 0 & i != u)
                    {
                        v = i;
                        if (adjMatrix.value[v, v] == 0)
                        {
                            adjMatrix.value[v, v] = 1;
                            adjMatrix.value[v, u] = 0;
                            q.Enqueue(v);
                        }

                    }
                }
                adjMatrix.value[u, u] = 2;
            }


        }


    }


    public class IMathNet
    {
        static double PI = Math.PI;

        #region Adjust the range of theta, mostly 0-2PI
        public static double Atan_2PI(double y, double x)
        {
            double angle = Math.Atan2(y, x) >= 0 ? Math.Atan2(y, x) : 2 * PI + Math.Atan2(y, x);
            return angle;
        }

        public static double Adjust_0To2PI(double rad)
        {
            double rad0 = rad;
            if (rad < 0)
                rad0 = rad + 2 * PI;
            else if (0 <= rad && rad < 2 * PI)
                return rad0;
            else
                rad0 = rad - 2 * PI;
            return Adjust_0To2PI(rad0);
        }
        #endregion


        #region Deg-Rad
        public static double[] ToDeg(double rad)
        {
            double[] deg = new double[3];
            double value = rad / PI * 180;
            deg[0] = (int)(value);
            value = 60 * (value - deg[0]);
            deg[1] = (int)(value);
            deg[2] = 60 * (value - deg[1]);
            return deg;
        }
        public static double ToRad(double second)//秒转弧度
        {
            return second / 3600 / 180 * PI;
        }
        #endregion
    }
}
