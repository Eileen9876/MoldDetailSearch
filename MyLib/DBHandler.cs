using System.Data.OleDb;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System;

namespace MyLib
{
    /// <summary>
    /// OLE 物件欄位與值，一個 Parameter 只能儲存一種 OLE 物件
    /// </summary>
    public class Parameter
    {
        public OleDbType Type = OleDbType.Empty;
        public int Count = 0;
        public List<string> Columns = new List<string>(); 
        public List<object> Values = new List<object>();
    }

    public class SelectObject
    {
        public string Table;
        public string[] Columns;
        public SelectObject(string table, string[] columns) { Table = table; Columns = columns;}
    }

    public class DBHandler
    {
        private OleDbConnection sqlConn = null;

        ~DBHandler()
        {
            sqlConn?.Dispose();
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

        /// <summary>
        /// Constructs an SQL SELECT query to retrieve specific columns from two tables, 
        /// joining them based on a specified condition.
        /// </summary>
        /// <param name="selectObject1">An object containing the name of the first table and the columns to be selected.</param>
        /// <param name="selectObject2">An object containing the name of the second table and the columns to be selected.</param>
        private OleDbCommand Create_Select2TableCommand(SelectObject selectObject1, SelectObject selectObject2, string joinCondition, string selectCondition = "")
        {
            // SELECT table.column1, table.column2, ..., table2.column1, table2.column2, ...
            // FROM table
            // INNER JOIN table2 ON table.id = table2.id
            // WHERE condition;

            StringBuilder query = new StringBuilder($"SELECT {selectObject1.Table}.{selectObject1.Columns[0]}");

            for (int i = 1; i < selectObject1.Columns.Length; i++)
            {
                query.Append($", {selectObject1.Table}.{selectObject1.Columns[i]}");
            }

            for (int i = 0; i < selectObject2.Columns.Length; i++)
            {
                query.Append($", {selectObject2.Table}.{selectObject2.Columns[i]}");
            }

            query.Append($" FROM {selectObject1.Table}");

            query.Append($" INNER JOIN {selectObject2.Table} ON {joinCondition}");

            query.Append((selectCondition == "") ? ";" : $" WHERE {selectCondition};");

            Console.WriteLine(query.ToString());

            return new OleDbCommand(query.ToString(), this.sqlConn);
        }

        private OleDbCommand Create_InsertCommand(string table_name, string[] col, string[] val, Parameter param = null)
        {
            // INSERT INTO table_name (column1, column2, column3, ... , param_column1, param_column2)
            // VALUES (value1, value2, value3, ... , @param_column1, @param_column2);

            StringBuilder query = new StringBuilder($"INSERT INTO {table_name} (");

            query.Append((col == null || col.Length == 0) ? "" : string.Join(", ", col));

            query.Append((param == null) ? "" : 
                ((col == null || col.Length == 0) ? "" : ", ") + string.Join(", ", param.Columns));

            query.Append(") VALUES (");

            query.Append((col == null || col.Length == 0) ? "" : string.Join(", ", val.Select(v => $"'{v}'")));

            query.Append((param == null) ? "" : 
                ((col == null || col.Length == 0) ? "" : ", ") + string.Join(", ", param.Columns.Select(c => $"@{c}")));

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

            query.Append((param == null) ? "" : ((col == null || col.Length == 0) ? "" : ", ") + string.Join(", ", param.Columns.Select(c => $"{c} = @{c}")));

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
            for (int i = 0; i < param.Count; i++)
                cmd.Parameters.Add(param.Columns[i], param.Type).Value = param.Values[i] ?? System.DBNull.Value;
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

        public DataTable Select2Table(SelectObject object1, SelectObject object2, string joinCondition, string selectCondition = "")
        {
            OleDbCommand cmd = Create_Select2TableCommand(object1, object2, joinCondition, selectCondition);
            Execute(cmd);

            OleDbDataAdapter dp = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            dp.Fill(dt);

            return dt;
        }

        /// <param name="col">表單"純文字"欄位，不包含"純文字"欄位(參數)對應的欄位</param>
        /// <param name="val">表單"純文字"欄位對應的值</param>
        /// <param name="param">"非純文字"欄位(參數)對應的欄位與值</param>
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
