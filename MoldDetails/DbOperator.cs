using System.Collections.Generic;
using System.Data;
using MyLib;

namespace MoldDetails
{
    public static class DbOperator
    {
        private static readonly string Table = "moldInfo";

        private static readonly string Primary_Column = "itemId";

        private static readonly string[] Columns = {
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

        private static readonly string[] ALL_COL = new string[] { "*" };

        public static void ConnectDB(DBHandler handler, string db_file_path)
        {
            handler.Connect(db_file_path);
        }

        public static bool CheckDataExist(DBHandler handler, string primary_key_val)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";
            DataTable dt = handler.Select(Table, ALL_COL, condition);
            return dt.Rows.Count != 0;
        }

        
        public static void AddData(DBHandler handler, string[] col, string[] val, List<byte[]> img_list)
        {
            Parameter param = new Parameter();
            param.Type = System.Data.OleDb.OleDbType.Binary;

            for (int i = 0; i < img_list.Count; i++) 
            {
                if (img_list[i] == null) continue;
                param.Count++;
                param.Columns.Add("img" + (i+1).ToString());
                param.Values.Add(img_list[i]);
            }

            if (param.Count == 0) handler.Insert(Table, col, val);
            else handler.Insert(Table, col, val, param);
        }

        public static void UpdateData(DBHandler handler, string primary_key_val, string[] col, string[] val, List<byte[]> img_list)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";

            Parameter param = new Parameter();
            param.Type = System.Data.OleDb.OleDbType.Binary;

            for (int i = 0; i < img_list.Count; i++)
            {
                if (img_list[i] == null) continue;
                param.Count++;
                param.Columns.Add("img" + (i + 1).ToString());
                param.Values.Add(img_list[i]);
            }

            if (param.Count == 0) handler.Update(Table, col, val, condition);
            else handler.Update(Table, col, val, condition, param);
        }

        public static void DeleteData(DBHandler handler, string primary_key_val)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";
            handler.Delete(Table, condition);
        }

        public static DataTable SearchData(DBHandler handler, string col, string val)
        {
            string condition = col + "= '" + val + "'";
            return handler.Select(Table, ALL_COL, condition);
        }

        public static DataTable GetAllData(DBHandler handler)
        {
            return handler.Select(Table, ALL_COL);
        }
    }
}
