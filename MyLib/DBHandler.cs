using System.Data.OleDb;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace MyLib
{
    public class Parameter
    {
        public OleDbType type = OleDbType.Empty;
        public int count = 0;
        public List<string> col = new List<string>(); 
        public List<object> val = new List<object>();
    }

    public class DBHandler
    {
        private OleDbConnection sqlConn = null;

        ~DBHandler()
        {
            if (sqlConn != null) sqlConn.Dispose();
        }

        public void Connect(string dbfile_path)
        {
            if (!File.Exists(dbfile_path)) throw new System.Exception(dbfile_path + "檔案不存在");

            if (sqlConn != null)
            {
                sqlConn.Dispose();
                sqlConn = null;
            }

            sqlConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbfile_path);
        }

        private OleDbCommand Create_SelectCommand(string table_name, string[] col, string condition = "")
        {   
            // SELECT column1, column2, ...
            // FROM table_name
            // WHERE condition;

            string query = "SELECT " + col[0];
            for (int i = 1; i < col.Length; i++) query += ", " + col[i];

            query += " FROM " + table_name;

            if (condition == "") query += ";";
            else query += " WHERE " + condition + ";";

            return new OleDbCommand(query, this.sqlConn);
        }

        private OleDbCommand Create_InsertCommand(string table_name, string[] col, string[] val, Parameter param = null)
        {
            // INSERT INTO table_name (column1, column2, column3, ... , param_column1, param_column2)
            // VALUES (value1, value2, value3, ... , @param_column1, @param_column2);

            string query = "INSERT INTO " + table_name;

            query += " (" + col[0];
            for (int i = 1; i < col.Length; i++) query += ", " + col[i];
            if (param != null) foreach (string param_col in param.col) query += ", " + param_col;
            query += ")";

            query += " VALUES ('" + val[0] + "'";
            for (int i = 1; i < val.Length; i++) query += ", '" + val[i] + "'";
            if (param != null) foreach(string param_col in param.col) query += ", @" + param_col;
            query += ");";

            return new OleDbCommand(query, this.sqlConn);
        }

        private OleDbCommand Create_DeleteCommand(string table_name, string condition)
        {
            string query = "DELETE FROM " + table_name + " WHERE " + condition + ";";
            return new OleDbCommand(query, this.sqlConn);
        }

        private OleDbCommand Create_UpdateCommand(string table_name, string[] col, string[] val, string condition, Parameter param = null)
        {
            // UPDATE table_name
            // SET column1 = value1, column2 = value2, ..., param_column1 = @param_column1, param_column2 = @param_column1
            // WHERE condition;

            string query = "UPDATE " + table_name + " SET " + col[0] + " = '" + val[0] + "'";
            
            for(int i = 1; i < col.Length;i++) query += ", " + col[i] + " = '" + val[i] + "'";

            if (param != null) foreach (string param_col in param.col) query += ", " + param_col + " = @" + param_col;

            query += " WHERE " + condition;

            return new OleDbCommand(query, this.sqlConn);
        }

        private void Execute(OleDbCommand cmd)
        {
            try 
            {
                this.sqlConn.Open();
                cmd.ExecuteNonQuery(); 
            }
            catch { throw; }
            finally { this.sqlConn.Close(); }
        }

        private void Proc_Parameter(OleDbCommand cmd, Parameter param)
        {
            for (int i = 0; i < param.count; i++)
                cmd.Parameters.Add(param.col[i], param.type).Value = param.val[i];
        }

        public DataTable Select(string table_name, string[] col, string condition = "")
        {
            OleDbCommand cmd = Create_SelectCommand(table_name, col, condition);
            Execute(cmd);

            OleDbDataAdapter dp = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            dp.Fill(dt);

            return dt;
        }

        /// <param name="col">表單欄位，不包含參數對應的欄位</param>
        /// <param name="val">表單欄位對應的值</param>
        /// <param name="param">參數對應的欄位與值</param>
        public void Insert(string table_name, string[] col, string[] val, Parameter param = null)
        {
            OleDbCommand cmd = Create_InsertCommand(table_name, col, val, param);

            if (param != null) Proc_Parameter(cmd, param);

            Execute(cmd);
        }

        public void Delete(string table_name, string condition)
        {
            OleDbCommand cmd = Create_DeleteCommand(table_name, condition);
            Execute(cmd);
        }

        /// <param name="col">表單欄位，不包含參數對應的欄位</param>
        /// <param name="val">表單欄位對應的值</param>
        /// <param name="param">參數對應的欄位與值</param>
        public void Update(string table_name, string[] col, string[] val, string condition, Parameter param = null)
        {
            OleDbCommand cmd = Create_UpdateCommand(table_name, col, val, condition, param);

            if (param != null) Proc_Parameter(cmd, param);

            Execute(cmd);
        }

    }
}
