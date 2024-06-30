using System.Data.OleDb;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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

            StringBuilder query = new StringBuilder($"SELECT {string.Join(",", col)} FROM {table_name}");

            query.Append((condition == "") ? ";" : $" WHERE {condition};");

            return new OleDbCommand(query.ToString(), this.sqlConn);
        }

        private OleDbCommand Create_InsertCommand(string table_name, string[] col, string[] val, Parameter param = null)
        {
            // INSERT INTO table_name (column1, column2, column3, ... , param_column1, param_column2)
            // VALUES (value1, value2, value3, ... , @param_column1, @param_column2);

            StringBuilder query = new StringBuilder($"INSERT INTO {table_name} (");

            query.Append((col == null || col.Length == 0) ? "" : string.Join(", ", col));

            query.Append((param == null) ? "" : ", " + string.Join(", ", param.col));

            query.Append(") VALUES (");

            query.Append((col == null || col.Length == 0) ? "" : string.Join(", ", val.Select(v => $"'{v}'")));

            query.Append((param == null) ? "" : ", " + string.Join(", ", param.col.Select(c => $"@{c}")));

            query.Append(");");

            return new OleDbCommand(query.ToString(), this.sqlConn);
        }

        private OleDbCommand Create_DeleteCommand(string table_name, string condition)
        {
            string query = $"DELETE FROM {table_name} WHERE {condition};";
            return new OleDbCommand(query, this.sqlConn);
        }

        private OleDbCommand Create_UpdateCommand(string table_name, string[] col, string[] val, string condition, Parameter param = null)
        {
            // UPDATE table_name
            // SET column1 = value1, column2 = value2, ..., param_column1 = @param_column1, param_column2 = @param_column1
            // WHERE condition;

            StringBuilder query = new StringBuilder($"UPDATE {table_name} SET ");

            query.Append((col == null || col.Length == 0) ? "" : string.Join(", ", col.Zip(val, (c, v) => $"{c} = '{v}'")));

            query.Append((param == null) ? "" : ", " + string.Join(", ", param.col.Select(c => $"{c} = @{c}")));

            query.Append($" WHERE {condition};");

            return new OleDbCommand(query.ToString(), this.sqlConn);
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
