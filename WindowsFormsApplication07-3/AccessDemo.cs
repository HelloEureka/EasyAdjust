using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;


namespace AccessDemo
{
    public class AccessDb
    {
        private OleDbConnection myCon;
        public AccessDb()
        {
            string strcon = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Lenovo\Desktop\FinalWork\Database1.mdb";
            myCon = new OleDbConnection(strcon);
        }
        void Open()
        {
            myCon.Open();
        }
        void Close()
        {
            myCon.Close();
        }
        // insert into Coor (Site, North, East) values('{0}',{1},{2})
        //double p, double x, double y ,string L, double Dis, 
        //p, x, y, L, Dis,
        public void InsertPoint(double p, double x,double y)
        {
            var sql = "insert into Point (	P,	X,Y) values";
            sql += string.Format("({0},{1},{2})", p,x,y);
            OleDbCommand cmd = myCon.CreateCommand();
            cmd.CommandText = sql;
            Open();
            cmd.ExecuteNonQuery();
            Close();
        }
        public void InsertLine(string L, double Dis)
        {
            var sql = "insert into Line (	line,	Dis) values";
            sql += string.Format("('{0}',{1})", L,Dis);
            OleDbCommand cmd = myCon.CreateCommand();
            cmd.CommandText = sql;
            Open();
            cmd.ExecuteNonQuery();
            Close();
        }
        public void InsertAng( string Ang, double d, double m, double s)
        {
            var sql = "insert into Ang (	Angel,	D	,M	,S) values";
            sql += string.Format("('{0}',{1},{2},{3})",  Ang, d, m, s);
            OleDbCommand cmd = myCon.CreateCommand();
            cmd.CommandText = sql;
            Open();
            cmd.ExecuteNonQuery();
            Close();
        }

        // selet * form Coor
        public List<string> Select()
        {
            List<string> res = new List<string>();
            string sql = "select * from Coor";
            OleDbCommand cmd = myCon.CreateCommand();
            cmd.CommandText = sql;
            Open();
            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.ToString() != null)
                {
                    string line = string.Format("({0},{1},{2},'{3}',{4},'{5}',{6},{7},{8})",
                        reader["Point"], reader["X"], reader["Y"], reader["Line"],
                        reader["Distance"], reader["Angel"], reader["D"], reader["M"], reader["S"]);
                    //Point	,X	,Y,	Line	,Distance,	Angel,	D	,M	,S
                    res.Add(line);
                }
            }
            reader.Close();
            return res;
        }

    }
}
