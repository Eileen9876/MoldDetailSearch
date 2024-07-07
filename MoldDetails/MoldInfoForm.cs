using System;
using System.Drawing;
using System.Windows.Forms;
using MyLib;

namespace MoldDetails
{
    public partial class MoldInfoForm : Form
    {
        private readonly TextBox[] TextBoxes;

        private readonly PictureBox[] PictureBoxes;

        private DBHandler DbHandler;

        public MoldInfoForm(DBHandler DbHandler)
        {
            InitializeComponent();

            this.DbHandler = DbHandler;

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

            PictureBoxes = new PictureBox[] { img1_pictureBox, img2_pictureBox };
        }

        public void Show(Control control)
        {
            this.StartPosition = FormStartPosition.Manual;

            this.Location = new Point(
                control.Location.X + (control.Width - this.Width) / 2,
                control.Location.Y + (control.Height - this.Height) / 2
            );

            this.Show();
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            if (!MainForm.Check_RequiredInput(itemId_textBox))
            {
                MsgBox.Show(this, "請輸入【貨品編號】");
                return;
            }

            ProgressTrack track = ProgressTrack.Run(this, () =>
            {
                // 資料已存在，更新資料
                if (DbOperator.CheckDataExist(DbHandler, itemId_textBox.Text))
                {
                    if (!MsgBox.Show(this, 
                                     "該筆資料已存在，要進行更新嗎？", 
                                     MessageBoxButtons.OKCancel, 
                                     MessageBoxIcon.Warning)) return;

                    DbOperator.UpdateData(DbHandler,
                                          itemId_textBox.Text,
                                          MainForm.Get_ColName(TextBoxes),
                                          MainForm.Get_TextBoxValue(TextBoxes),
                                          MainForm.Get_ImageBinaryValue(PictureBoxes));

                    return;
                }

                // 新增資料
                DbOperator.AddData(DbHandler, 
                                   MainForm.Get_ColName(TextBoxes),
                                   MainForm.Get_TextBoxValue(TextBoxes),
                                   MainForm.Get_ImageBinaryValue(PictureBoxes));
            });

            ResultMsgShow_And_ErrLog("執行成功", "執行失敗", track.GetException);
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            foreach (TextBox box in TextBoxes) box.Text = "";
            foreach (PictureBox box in PictureBoxes) box.Image = null;
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

        private void img1_clearBtn_Click(object sender, EventArgs e)
        {
            img1_pictureBox.Image = null;
        }

        private void img2_clearBtn_Click(object sender, EventArgs e)
        {
            img2_pictureBox.Image = null;
        }

        private void ResultMsgShow_And_ErrLog(string success_msg, string error_msg, Exception ex)
        {
            if (ex == null && success_msg.Length > 0) MsgBox.Show(this, success_msg);
            else
            {
                MainForm.Log_Error(ex);
                MsgBox.ShowErr(this, error_msg, ex);
            }
        }
    }
}
