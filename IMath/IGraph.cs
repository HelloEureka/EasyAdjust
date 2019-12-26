using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IMath
{
    public class IGraph
    {
        public int V;
        public int E;
        public Mat adjMatrix;

        public int startPoint;

        public IGraph(int vertex, int edge)
        {
            V = vertex;
            E = edge;
            adjMatrix = new Mat(V, V, false);

        }
        public void UndirectedGraph(string filepathPoint, bool flag = true)
        {

            var reader = new StreamReader(filepathPoint);
            int i, j;
            double value;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var buf = line.Split(',');
                i = Int32.Parse(buf[0]);
                j = Int32.Parse(buf[1]);
                if (!flag)
                {
                    value = double.Parse(buf[2]);
                    adjMatrix.value[i, j] = value;
                    adjMatrix.value[j, i] = value;
                }
                if (flag)
                {
                    adjMatrix.value[i, j] = 1;
                    adjMatrix.value[j, i] = 1;
                }
            }
        }
        public void UndirectedGraph()
        {
            int i, j;
            for (i = 0; i < V; i++)
            {
                for (j = i; j < V; j++)
                {
                    if (adjMatrix.value[i, j] != 0)
                    {
                        adjMatrix.value[j, i] = adjMatrix.value[i, j];
                    }
                }
            }
        }



        public void BFS()
        {
            startPoint = 0;
            int i, u, v = 0;
            Queue<int> q = new Queue<int>();

            for (i = 0; i < V; i++)
            {
                adjMatrix.value[i, i] = 0;

            }
            adjMatrix.value[startPoint, startPoint] = 1;
            q.Enqueue(startPoint);

            while (q.Count != 0)
            {
                u = q.Dequeue();

                for (i = 0; i < V; i++)
                {
                    if (adjMatrix.value[u, i] > 0 & i != u)
                    {
                        v = i;
                        if (adjMatrix.value[v, v] == 0)
                        {
                            adjMatrix.value[v, v] = 1;
                            adjMatrix.value[v, u] = 0;
                            q.Enqueue(v);
                            Console.WriteLine("{0}", v);
                            Console.WriteLine("{0}->{1}", u, v);
                        }
                    }
                }


                adjMatrix.value[u, u] = 2;
            }

        }

        public void DFS()
        {
            startPoint = 1;
            int i, u, v = 0;

            int n = 0;
            for (i = 0; i < V; i++)
            {
                adjMatrix.value[i, i] = 0;
            }

            for (i = 0; i < V; i++)
            {
                if (adjMatrix.value[i, i] == 0)
                {
                    u = i;
                    DFS_VISIT(u);
                }
            }
        }
        public void DFS_VISIT(int u)
        {
            int v;
            adjMatrix.value[u, u] = 1;
            for (int i = 0; i < V; i++)
            {
                if (adjMatrix.value[u, i] != 0)
                {
                    v = i;
                    if (adjMatrix.value[v, v] == 0)
                    {
                        Console.WriteLine("->{0}->{1}", u, v);
                        DFS_VISIT(v);

                    }

                }
            }
            adjMatrix.value[u, u] = 2;
        }


        public void DFS(bool flag)
        {
            startPoint = 0;
            int i, u, v = 0;
            Stack<int> stack = new Stack<int>();

            for (i = 0; i < V; i++)
            {
                adjMatrix.value[i, i] = 0;

            }

            adjMatrix.value[startPoint, startPoint] = 1;
            stack.Push(startPoint);

            while (stack.Count != 0)
            {
                u = stack.Pop();

                for (i = 0; i < V; i++)
                {
                    if (adjMatrix.value[u, i] > 0 & i != u)
                    {
                        v = i;
                        if (adjMatrix.value[v, v] == 0)//树边
                        {
                            adjMatrix.value[v, v] = 1;
                            adjMatrix.value[v, u] = 0;
                            stack.Push(v);
                            Console.WriteLine("{0}", v);

                        }
                        if (adjMatrix.value[v, v] == 1)//反向边
                        {

                            adjMatrix.value[u, v] = 0;

                        }
                    }
                }



            }
        }






        public List<List<int>> DFS_FindCircle()
        {
            startPoint = 0;
            int i, u, v = 0;
            Stack<int> stack = new Stack<int>();

            for (i = 0; i < V; i++)
            {
                adjMatrix.value[i, i] = 0;

            }

            adjMatrix.value[startPoint, startPoint] = 1;
            stack.Push(startPoint);

            while (stack.Count != 0)
            {
                u = stack.Pop();

                for (i = 0; i < V; i++)
                {
                    if (adjMatrix.value[u, i] > 0 & i != u)
                    {
                        v = i;
                        if (adjMatrix.value[v, v] == 1)//反向边
                        {

                            adjMatrix.value[u, v] = 0;
                            adjMatrix.value[u, u]++;//u为结点
                        }
                        if (adjMatrix.value[v, v] == 0)//树边
                        {
                            adjMatrix.value[v, v] = 1;
                            adjMatrix.value[v, u] = 0;
                            stack.Push(v);
                            //Console.WriteLine("{0}", v);

                        }

                    }
                }
                adjMatrix.value[u, u]++;
            }
            //深搜之后黑灰色都为1，结点处大于1:1+回去路径数

            List<List<int>> circle = Backpath();
            foreach (List<int> path in circle)
            {
                foreach (int point in path)
                {
                    Console.Write("{0}->", point);
                }
                Console.WriteLine();
            }
            return circle;
        }

        public List<List<int>> Backpath()
        {
            int i = 0;
            List<List<int>> roundPaths = new List<List<int>>();
            List<List<int>> backPaths = new List<List<int>>();

            for (i = 0; i < V; i++)
            {
                if (adjMatrix.value[i, i] == 2)
                {
                    adjMatrix.value[i, i]--;
                }

                bool flag = false;
                while (adjMatrix.value[i, i] > 1)
                {
                    int node = i;
                    List<int> path = BackSearch(node, startPoint);
                    adjMatrix.value[i, i]--;
                    backPaths.Add(path);
                    flag = true;
                }
                int backPathsNum = backPaths.Count;

                int roundPathsCount = 0;
                if (flag)
                {
                    while (backPathsNum > 1)
                    {
                        backPathsNum--;
                        backPaths[backPathsNum].RemoveAt(0);
                        backPaths[backPathsNum].Reverse();
                        backPaths[backPathsNum].AddRange(backPaths[0]);
                        roundPaths.Add(backPaths[backPathsNum]);
                    }
                    backPaths.Clear();
                }

            }

            return roundPaths;
        }

        public List<int> BackSearch(int node, int source)
        {
            int i, j;
            int u, v = -1;
            List<int> backPath = new List<int>();
            backPath.Add(node);
            for (i = 0; i < V; i++)
            {
                if (adjMatrix.value[i, node] != 0 && i != node)
                {
                    u = i;
                    backPath.Add(u);
                    adjMatrix.value[u, node] = 0;
                    v = u;

                    break;
                }
            }

            for (j = 0; j < V; j++)
            {
                for (i = 0; i < V; i++)
                {
                    if (adjMatrix.value[i, v] != 0 && i != v)
                    {
                        u = i;
                        backPath.Add(u);
                        v = u;
                        break;
                    }
                }
                if (v == source)
                {
                    break;
                }
            }
            return backPath;
        }
    }
}
