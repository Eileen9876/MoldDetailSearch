using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace MyLib
{
    struct Merge_Info
    {
        public string title;
        public Rectangle rec;
    }

    public class DataGridView_Operator
    {
        private readonly DataGridView view;
        private List<Merge_Info> merge_info_list = new List<Merge_Info>(); // 主標頭資訊
        private int offset;  // 滾動滾輪後相對於起始點的偏移量

        public DataGridView_Operator(DataGridView view)
        {
            this.view = view;

            this.view.Scroll += (sender, e) => 
            {
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll) return;

                offset = e.NewValue;
                this.view.Invalidate();
            };

            this.view.Paint += (sender, e) =>
            {
                foreach (Merge_Info info in merge_info_list) Draw(e, info.rec, info.title);
            };

            Set_HeaderProperties();

            Optimize_Paint();
        }

        private void Set_HeaderProperties()
        {
            this.view.RowHeadersVisible = false;
            this.view.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.view.ColumnHeadersHeight *= 3;
            this.view.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
        }

        private void Optimize_Paint()
        {
            Type type = this.view.GetType();
            PropertyInfo info = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            info.SetValue(this.view, true, null);
        }

        /// <param name="header">DataGridView 的標頭</param>
        /// <param name="table">要寫入 DataGridView 的資料</param>
        /// <param name="table_col">參數 table 的欄位名稱，用於排序 DataGridView</param>
        public void Set_Data(List<KeyValuePair<string, string[]>> header, DataTable table, List<string> table_col)
        {
            merge_info_list.Clear();
            offset = 0;

            this.view.DataSource = table;

            Sort_Column(table_col);

            Set_Header(header);

            Cancel_SortMode();
        }

        public void Sort_Column(List<string> col)
        {
            // 調整 DataGridView 的資料顯示順序
            for(int i = 0; i < col.Count; i++) this.view.Columns[col[i]].DisplayIndex = i;
        }

        private void Set_Header(List<KeyValuePair<string, string[]>> header)
        {
            // 設置主標頭資訊於 merge_info_list 用於後續繪製標頭
            // 副標頭直接寫入 DataGridView

            int rec_x = 0;
            int rec_height = this.view.ColumnHeadersHeight;
            int idx = 0;
            foreach (KeyValuePair<string, string[]> pair in header)
            {
                this.view.Columns[idx].HeaderText = (pair.Value == null) ? pair.Key : pair.Value[0]; 

                Merge_Info info = new Merge_Info  // 主標頭資訊設置
                {
                    title = pair.Key, // 主標頭文字
                    rec = new Rectangle
                    {
                        X = rec_x,
                        Y = 0,
                        Width = this.view.Columns[idx].Width,
                        Height = rec_height
                    }
                };              

                idx++;

                if (pair.Value != null) 
                {
                    info.rec.Height /= 2;
                    for (int i = 1; i < pair.Value.Length; i++)
                    {
                        this.view.Columns[idx].HeaderCell.Value = pair.Value[i]; // 副標頭文字設置
                        info.rec.Width += this.view.Columns[idx].Width; // 主標頭寬度設置
                        idx++;
                    }
                }

                merge_info_list.Add(info);
                rec_x += info.rec.Width;
            }
        }

        private void Draw(PaintEventArgs e, Rectangle rec, string title)
        {
            rec.X -= offset; // 滾動滾輪後的偏移位置
            using (Brush back = new SolidBrush(this.view.ColumnHeadersDefaultCellStyle.BackColor))
            using (Brush fore = new SolidBrush(this.view.ColumnHeadersDefaultCellStyle.ForeColor))
            using (Pen p = new Pen(this.view.GridColor))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                e.Graphics.FillRectangle(back, rec);
                e.Graphics.DrawRectangle(p, rec);
                e.Graphics.DrawString(title, this.view.ColumnHeadersDefaultCellStyle.Font, fore, rec, format);
            }
        }

        private void Cancel_SortMode()
        {
            foreach (DataGridViewColumn col in this.view.Columns) col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }
    }
}

/* 
 */