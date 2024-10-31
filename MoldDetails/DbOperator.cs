using System.Collections.Generic;
using System.Data;
using MyLib;

namespace MoldDetails
{
    public static class DbOperator
    {
        private static readonly string Table = "moldInfo";

        private static readonly string ImageTable = "moldImg";

        private static readonly string Primary_Column = "itemId";

        private static readonly string[] TableColumns = {
                "moldId", "itemId", "rawMaterial", "itemName", "moldingTime",
                "corId", "corNum", "corComp",
                "cavId", "cavNum", "cavComp",
                "texPitch", "texMaxDia", "texMinDia",
                "texPitch2", "texMaxDia2", "texMinDia2",
                "orgPrice", "fivePrice", "tenPrice", "thirtyPrice", "fiftyPrice",
                "machine", "toGW", "toNW", "toCavNum", "toSprue",
                "quotNW", "quotSprue", "quotGW",
                "clientNW", "clientSprue", "clientGW", "clientCons",
                "notes"
        };

        private static readonly string[] ImageTableColumns = {
                "img1", "img2"
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

            // 寫入模具資訊
            handler.Insert(Table, col, val);

            // 寫入模具圖片
            if (param.Count != 0) handler.Insert(ImageTable, null, null, param);
        }

        public static void UpdateData(DBHandler handler, string primary_key_val, string[] col, string[] val, List<byte[]> img_list)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";

            Parameter param = new Parameter();
            param.Type = System.Data.OleDb.OleDbType.Binary;

            for (int i = 0; i < img_list.Count; i++)
            {
                param.Count++;
                param.Columns.Add("img" + (i + 1).ToString());
                param.Values.Add(img_list[i]);
            }

            // 寫入模具資訊
            handler.Update(Table, col, val, condition);

            // 寫入模具圖片
            if (param.Count != 0) handler.Update(ImageTable, null, null, condition, param);
        }

        public static void DeleteData(DBHandler handler, string primary_key_val)
        {
            string condition = Primary_Column + "= '" + primary_key_val + "'";
            handler.Delete(Table, condition);
            handler.Delete(ImageTable, condition);
        }

        /// <summary>
        /// 根據條件在指定的資料表中查詢資料。
        /// </summary>
        /// <param name="handler">資料庫操作的處理器物件，用於執行 SQL 查詢。</param>
        /// <param name="col">查詢條件的欄位。</param>
        /// <param name="val">查詢條件的值，用於匹配指定欄位的內容。</param>
        /// <returns>包含查詢結果的 <see cref="DataTable"/> 物件。</returns>
        public static DataTable SearchData(DBHandler handler, string col, string val)
        {
            SelectObject object1 = new SelectObject(Table, TableColumns);
            SelectObject object2 = new SelectObject(ImageTable, ImageTableColumns);
            string joinCondition = $"{ImageTable}.{Primary_Column} = {Table}.{Primary_Column}";
            string selectCondition = $"{Table}.{col} = '{val}'";

            return handler.Select2Table(object1, object2, joinCondition, selectCondition);
        }

        public static DataTable GetAllData(DBHandler handler)
        {
            SelectObject object1 = new SelectObject(Table, TableColumns);
            SelectObject object2 = new SelectObject(ImageTable, ImageTableColumns);
            string joinCondition = $"{ImageTable}.{Primary_Column} = {Table}.{Primary_Column}";

            return handler.Select2Table(object1, object2, joinCondition);
        }
    }
}
