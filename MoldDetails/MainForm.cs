using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Linq;
using MyLib;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MoldDetails
{
    public partial class MainForm : Form
    {
        private readonly static string EXCEL_SAMPLE_FILE_PATH = @"assets\sample.xlsx";

        private readonly static string ERR_LOG_FILE_PATH = @"data\ErrLog.txt";

        private readonly static string DataBase_FILE_PATH = @"data\DatabaseFilePath.txt";

        private readonly TextBox[] TextBoxes;

        private readonly CheckBox[] CheckBoxes;

        private readonly RadioButton[] RadioButtons;

        private readonly PictureBox[] PictureBoxes;

        private DB_Operator DB_OP;

        private DataGridViewInfo ViewInfo; 

        private delegate Exception ListRow(DataRow row); // 多執行緒使用

        private delegate Exception ListTable(DataTable table); // 多執行緒使用

        public MainForm()
        {
            InitializeComponent();

            InitializeDataGridView();

            InitializeDatabase();

            AdjustFormSize();

            TextBoxes = new TextBox[]{
                moldId_textBox, itemId_textBox, itemName_textBox,
                corId_textBox, corNum_textBox, corComp_textBox,
                cavId_textBox, cavNum_textBox, cavComp_textBox,
                texPitch_textBox, texMaxDia_textBox, texMinDia_textBox,
                orgPrice_textBox, fivePrice_textBox, tenPrice_textBox, thirtyPrice_textBox,
                machine_textBox, toGW_textBox, toNW_textBox, toCavNum_textBox, toSprue_textBox,
                quotNW_textBox, quotSprue_textBox, quotGW_textBox,
                clientNW_textBox, clientSprue_textBox, clientGW_textBox, clientCons_textBox,
                notes_textBox
            };

            CheckBoxes = new CheckBox[]
            {
                moldId_checkBox,
                itemId_checkBox,
                itemName_checkBox,
                core_checkBox,
                cavity_checkBox,
                textureSize_checkBox,
                unPrice_checkBox,
                tryoutWT_checkBox,
                quoteWT_checkBox,
                clientWT_checkBox,
                notes_checkBox
            };

            RadioButtons = new RadioButton[] 
            { 
                moldId_radioBtn,
                itemId_radioBtn,
                itemName_radioBtn,
                corId_radioBtn,
                corNum_radioBtn,
                cavId_radioBtn,
                cavNum_radioBtn,
                texPitch_radioBtn,
                texMaxDia_radioBtn,
                texMinDia_radioBtn,
                machine_radioBtn
            };

            PictureBoxes = new PictureBox[] { img1_pictureBox, img2_pictureBox };

            itemId_textBox.Enabled = false;

            itemId_radioBtn.Checked = true;

            itemId_checkBox.Enabled = false;

            itemId_checkBox.Checked = true;

            Enable_Button(false);
        }

        private void InitializeDataGridView()
        {
            ViewInfo.HeaderChinese = new List<KeyValuePair<string, string[]>>
            {
                new KeyValuePair<string, string[]>("模具編號", null),
                new KeyValuePair<string, string[]>("貨品編號", null),
                new KeyValuePair<string, string[]>("對銷編號", null),
                new KeyValuePair<string, string[]>("公模仁", new string[] { "編號", "模仁", "配件" }),
                new KeyValuePair<string, string[]>("母模仁", new string[] { "編號", "模仁", "配件" }),
                new KeyValuePair<string, string[]>("咬牙", new string[] { "螺距", "大徑", "小徑" }),
                new KeyValuePair<string, string[]>("單價", new string[] { "原價", "5K", "10K", "30K" }),
                new KeyValuePair<string, string[]>("試模重量", new string[] { "機台", "總毛重", "單淨重", "穴數", "總料頭" }),
                new KeyValuePair<string, string[]>("報價重量", new string[] { "淨重", "料頭", "毛重" }),
                new KeyValuePair<string, string[]>("報客戶重量", new string[] { "淨重", "料頭", "毛重", "消耗量" }),
                new KeyValuePair<string, string[]>("備註", null)
            };

            ViewInfo.HeaderEnglish = new List<KeyValuePair<string, string[]>>
            {
                new KeyValuePair<string, string[]>("moldId", null),
                new KeyValuePair<string, string[]>("itemId", null),
                new KeyValuePair<string, string[]>("itemName", null),
                new KeyValuePair<string, string[]>("core", new string[] { "corId", "corNum", "corComp" }),
                new KeyValuePair<string, string[]>("cavity", new string[] { "cavId", "cavNum", "cavComp" }),
                new KeyValuePair<string, string[]>("textureSize", new string[] { "texPitch", "texMaxDia", "texMinDia" }),
                new KeyValuePair<string, string[]>("unPrice", new string[] { "orgPrice", "fivePrice", "tenPrice", "thirtyPrice" }),
                new KeyValuePair<string, string[]>("tryoutWT", new string[] { "machine", "toGW", "toNW", "toCavNum", "toSprue" }),
                new KeyValuePair<string, string[]>("quoteWT", new string[] { "quotNW", "quotSprue", "quotGW" }),
                new KeyValuePair<string, string[]>("clientWT", new string[] { "clientNW", "clientSprue", "clientGW", "clientCons" }),
                new KeyValuePair<string, string[]>("notes", null)
            };

            ViewInfo.OP = new DataGridView_Operator(this.dataGridView);
        }

        private void InitializeDatabase()
        {
            // 資料庫檔案位置
            string file_path = "";

            FileInfo file_info = new FileInfo(DataBase_FILE_PATH);
            if (file_info.Length == 0)
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "請選擇要使用的資料庫檔案",
                    Filter = "|*.accdb"
                };

                if (dialog.ShowDialog() != DialogResult.OK) 
                {
                    this.Dispose();
                    return;
                }

                file_path = dialog.FileName;

                File.WriteAllLines(DataBase_FILE_PATH, new string[] { file_path + "\r\n\r\n" + file_path });
            }
            else file_path = File.ReadLines(DataBase_FILE_PATH).First();
 
            // 資料庫操作
            DB_OP = new DB_Operator();
            DB_OP.ConnectDB(file_path);
        }

        private void AdjustFormSize()
        {
            this.Height = SystemInformation.WorkingArea.Height;
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            MoldInfoForm form = new MoldInfoForm(DB_OP);

            try 
            { 
                form.ShowDialog(); 
            }
            catch (Exception ex) 
            { 
                Log_Error(ex);
            }
            finally 
            { 
                form.Dispose(); 
            }
        }

        private void update_button_Click(object sender, EventArgs e)
        {
            if (!MsgBox.Show(this, "確認更新該筆資料?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)) return;

            ProgressTrack track = ProgressTrack.Run(this, () =>
            {
                DB_OP.UpdateData(itemId_textBox.Text, 
                                Get_ColName(TextBoxes), 
                                Get_TextBoxValue(TextBoxes),
                                Get_ImageBinaryValue(PictureBoxes));
            });

            ResultMsgShow_And_ErrLog("資料更新成功", "資料更新失敗", track.GetException);
        }

        private void search_button_Click(object sender, EventArgs e)
        {
            if (!Check_RequiredInput(search_textBox))
            {
                MsgBox.Show(this, "請輸入查詢值");
                return;
            }

            ProgressTrack track = new ProgressTrack(this);

            track.Run(() =>
            {
                foreach (RadioButton btn in RadioButtons)
                {
                    if (!btn.Checked) continue;

                    track.SetMsg("查詢中");
                    DataTable table = DB_OP.SearchData(btn.Name.Split('_')[0], search_textBox.Text);
                    if (table.Rows.Count == 0)
                    {
                        MsgBox.Show(this, "查無該筆資料");
                        return;
                    }

                    // 假如查詢條件為「貨品編號」則顯示於上方 Textbox，否則顯示於下方的 DataGridView。
                    track.SetMsg("查詢完畢，將資料顯示於頁面中");
                    Exception ex;
                    if (btn.Name == DB_OP.Primary_Column + "_radioBtn")
                    {
                        ex = Invoke_ListInTextbox(table.Rows[0]);
                    }
                    else
                    {
                        ViewInfo.Table = table.Copy();

                        ex = Invoke_ListInView(table);
                    }

                    if (ex != null) throw ex;

                    return;
                }
            });

            ResultMsgShow_And_ErrLog("", "資料查詢失敗", track.GetException);
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            if (!DB_OP.CheckDataExist(itemId_textBox.Text))
            {
                MsgBox.Show(this, "查無資料");
                return;
            }

            if (!MsgBox.Show(this, "確認刪除該筆資料?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)) return;

            ProgressTrack track = ProgressTrack.Run(this, () =>
            {
                DB_OP.DeleteData(itemId_textBox.Text);
            });

            ResultMsgShow_And_ErrLog("資料刪除成功", "資料刪除失敗", track.GetException);
        }

        private void export_all_button_Click(object sender, EventArgs e)
        {
            string file_path = Get_FileSavePath();
            if (file_path == "") return;

            string file_name = "模具明細表" + DateTime.Now.ToString("MMddHHmmss") + ".xlsx";

            string target_file = file_path + file_name;

            ProgressTrack track = new ProgressTrack(this);

            track.Run(() =>
            {
                track.SetMsg("獲取資料");
                DataTable table = DB_OP.GetAllData();
                if (table.Rows.Count == 0)
                {
                    MsgBox.Show(this, "無資料");
                    return;
                }

                track.SetMsg("範本檔複製");
                File.Copy(EXCEL_SAMPLE_FILE_PATH, target_file);

                track.SetMsg("寫入檔案");
                Write_In_Excel(table.Copy(), DB_OP.Columns, target_file);
            });

            ResultMsgShow_And_ErrLog("檔案匯出成功，檔名為【" + file_name + "】", "檔案匯出失敗", track.GetException);
        }

        private void export_DataGridView_button_Click(object sender, EventArgs e)
        {
            if (ViewInfo.Table == null || ViewInfo.Table.Rows.Count == 0)
            {
                MsgBox.Show(this, "無資料");
                return;
            }

            string file_path = Get_FileSavePath();
            if (file_path == "") return;

            string file_name = "模具明細表" + DateTime.Now.ToString("MMddHHmmss") + ".xlsx";

            string target_file = file_path + file_name;

            ProgressTrack track = new ProgressTrack(this);

            track.Run(() =>
            {
                track.SetMsg("範本檔複製");
                File.Copy(EXCEL_SAMPLE_FILE_PATH, target_file);

                track.SetMsg("寫入檔案");
                Write_In_Excel(ViewInfo.Table.Copy(), DB_OP.Columns, target_file);
            });

            ResultMsgShow_And_ErrLog("檔案匯出成功，檔名為【" + file_name + "】", "檔案匯出失敗", track.GetException);
        }

        private void dbSetup_MenuItem_Click(object sender, EventArgs e)
        {
            DatabaseForm form = new DatabaseForm();

            string current_filepath = form.Db_FilePath;

            try
            {
                form.ShowDialog();

                if (current_filepath != form.Db_FilePath)
                {
                    DB_OP.ConnectDB(form.Db_FilePath);

                    MsgBox.Show(this, "資料庫更換成功");
                }
            }
            catch (Exception ex)
            {
                Log_Error(ex);

                DB_OP.ConnectDB(current_filepath);

                MsgBox.ShowErr(this, "資料庫更換失敗", ex);
            }
            finally 
            { 
                form.Dispose(); 
            }
        }

        private void errLog_MenuItem_Click(object sender, EventArgs e)
        {
            LogForm form = new LogForm();
            try 
            { 
                form.ShowDialog(); 
            }
            catch (Exception ex) 
            {
                Log_Error(ex);

                MsgBox.ShowErr(this, "開啟失敗", ex);
            }
            finally 
            { 
                form.Dispose(); 
            }
        }

        private void img1_chooseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "請選擇圖片",
                Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png;)|*.jpg; *.jpeg; *.gif; *.bmp; *.png"
            };

            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName.Length > 0)
            {
                img1_pictureBox.Image = new Bitmap(dialog.FileName);
            }
        }

        private void img1_clearBtn_Click(object sender, EventArgs e)
        {
            img1_pictureBox.Image = null;
        }
        
        private void img2_chooseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "請選擇圖片";
            dialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png;)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName.Length > 0)
            {
                img2_pictureBox.Image = new Bitmap(dialog.FileName);
            }
        }

        private void img2_clearBtn_Click(object sender, EventArgs e)
        {
            img2_pictureBox.Image = null;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            if (row == -1) return;

            List_In_Textbox(ViewInfo.Table.Rows[row]);
        }

        private void ResultMsgShow_And_ErrLog(string success_msg, string error_msg, Exception ex)
        {
            if (ex == null && success_msg.Length > 0) MsgBox.Show(this, success_msg);
            else if (ex != null)
            {
                Log_Error(ex);
                MsgBox.ShowErr(this, error_msg, ex);
            }
        }

        private void itemId_textBox_TextChanged(object sender, EventArgs e)
        {
            if (itemId_textBox.Text == "")
            {
                Enable_Button(false);
                return;
            }

            Enable_Button(true);
        }

        private Exception Invoke_ListInTextbox(DataRow row)
        {
            if (this.InvokeRequired)
            {
                ListRow fun = new ListRow(Invoke_ListInTextbox);

                return (Exception)this.Invoke(fun, row);
            }

            try
            {
                List_In_Textbox(row);
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }


        private void List_In_Textbox(DataRow row)
        {
            Clear_AllTextBoxValue();

            foreach (TextBox box in TextBoxes)
            {
                string col_name = box.Name.Split('_')[0];
                box.Text = row[col_name].ToString();
            }

            foreach (PictureBox box in PictureBoxes)
            {
                string col_name = box.Name.Split('_')[0];
                byte[] binary = row[col_name] as byte[];
                if (binary == null) continue;
                box.Image = BinaryToImage(binary);
            }
        }

        private Exception Invoke_ListInView(DataTable table)
        {
            if (this.InvokeRequired)
            {
                ListTable fun = new ListTable(Invoke_ListInView);

                return (Exception)this.Invoke(fun, table);
            }

            try
            {
                List_In_DataGridView(table);
            }
            catch (Exception ex) 
            {
                return ex;
            }

            return null;
        }

        private void List_In_DataGridView(DataTable table)
        {
            table.Columns.Remove("img1");
            table.Columns.Remove("img2");

            List<KeyValuePair<string, string[]>> header = new List<KeyValuePair<string, string[]>>(); //中文標頭資訊
            List<string> table_col = new List<string>(); //將 table columns 進行排序用

            for (int i = 0; i < CheckBoxes.Length; i++)
            {
                if (CheckBoxes[i].Checked)
                {
                    // 加入中文標頭
                    header.Add(ViewInfo.HeaderChinese[i]);

                    // 加入對應參數 table 的標頭名稱，用來排序。
                    if (ViewInfo.HeaderEnglish[i].Value == null) table_col.Add(ViewInfo.HeaderEnglish[i].Key);
                    else foreach (string name in ViewInfo.HeaderEnglish[i].Value) table_col.Add(name);

                    continue;
                }

                // 移除不用顯示的資訊
                if (ViewInfo.HeaderEnglish[i].Value == null)
                    table.Columns.Remove(ViewInfo.HeaderEnglish[i].Key);
                else
                    foreach (string name in ViewInfo.HeaderEnglish[i].Value) table.Columns.Remove(name);
            }

            ViewInfo.OP.Set_Data(header, table, table_col);
            ViewInfo.OP.Set_Data(header, table, table_col); // 標頭對齊需要，目前還沒找到原因
        }

        private void Clear_AllTextBoxValue()
        {
            foreach (TextBox box in TextBoxes) box.Text = "";
            img1_pictureBox.Image = null;
            img2_pictureBox.Image = null;
        }

        private void Enable_Button(bool enable)
        {
            update_button.Enabled = enable;
            delete_button.Enabled = enable;

            img1_chooseBtn.Visible = enable;
            img1_chooseBtn.Enabled = enable;
            img1_clearBtn.Visible = enable;
            img1_clearBtn.Enabled = enable;
            img2_chooseBtn.Visible = enable;
            img2_chooseBtn.Enabled = enable;
            img2_clearBtn.Visible = enable;
            img2_clearBtn.Enabled = enable;
        }

        private void Write_In_Excel(DataTable table,string[] table_columns, string fileName)
        {
            Excel.Application app = new Excel.Application(); //應用程序
            Excel.Workbook wb = null; //檔案
            Excel.Worksheet ws = null; //工作表
            try
            {
                wb = app.Workbooks.Open(fileName);
                ws = wb.Worksheets[1];

                int columns_count = table_columns.Length - 2; // -2 去除 img1 與 img2
                int r = 3;
                foreach (DataRow dr in table.Rows)
                {
                    int c = 1;
                    for(int i = 0; i < columns_count; i++)
                    {
                        ws.Cells[r, c] = dr[table_columns[i]];
                        c++;
                    }
                    r++;
                }

                wb.Save();
            }
            catch
            {
                throw;
            }
            finally
            {
                ws = null;
                wb.Close();
                wb = null;
                app.Quit();
                app = null;
            }
        }

        private static string Get_FileSavePath()
        {
            FolderBrowserDialog folder_dialog = new FolderBrowserDialog
            {
                Description = "請選擇要儲存的位置"
            };

            if (folder_dialog.ShowDialog() != DialogResult.OK) return "";

            return folder_dialog.SelectedPath + "\\";
        }

        public static void Log_Error(Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(ERR_LOG_FILE_PATH))
            {
                writer.WriteLine($"{DateTime.Now}: {ex}");
            }
        }

        public static string[] Get_ColName(TextBox[] boxes)
        {
            string[] col = new string[boxes.Length];

            for (int i = 0; i < boxes.Length; i++) col[i] = boxes[i].Name.Split('_')[0];

            return col;
        }

        public static string[] Get_TextBoxValue(TextBox[] boxes)
        {
            string[] val = new string[boxes.Length];

            for(int i = 0; i < boxes.Length; i++) val[i] = boxes[i].Text;

            return val;
        }

        public static List<byte[]> Get_ImageBinaryValue(PictureBox[] boxes)
        {
            List<byte[]> img_list = new List<byte[]>();

            foreach (PictureBox box in boxes) img_list.Add((box.Image == null)? null : ImageToBinary(box.Image));

            return img_list;
        }

        public static bool Check_RequiredInput(Control control)
        {
            return control.Text != "";
        }

        public static Image BinaryToImage(byte[] binary)
        {
            MemoryStream stream = new MemoryStream(binary);
            Image image;

            try { image = Image.FromStream(stream); }
            catch { throw; }
            finally { stream.Dispose(); }

            return image;
        }

        public static byte[] ImageToBinary(Image image)
        {
            MemoryStream stream = new MemoryStream();
            byte[] binary;

            try
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                binary = stream.ToArray();
            }
            catch { throw; }
            finally { stream.Dispose(); }

            return binary;
        }
    }

    struct DataGridViewInfo
    {
        public List<KeyValuePair<string, string[]>> HeaderChinese;

        public List<KeyValuePair<string, string[]>> HeaderEnglish;

        public DataGridView_Operator OP;

        public DataTable Table;
    }
}
