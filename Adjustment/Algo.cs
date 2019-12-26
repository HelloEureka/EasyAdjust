using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using INetWork;
using IMath;

namespace Adjustment
{
    public class Indirect
    {

        const double PI = Math.PI;
        public TraverseNetwork net;
        private int n, t, r, u, c, s;//观测量，必要，多余，参数，方程，不独立参数
        public Mat observe;
        public Mat observeAppro;
        public Mat observeAdjust;
        public Mat paramAppro;
        public Mat paramAdjust;
        public Mat B, P, l, N, W, x, V;
        bool isOrigin;
        private Dictionary<int, int> xDict = new Dictionary<int, int>();//x对应参数
        private Dictionary<int, int> yDict = new Dictionary<int, int>();//y对应参数
        public string pathLengthsAdjust =
            @".\results\LengthsAdjust.txt";
        public string pathAnglesAdjust =
            @".\results\AnglesAdjust.txt";
        public string pathPointsAdjust =
            @".\results\PointsAdjust.txt";
        public string pathObserveD =
            @".\results\observeD.txt";
        public string pathObserveAdjustD =
            @".\results\ObserveAdjustD.txt";
        public string pathParamAdjustD =
            @".\results\ParamAdjustD.txt";

        public string pathB =
            @".\results\MatrixB.txt";


        public Indirect(TraverseNetwork travNet, bool isAdjusted = false)
        {
            this.net = travNet;
            n = travNet.lengthNum + travNet.angleNum;
            t = 2 * (net.pointNum - 2);//可扩展成多个点
            r = n - t;
            u = t;
            c = r + u;
            s = 0;
            observe = new Mat(n, 1, false);
            observeAppro = new Mat(n, 1, false);
            observeAdjust = new Mat(n, 1, false);
            paramAppro = new Mat(u, 1, false);
            paramAdjust = new Mat(u, 1, false);
            B = new Mat(n, u, false);
            P = new Mat(n, n);
            l = new Mat(n, 1, false);
            x = new Mat(u, 1, false);
            V = new Mat(n, 1, false);
            MakeParaDictionary();
            isOrigin = !isAdjusted;
        }
        private void MakeParaDictionary()
        {
            int index = 0;
            for (int pointOrder = 0; pointOrder < net.pointNum; pointOrder++)
            {
                if (pointOrder != net.startKnownPoint && pointOrder != net.endKnownPoint)
                {
                    xDict.Add(pointOrder, index);
                    index++;
                    yDict.Add(pointOrder, index);
                    index++;

                }
            }
        }

        private void CalculateChildPoint(int parent, int me, int child)
        {
            double azimuth_pm = NetPoint.Azimuth(net.netPoints[parent], net.netPoints[me]);
            int angleOrder = -1;     //设置-1，以检查错误
            int lengthOrder = -1;   //设置-1，以检查错误
            bool isleft = false;
            int i;

            //寻找角度序号
            for (i = 0; i < net.angleNum; i++)
            {
                if (net.netAngles[i].me == me)
                {
                    if (net.netAngles[i].parent == parent && net.netAngles[i].child == child)
                    {
                        isleft = true;
                        angleOrder = i;
                        break;
                    }
                    if (net.netAngles[i].parent == child && net.netAngles[i].child == parent)
                    {
                        isleft = false;
                        angleOrder = i;
                        break;
                    }
                }

            }

            //计算当前点到子点的坐标方位角
            double azimuth_mc;
            if (isleft)
            {
                azimuth_pm += PI;//反向
                azimuth_mc = azimuth_pm + net.netAngles[angleOrder].value;//加左角
                //azimuth_mc = IMathNet.Adjust_0To2PI(azimuth_mc);//调范围
            }
            else
            {
                azimuth_pm += PI;//反向
                azimuth_mc = azimuth_pm - net.netAngles[angleOrder].value;//减右角
                                                                          // azimuth_mc = IMathNet.Adjust_0To2PI(azimuth_mc);//调范围
            }

            //寻找长度序号

            for (i = 0; i < net.lengthNum; i++)
            {
                if (child == net.netLengths[i].start || child == net.netLengths[i].end)
                {
                    if (me == net.netLengths[i].start || me == net.netLengths[i].end)
                    {
                        lengthOrder = i;
                        break;
                    }

                }

            }
            double dis = net.netLengths[lengthOrder].value;

            //计算子点坐标
            double dx = dis * Math.Cos(azimuth_mc);
            double dy = dis * Math.Sin(azimuth_mc);
            net.netPoints[child].x = net.netPoints[me].x + dx;
            net.netPoints[child].y = net.netPoints[me].y + dy;
        }
        private void CalculateLengthsAppro()
        {
            for (int i = 0; i < net.lengthNum; i++)
            {
                int start = net.netLengths[i].start;
                int end = net.netLengths[i].end;
                double length = NetPoint.Distance
                    (net.netPoints[start], net.netPoints[end]);
                net.netLengths[i].value = length;
                observeAppro.value[i, 0] = length;
            }
        }
        private void CalculateAngleAppro()
        {
            for (int i = 0; i < net.angleNum; i++)
            {
                int parent = net.netAngles[i].parent;
                int me = net.netAngles[i].me;
                int child = net.netAngles[i].child;
                //mp + theta = mc
                double angle_mp = NetPoint.Azimuth
                    (net.netPoints[me], net.netPoints[parent]);
                double angle_mc = NetPoint.Azimuth
                    (net.netPoints[me], net.netPoints[child]);
                double theta = angle_mc - angle_mp;
                theta = IMathNet.Adjust_0To2PI(theta);
                net.netAngles[i].value = theta;
                observeAppro.value[i + net.lengthNum, 0] = theta;
            }
        }

        public void Calculate_observe()
        {

            int i;
            for (i = 0; i < net.lengthNum; i++)
            {
                observe.value[i, 0] = net.netLengths[i].value;
            }
            for (i = 0; i < net.angleNum; i++)
            {
                observe.value[i + net.lengthNum, 0] = net.netAngles[i].value;
            }
        }


        public void CalculatePointsAppro()
        {
            int u, m, v;//父点、当前点、子点
            int i, j;
            Queue<int> q = new Queue<int>();
            for (i = 0; i < net.pointNum; i++)
            {
                net.adjMatrix.value[i, i] = 0;//所有点均未确定
            }
            net.adjMatrix.value[net.startKnownPoint, net.startKnownPoint] = 1;
            net.adjMatrix.value[net.endKnownPoint, net.endKnownPoint] = 1;

            u = net.endKnownPoint;
            m = net.startKnownPoint;

            q.Enqueue(net.startKnownPoint);
            while (q.Count != 0)
            {
                m = q.Dequeue();

                for (j = 0; j < net.pointNum; j++)
                {
                    if (net.adjMatrix.value[m, j] > 0 && j != m && net.adjMatrix.value[j, j] == 0)
                    {//找到子点，且子点未被查找过
                        v = j;
                        if (m == net.startKnownPoint)
                        {
                            u = net.endKnownPoint;
                        }
                        else
                        {
                            for (i = 0; i < net.pointNum; i++)//找到子节点才有必要找父节点
                            {
                                if (net.adjMatrix.value[i, m] > 0 && i != m && net.adjMatrix.value[i, i] != 0)
                                {
                                    u = i;
                                    break;//找到一个已知父节点就能算了
                                }
                            }
                        }

                        CalculateChildPoint(u, m, v);
                        //Console.WriteLine("{0}->{1}->{2}", u, m, v);
                        net.adjMatrix.value[v, v] = 1;//该点已被确定
                        q.Enqueue(v);
                    }
                }
            }
        }

        public void CalculateObserveAppro()
        {
            CalculateLengthsAppro();
            CalculateAngleAppro();
        }

        public void Calculate_paramAppro()
        {
            for (int i = 0; i < net.pointNum; i++)
            {
                if (i != net.startKnownPoint && i != net.endKnownPoint)
                {
                    paramAppro.value[xDict[i], 0] = net.netPoints[i].x;
                    paramAppro.value[yDict[i], 0] = net.netPoints[i].y;
                }
            }

        }

        #region BPlxV


        public void Calculate_B()
        {
            Equation_L();
            Equation_A();
            B.WriteMatrix(pathB);
        }

        #region Equation
        public void Equation_L()
        {
            for (int i = 0; i < net.lengthNum; i++)
            {
                int start = net.netLengths[i].start;
                int end = net.netLengths[i].end;
                double azimuth = NetPoint.Azimuth
                    (net.netPoints[start], net.netPoints[end]);
                double cos = Math.Cos(azimuth);
                double sin = Math.Sin(azimuth);
                if (start != net.startKnownPoint && start != net.endKnownPoint)
                {
                    B.value[i, xDict[start]] = -cos;
                    B.value[i, yDict[start]] = -sin;
                }
                if (end != net.startKnownPoint && end != net.endKnownPoint)
                {
                    B.value[i, xDict[end]] = cos;
                    B.value[i, yDict[end]] = sin;
                }
            }
        }

        public void Equation_A()
        {
            double theta, dis, c_mp, d_mp, c_mc, d_mc;
            for (int i = 0; i < net.angleNum; i++)
            {
                int parent = net.netAngles[i].parent;
                int me = net.netAngles[i].me;
                int child = net.netAngles[i].child;
                //mp
                theta = NetPoint.Azimuth(net.netPoints[me], net.netPoints[parent]);
                dis = NetPoint.Distance(net.netPoints[me], net.netPoints[parent]);
                c_mp = -Math.Sin(theta) / dis;
                d_mp = Math.Cos(theta) / dis;
                theta = NetPoint.Azimuth(net.netPoints[me], net.netPoints[child]);
                dis = NetPoint.Distance(net.netPoints[me], net.netPoints[child]);
                c_mc = -Math.Sin(theta) / dis;
                d_mc = Math.Cos(theta) / dis;

                int row = i + net.lengthNum;
                if (parent != net.endKnownPoint && parent != net.startKnownPoint)
                {
                    B.value[row, xDict[parent]] = -c_mp;
                    B.value[row, yDict[parent]] = -d_mp;
                }
                if (me != net.endKnownPoint && me != net.startKnownPoint)
                {
                    B.value[row, xDict[me]] = c_mp - c_mc;
                    B.value[row, yDict[me]] = d_mp - d_mc;
                }
                if (child != net.endKnownPoint && child != net.startKnownPoint)
                {
                    B.value[row, xDict[child]] = c_mc;
                    B.value[row, yDict[child]] = d_mc;
                }
            }
        }
        #endregion

        public void Calculate_P(double d0)//no problem
        {//是否原始数据，默认未经平差
            //P初始化为单位阵，右下矩阵即角度权阵为单位阵
            Mat D = new Mat(n, n);
            if (isOrigin)
            {
                for (int i = 0; i < net.lengthNum; i++)
                {
                    int indexL = i;
                    double dL = net.netLengths[i].value * net.deltaLength;
                    D.value[indexL, indexL] = dL * dL;
                }
                for (int i = 0; i < net.angleNum; i++)
                {
                    int indexA = i + net.lengthNum;
                    D.value[indexA, indexA] = net.deltaAngle * net.deltaAngle;
                }
                D.WriteMatrix(pathObserveD, false);//写出原始观测值方差阵
            }
            else
            {
                D = new Mat(n, n, pathObserveAdjustD);//上一步观测值平差阵作为这次原始方差阵
            }
            Mat Q = D / (d0 * d0);
            P = 1 / Q;
        }
        public void Compute_l()//no problem
        {
            l = observe - observeAppro;
        }
        public void Compute_x()//no problem
        {
            N = B.Transpose() * P * B;
            W = B.Transpose() * P * l;
            x = (1 / N) * W;
        }
        public void Compute_V()//no problem
        {
            V = B * x - l;
        }


        #endregion

        public double Accuracy()
        {
            double d0Adjust = Math.Sqrt((V.Transpose() * P * V).value[0, 0] / r);
            //Console.WriteLine("delta delta0Adjust:   {0}'' ", d0Adjust / PI * 180 * 3600);
            //accur = accur / PI * 180 * 3600; //以''为单位
            double deltaMax = 0;
            Mat Q = B * (1 / N) * (B.Transpose());//观测值平差值的协方差阵
            Mat D = (d0Adjust * d0Adjust) * Q;
            D.WriteMatrix(pathObserveAdjustD, false);//观测值平差值的方差阵

            for (int i = 0; i < net.lengthNum; i++)
            {
                if (D.value[i, i] > deltaMax)
                {
                    deltaMax = Math.Sqrt(D.value[i, i]) / net.netLengths.Length;
                }
            }

           //Console.WriteLine("relative error of the weakest length:  {0} ", deltaMax);

            deltaMax = 0;
            for (int i = net.lengthNum; i < n; i++)
            {
                if (D.value[i, i] > deltaMax)
                {
                    deltaMax = D.value[i, i];
                }
            }
            deltaMax = Math.Sqrt(deltaMax);
            //Console.WriteLine("delta angleAdjust:  {0} ''", deltaMax / PI * 180 * 3600);


            Q = 1 / N;//参数平差值的协方差阵
            D = (d0Adjust * d0Adjust) * Q;
            D.WriteMatrix(pathParamAdjustD, false);//参数平差值的方差阵


            return d0Adjust;
        }


        public void Compute_observeAdjust()
        {
            //observeAdjust = observeAppro + V;
            observeAdjust = observe + V;
        }
        public void Compute_paramAdjust()
        {
            paramAdjust = paramAppro + x;
        }


        private void WriteLengthsAdjust()
        {
            StreamWriter streamWriter = new StreamWriter(pathLengthsAdjust);
            int i;
            for (i = 0; i < net.lengthNum; i++)
            {
                int LIndex = i;
                streamWriter.WriteLine("{0},{1},{2:f3}"
                    , net.netLengths[LIndex].start
                    , net.netLengths[LIndex].end
                    , observeAdjust.value[LIndex, 0]);
            }

            streamWriter.Close();
        }
        private void WriteAnglesAdjust()
        {
            StreamWriter streamWriter = new StreamWriter(pathAnglesAdjust);
            int i;
            for (i = 0; i < net.angleNum; i++)
            {
                int AIndex = i + net.lengthNum;
                double[] deg = IMathNet.ToDeg(observeAdjust.value[AIndex, 0]);
                streamWriter.WriteLine("{0},{1},{2},{3},{4},{5:f1}"
                        , net.netAngles[i].parent
                        , net.netAngles[i].me
                        , net.netAngles[i].child
                        , deg[0]//度
                        , deg[1]//分
                        , deg[2]);//秒
            }

            streamWriter.Close();
        }
        public void WriteObserAdjust()
        {
            WriteLengthsAdjust();
            WriteAnglesAdjust();
        }
        public void WritePointsAdjust()
        {
            Mat D = new Mat(u, u, pathParamAdjustD);
            StreamWriter streamWriter = new StreamWriter(pathPointsAdjust);
            int i;
            double sigmaMax = 0;
            for (i = 0; i < net.pointNum; i++)
            {
                int xIndex, yIndex;

                if (i == net.startKnownPoint || i == net.endKnownPoint)
                {
                    streamWriter.WriteLine("{0},{1},{2},  accuracy: {3}"
                    , i
                    , net.netPoints[i].x
                    , net.netPoints[i].y
                    , 0);
                }
                else
                {
                    xIndex = xDict[i];
                    yIndex = yDict[i];
                    double sigmaX2 = D.value[xIndex, xIndex];//x平方
                    double sigmaY2 = D.value[yIndex, yIndex];//y平方
                    double sigmaCoor = Math.Sqrt(sigmaX2 + sigmaY2);//点位平差值的方差
                    if (sigmaCoor > sigmaMax)
                    {
                        sigmaMax = sigmaCoor;
                    }
                    streamWriter.WriteLine("{0},{1:f3},{2:f3}  accuracy: {3:f3}mm  x: {4:f3}m  y: {5:f3}m "
                    , i
                    , paramAdjust.value[xIndex, 0]
                    , paramAdjust.value[yIndex, 0]
                    , sigmaCoor * 1000
                    , Math.Sqrt(sigmaX2)
                    , Math.Sqrt(sigmaY2));
                }
            }
            streamWriter.WriteLine("\n max: {0} mm", sigmaMax * 1000);
            streamWriter.Close();
        }

        public double CalculateAngleClosure(List<int> circle)
        {
            double angleClosure = 0;
            int Vnum = circle.Count - 1;//起始点到起始点，多一个
            int parent = circle[Vnum - 1];
            int me = circle[0];
            int child = circle[1];

            int angleOrder = -1;     //设置-1，以检查错误
            bool isleft = false;
            int i, j;

            double val;
            //寻找角度序号,判断左右角
            for (i = 0; i < net.angleNum; i++)
            {
                if (net.netAngles[i].me == me)
                {
                    if (net.netAngles[i].parent == parent && net.netAngles[i].child == child)
                    {
                        isleft = true;
                        angleOrder = i;
                        break;
                    }
                    if (net.netAngles[i].parent == child && net.netAngles[i].child == parent)
                    {
                        isleft = false;
                        angleOrder = i;
                        break;
                    }
                }

            }
            angleOrder += net.lengthNum;

            if (isleft)
            {
                val = IMathNet.Adjust_0To2PI(observeAdjust.value[angleOrder, 0]);
            }
            else
            {
                val = IMathNet.Adjust_0To2PI(-observeAdjust.value[angleOrder, 0]);
            }
            angleClosure += val;

            for (j = 0; j < Vnum - 1; j++)//有几个点就有几个角
            {
                parent = me;
                me = child;
                child = circle[j + 2];
                for (i = 0; i < net.angleNum; i++)
                {
                    if (net.netAngles[i].me == me)
                    {
                        if (net.netAngles[i].parent == parent && net.netAngles[i].child == child)
                        {
                            isleft = true;
                            angleOrder = i;
                            break;
                        }
                        if (net.netAngles[i].parent == child && net.netAngles[i].child == parent)
                        {
                            isleft = false;
                            angleOrder = i;
                            break;
                        }
                    }

                }
                angleOrder += net.lengthNum;


                if (isleft)
                {
                    val = IMathNet.Adjust_0To2PI(observeAdjust.value[angleOrder, 0]);
                }
                else
                {
                    val = IMathNet.Adjust_0To2PI(-observeAdjust.value[angleOrder, 0]);
                }
                angleClosure += val;
            }

            double res = 0;
            double inAngles = (Vnum - 2) * PI;
            double outAngles = 2 * PI * Vnum - inAngles;
            res = angleClosure - inAngles;
            bool isIn = -PI / 6 < res && res < PI / 6;
            if (!isIn)
            {
                res = angleClosure - outAngles;
            }

            res = res / PI * 180 * 3600;


            return res;
        }
    }
}
