using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace MoldDetails
{
    public partial class DatabaseForm : Form
    {
        private const string FilePath = @"data\DatabaseFilePath.txt";
        private List<string> FilePath_List = new List<string>();
        public string Db_FilePath;

        public DatabaseForm()
        {
            InitializeComponent();

            Get_Path();

            Initialize_ListView();

            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void Get_Path()
        {
            bool start_add_data_in_list = false;    // 確認是否開始新增資料至陣列

            foreach (string line in File.ReadLines(FilePath))
            {
                if (string.IsNullOrEmpty(line))
                {
                    start_add_data_in_list = true;
                    continue;
                }

                if (!start_add_data_in_list)
                {
                    usedFile_textBox.Text = line;
                    Db_FilePath = line;
                    continue;
                }

                FilePath_List.Add(line);
            }
        }

        private void Initialize_ListView()
        {
            this.listView.GridLines = true;
            this.listView.View = View.Details;
            this.listView.Scrollable = true;
            this.listView.FullRowSelect = true;

            InitialItem();
        }

        private void InitialItem()
        {
            this.listView.BeginUpdate();

            this.listView.Columns.Add("檔案名稱", -2, HorizontalAlignment.Left);
            this.listView.Columns.Add("檔案路徑", -2, HorizontalAlignment.Left);

            foreach (string line in FilePath_List)
            {
                ListViewItem item = new ListViewItem(Path.GetFileName(line));
                item.SubItems.Add(line);
                this.listView.Items.Add(item);
            }

            this.listView.EndUpdate();
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            string file_path = Choose_File();
            if (file_path == "") return;

            ListViewItem item = new ListViewItem(Path.GetFileName(file_path));
            item.SubItems.Add(file_path);
            this.listView.Items.Add(item);
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
                this.listView.Items.Remove(this.listView.SelectedItems[0]);
        }

        private void edit_button_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                string file_path = Choose_File();
                if (file_path == "") return;

                ListViewItem item = this.listView.SelectedItems[0];
                item.SubItems[0].Text = Path.GetFileName(file_path);
                item.SubItems[1].Text = file_path;
            }
        }

        private void setup_button_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
                usedFile_textBox.Text = this.listView.SelectedItems[0].SubItems[1].Text;
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                writer.WriteLine(usedFile_textBox.Text);
                writer.WriteLine();
                foreach (ListViewItem item in listView.Items) writer.WriteLine(item.SubItems[1].Text);
            }

            Db_FilePath = usedFile_textBox.Text;

            this.Close();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string Choose_File()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "請選擇要使用的資料庫檔案",
                Filter = "|*.accdb"
            };

            if (dialog.ShowDialog() == DialogResult.OK) return dialog.FileName;

            return "";
        }
    }
}
