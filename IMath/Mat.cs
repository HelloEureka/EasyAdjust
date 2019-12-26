using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace IMath
{
    public class Mat
    {
        public int row, col;   //行与列
        public double[,] value = new double[30, 30];//矩阵值

        #region in
        public Mat(int m, int n, bool isIdentity = true)
        {
            row = m;
            col = n;
            int i, j;
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < col; j++)
                {
                    value[i, j] = 0;


                    //value[i, j] = double.Parse(Console.ReadLine());
                }
                if (isIdentity)
                    value[i, i] = 1; //初始化为单位阵
            }

        }

        public Mat(int m, int n, string dataPath)
        {
            row = m;
            col = n;
            var reader = new StreamReader(dataPath);
            string buf; //缓冲区
            int i, j;
            for (i = 0; i < row; i++)
            {
                buf = reader.ReadLine();    //内容读入缓冲区，“指针”移动到下一行开头！！！
                if (col != 1)
                {
                    var arrValue = buf.Split(',');   //以‘’之间的为标识分割字符串
                    for (j = 0; j < col; j++)
                    {
                        value[i, j] = double.Parse(arrValue[j]);
                    }
                }
                else
                {
                    value[i, 0] = Convert.ToDouble(buf);
                }

            }
            reader.Close();
        }

        public Mat DeepCopy()
        {
            Mat copy = new Mat(this.row, this.col);
            int i, j;
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < col; j++)
                {
                    copy.value[i, j] = this.value[i, j];
                }
            }
            return copy;
        }
        #endregion





        #region out
        public void PrintMatrix()
        {
            int i, j;
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < col; j++)
                    Console.Write("{0} ", value[i, j]);
                Console.Write("\n\n\n");
            }
        }

        public void PrintMatrixEasy()
        {
            double accur = 0.000000001;
            int i, j;
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < col; j++)
                {
                    if (-accur < value[i, j] & value[i, j] < accur)
                    {
                        Console.Write("{0,-7} ", 0);
                    }
                    else
                        Console.Write("{0,-7:f2} ", value[i, j]);
                }
                Console.Write("\n\n\n");
            }
        }

        public void WriteMatrix(string filePath, bool isAppend = true)    //TODO
        {

            StreamWriter streamWriter = new StreamWriter(filePath, isAppend);
            int i, j;
            for (i = 0; i < row; i++)
            {
                for (j = 0; j < col; j++)
                {
                    if (j != col - 1)
                        streamWriter.Write("{0},", value[i, j]);
                    else
                        streamWriter.Write("{0}", value[i, j]);
                }

                streamWriter.Write("\n");
            }

            //streamWriter.Flush();
            ////关闭流
            streamWriter.Close();
        }
        #endregion



        #region operate
        public Mat Add(Mat A)
        {
            Mat res = new Mat(row, col);
            if (A.row != row || A.col != col)
            {
                Console.WriteLine("add error");
            }
            int i, j;
            for (i = 0; i < row; i++)
                for (j = 0; j < col; j++)
                    res.value[i, j] = value[i, j] + A.value[i, j];
            return res;
        }
        public Mat Minus(Mat A)
        {
            if (A.row != row || A.col != col)
            {
                Console.WriteLine("minus error");
            }
            Mat res = new Mat(row, col);
            int i, j;
            for (i = 0; i < row; i++)
                for (j = 0; j < col; j++)
                    res.value[i, j] = value[i, j] - A.value[i, j];
            return res;
        }
        private Mat Multiply(Mat A)
        {
            if (A.row != col)
            {
                Console.WriteLine("mutiply error");
            }
            Mat res = new Mat(row, A.col);
            int i, j, k;
            for (i = 0; i < row; i++)
                for (j = 0; j < col; j++)
                {
                    res.value[i, j] = 0;
                    for (k = 0; k < col; k++)
                    {
                        if (value[i, k] != 0 && A.value[k, j] != 0)
                            res.value[i, j] += value[i, k] * A.value[k, j];
                    }

                }
            return res;
        }
        //private Mat Multiply(double [] v)    //v 列向量
        //{
        //    Mat res = new Mat(row, 1);
        //    int i, j;
        //    for (i = 0; i < row; i++)
        //    {
        //        res.value[i, 0] = 0;
        //        for (j = 0; j < col; j++)
        //        {
        //            res.value[i, 0] += value[i, j] * v[j];
        //        }
        //    }
        //    return res;
        //}

        private Mat Multiply(double a)
        {
            Mat res = new Mat(row, col);
            int i, j;
            for (i = 0; i < row; i++)
                for (j = 0; j < col; j++)
                    res.value[i, j] = a * value[i, j];
            return res;
        }
        private Mat Inverse()   //矩阵求逆
        {
            //int n = row;
            //Mat res = new Mat(n, n);
            //double norm = this.Norm();

            //if (n == 1)
            //    res.value[0, 0] = 1 / this.value[0, 0];
            //else
            //{
            //    int i, j;
            //    for (i = 0; i < n; i++)
            //        for (j = 0; j < n; j++)
            //            res.value[i, j] = Math.Pow(-1, i + j) * this.Norm(this.cofactor(j, i), n - 1)
            //                                / norm;
            //}


            Mat[] LU = this.Factor_LU();
            Mat L = LU[0];
            Mat U = LU[1];

            Mat res = U.inverse_UpTri() * L.inverse_LowTri();

            return res;
        }

        public Mat inverse_UpTri()
        {
            int n = row;
            Mat res = new Mat(n, n);
            int i, j, k;
            double sum;
            for (i = 0; i < n; i++)
            {
                res.value[i, i] = 1 / value[i, i];
            }
            for (i = n - 2; i >= 0; i--)  //i递减
            {
                for (j = i + 1; j < n; j++)
                {
                    sum = 0;
                    for (k = i + 1; k <= j; k++)
                    {
                        sum += res.value[k, j] * value[i, k];
                    }
                    res.value[i, j] = -sum / value[i, i];
                }
            }
            return res;
        }

        public Mat inverse_LowTri()
        {
            int n = row;
            Mat res = ((this.Transpose()).inverse_UpTri()).Transpose();//T,-1,T
            return res;
        }

        public Mat Transpose()
        {
            Mat res = new Mat(col, row);
            int i, j;
            for (i = 0; i < row; i++)
                for (j = 0; j < col; j++)
                    res.value[j, i] = value[i, j];
            return res;
        }   //矩阵转置
        public Mat cofactor(int m, int n)    //(m,n)的余子式
        {
            Mat res = new Mat(row - 1, col - 1);
            int i, j;
            int rowSkip = 0;
            int colSkip = 0;
            for (i = 0; i < row; i++)
            {
                colSkip = 0;
                if (i == m)
                {
                    rowSkip = 1;
                    continue;
                }
                else
                {
                    for (j = 0; j < col; j++)
                    {
                        if (j == n)
                        {
                            colSkip = 1;
                            continue;
                        }
                        else
                        {
                            res.value[i - rowSkip, j - colSkip] = value[i, j];

                        }

                    }
                }
            }
            return res;
        }

        public Mat[] Factor_LU()
        {
            int n = row;
            Mat L = new Mat(n, n);
            Mat U = new Mat(n, n);
            Mat[] lu = new Mat[2];
            U.value[0, 0] = value[0, 0];

            int i, j, k, s;
            for (i = 1; i < n; i++)
            {
                U.value[0, i] = value[0, i];

                L.value[i, 0] = value[i, 0] / value[0, 0];
            }
            double sum = 0;
            for (k = 1; k < n; k++)
            {
                for (i = k; i < n; i++)
                {
                    sum = 0;
                    for (s = 0; s <= k - 1; s++)
                    {
                        sum += L.value[k, s] * U.value[s, i];
                    }
                    U.value[k, i] = value[k, i] - sum;

                    if (i == k)
                    {
                        continue;
                    }
                    sum = 0;
                    for (s = 0; s <= k - 1; s++)
                    {
                        sum += L.value[i, s] * U.value[s, k];
                    }
                    L.value[i, k] = (value[i, k] - sum) / U.value[k, k];
                }

            }
            lu[0] = L.DeepCopy();
            lu[1] = U.DeepCopy();
            return lu;

        }


        public Mat Factor_Cholesky(Mat A)   //TODO
        {
            Mat G = new Mat(A.row, A.col);
            //int i, j, k;
            //double sum = 0;
            //for (i = 0; i < A.row; i++)
            //{
            //    for (j = 0; j <= i; j++)
            //    {
            //        if (i == j)
            //        {
            //            for (k = 0; k <= i - 1; k++)
            //            {
            //                sum += G.value[i, k] * G.value[i, k];
            //            }
            //            G.value[i, i] = Math.Sqrt(A.value[i, i] - sum);
            //        }
            //        else
            //        {
            //            G
            //        }
            //    }
            //}
            return G;
        }

        public Mat[] Factor_LDLt(Mat A)     //TODO
        {
            int n = A.row;
            Mat[] LDLTt = new Mat[2];
            LDLTt[0] = new Mat(n, n);//L
            LDLTt[1] = new Mat(n, n);//D
            //double[,] u = new double[n,n];
            //int i, j, k;
            //double sum = 0;
            //for (i = 0; i < n; i++)
            //{
            //    for (k = 0; k <= i - 1; k++)
            //    {
            //        sum += u[i, k] * LDLTt[0].value[i, k];
            //    }
            //    LDLTt[1].value[i, i] = A.value[i, i] - sum;
            //}
            return LDLTt;
        }

        public double NormTriangular()
        {
            double res = 1;
            for (int i = 0; i < row; i++)
            {
                res *= value[i, i];
            }
            return res;
        }

        //public double Norm(Mat A, int n)    //求模
        //{
        //    double norm = 0;
        //    //if (n == 1)
        //    //    norm = A.value[0, 0];
        //    //else
        //    //{
        //    //    int j;
        //    //    int sign = -1;
        //    //    for (j = 0; j < n; j++)
        //    //    {
        //    //        if (A.value[0, j] != 0)
        //    //        {
        //    //            sign *= -1;
        //    //            norm += sign * A.value[0, j] * Norm(A.cofactor(0, j), n - 1);
        //    //        }
        //    //    }
        //    //}

        //    return norm;
        //}


        public double Norm()    //求模
        {

            //int j;
            //int sign = -1;
            //if (this.row == 1)
            //    norm = this.value[0, 0];
            //else
            //{
            //    for (j = 0; j < row; j++)
            //    {
            //        sign *= -1;
            //        norm += sign * value[0, j] * Norm(this.cofactor(0, j), row - 1);
            //    }
            //}
            Mat[] LU = this.Factor_LU();
            double norm = LU[1].NormTriangular();
            return norm;
        }
        #endregion


        #region operator

        public static Mat operator +(Mat left, Mat right)
        {
            return left.Add(right);
        }

        public static Mat operator -(Mat left, Mat right)
        {
            return left.Minus(right);
        }

        public static Mat operator *(Mat left, Mat right)
        {
            return left.Multiply(right);
        }
        public static Mat operator *(double left, Mat right)
        {
            return right.Multiply(left);
        }
        public static Mat operator *(Mat left, double right)
        {
            return left.Multiply(right);
        }

        public static Mat operator /(Mat left, Mat right)
        {
            return left * (right.Inverse());
        }
        public static Mat operator /(Mat left, double right)
        {
            return left * (1 / right);
        }
        public static Mat operator /(double left, Mat right)
        {
            return left * (right.Inverse());
        }
        #endregion
    }
}
