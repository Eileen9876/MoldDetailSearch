using System;
using System.Drawing;
using System.Windows.Forms;

namespace MoldDetails
{
    public partial class MsgBox : Form
    {
        public static bool Show(IWin32Window window, string msg, MessageBoxButtons btn = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MsgBox box = new MsgBox();

            box.Initial(msg, btn, icon);

            box.ShowDialog(window);

            bool result = (box.DialogResult == DialogResult.OK);

            box.Dispose();

            return result;
        }

        private MsgBox()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;

            label.AutoSize = true;

            ok_button.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            cancel_button.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void Initial(string msg, MessageBoxButtons btn, MessageBoxIcon icon)
        {
            switch (btn)
            {
                case MessageBoxButtons.OK:
                    Use_OkBtn();
                    break;

                case MessageBoxButtons.OKCancel:
                    Use_OkBtn();
                    Use_CancelBtn();
                    break;

                default:
                    break;
            }

            Set_Icon(icon);

            Set_Msg(msg);
        }

        private void Set_Icon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.None:
                    pictureBox.Visible = false;
                    label.Location = new Point(pictureBox.Location.X, label.Location.Y);
                    return;

                case MessageBoxIcon.Warning:
                    pictureBox.Image = SystemIcons.Warning.ToBitmap();
                    return;

                case MessageBoxIcon.Error:
                    pictureBox.Image = SystemIcons.Error.ToBitmap();
                    return;

                default:
                    return;
            }
        }

        private void Set_Msg(string msg)
        {
            label.Text = msg;

            int width = label.Width + 50;

            if (pictureBox.Image != null) width += pictureBox.Width + 10;

            if (this.Width < width) this.Size = new Size(width, 180);
        }

        private void Use_OkBtn()
        {
            ok_button.Enabled = true;
            ok_button.Visible = true;
        }

        private void Use_CancelBtn()
        {
            cancel_button.Enabled = true;
            cancel_button.Visible = true;
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
