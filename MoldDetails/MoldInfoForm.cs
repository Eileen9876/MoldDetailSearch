using System;
using System.Drawing;
using System.Windows.Forms;

namespace MoldDetails
{
    public partial class MoldInfoForm : Form
    {
        private readonly TextBox[] TextBoxes;

        private readonly PictureBox[] PictureBoxes;

        private ProgressTrack Track;

        private DB_Operator DB_OP;

        public MoldInfoForm(DB_Operator DB_OP)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;

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
            
            Track = new ProgressTrack(this);

            this.DB_OP = DB_OP;
        }

        private void add_button_Click(object sender, EventArgs e)
        {
            if (!MainForm.Check_RequiredInput(itemId_textBox))
            {
                MsgBox.Show(this, "請輸入【貨品編號】");
                return;
            }

            try
            {
                Track.Run(() =>
                {
                    if (DB_OP.CheckDataExist(itemId_textBox.Text))
                    {
                        if (!Track.MsgBoxShow("該筆資料已存在，要進行更新嗎？", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)) return;

                        try
                        {
                            DB_OP.UpdateData(itemId_textBox.Text,
                                         MainForm.Get_ColName(TextBoxes),
                                   MainForm.Get_TextBoxValue(TextBoxes),
                                   MainForm.Get_ImageBinaryValue(PictureBoxes));

                            Track.MsgBoxShow("資料更新成功");
                        }
                        catch
                        {
                            Track.MsgBoxShow("資料更新失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            throw;
                        }

                        return;
                    }

                    try
                    {
                        DB_OP.AddData(MainForm.Get_ColName(TextBoxes),
                                  MainForm.Get_TextBoxValue(TextBoxes),
                                  MainForm.Get_ImageBinaryValue(PictureBoxes));

                        Track.MsgBoxShow("資料新增成功");
                    }
                    catch
                    {
                        Track.MsgBoxShow("資料新增失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }

                });
            }
            catch (Exception ex)
            {
                MainForm.Log_Error(ex);
            }

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
    }

}
