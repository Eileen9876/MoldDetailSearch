using System.Collections.Generic;
using System.Data;
using MyLib;

namespace MoldDetails
{
    public class DB_Operator
    {
        private readonly DBHandler Handler;

        private readonly string[] ALL_COL;

        public string Table;

        public string[] Columns;

        public string Primary_Column;

        public DB_Operator()
        {
            Handler = new DBHandler();

            ALL_COL = new string[] { "*" };

            Table = "moldInfo";

            Columns = new string[]{
                "moldId", "itemId", "itemName",
                "corId", "corNum", "corComp",
                "cavId", "cavNum", "cavComp",
                "texPitch", "texMaxDia", "texMinDia",
                "orgPrice", "fivePrice", "tenPrice", "thirtyPrice",
                "machine", "toGW", "toNW", "toCavNum", "toSprue",
                "quotNW", "quotSprue", "quotGW",
                "clientNW", "clientSprue", "clientGW", "clientCons",
                "notes", "img1", "img2"
            };

            Primary_Column = "itemId";
        }

        public void ConnectDB(string db_file_path)
        {
            Handler.Connect(db_file_path);
        }

        public bool CheckDataExist(string primary_key_val)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";
            DataTable dt = Handler.Select(Table, ALL_COL, condition);
            return dt.Rows.Count != 0;
        }

        public void AddData(string[] col, string[] val, List<byte[]> img_list)
        {
            Parameter param = new Parameter();
            param.type = System.Data.OleDb.OleDbType.Binary;

            for (int i = 0; i < img_list.Count; i++) 
            {
                if (img_list[i] == null) continue;
                param.count++;
                param.col.Add("img" + (i+1).ToString());
                param.val.Add(img_list[i]);
            }

            if (param.count == 0) Handler.Insert(Table, col, val);
            else Handler.Insert(Table, col, val, param);
        }

        public void UpdateData(string primary_key_val, string[] col, string[] val, List<byte[]> img_list)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";

            Parameter param = new Parameter();
            param.type = System.Data.OleDb.OleDbType.Binary;

            for (int i = 0; i < img_list.Count; i++)
            {
                if (img_list[i] == null) continue;
                param.count++;
                param.col.Add("img" + (i + 1).ToString());
                param.val.Add(img_list[i]);
            }

            if (param.count == 0) Handler.Update(Table, col, val, condition);
            else Handler.Update(Table, col, val, condition, param);
        }

        public void DeleteData(string primary_key_val)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";
            Handler.Delete(Table, condition);
        }

        public DataTable SearchData(string col, string val)
        {
            string condition = col + "= '" + val + "'";
            return Handler.Select(Table, ALL_COL, condition);
        }

        public DataTable GetAllData()
        {
            return Handler.Select(Table, ALL_COL);
        }
    }
}
