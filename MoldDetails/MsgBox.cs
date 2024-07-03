using System;
using System.Drawing;
using System.Windows.Forms;

namespace MoldDetails
{
    public partial class MsgBox : Form
    {
        private delegate void DelShowErr(Control control, string msg, Exception ex);

        private delegate bool DelShow(Control control, string msg, MessageBoxButtons btn = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None);

        public static void ShowErr(Control control, string msg, Exception ex)
        {
            if (control.InvokeRequired)
            {
                DelShowErr fun = new DelShowErr(ShowErr);

                control.Invoke(fun, control, msg, ex);

                return;
            }

            MsgBox box = new MsgBox();

            msg = $"{msg}\r\n\r\n錯誤訊息：{ex.Message}";

            box.Initial(msg, MessageBoxButtons.OK, MessageBoxIcon.Error);

            box.ShowDialog(control);

            box.Dispose();
        }

        public static bool Show(Control control, string msg, MessageBoxButtons btn = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            if (control.InvokeRequired)
            {
                DelShow fun = new DelShow(Show);

                return (bool)control.Invoke(fun, control, msg, btn, icon);
            }

            MsgBox box = new MsgBox();

            box.Initial(msg, btn, icon);

            box.ShowDialog(control);

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

            int width = label.Width + 50 + ((pictureBox.Image != null) ? pictureBox.Width + 10 : 0);

            int height = label.Height + 180;

            this.Size = new Size((this.Width < width) ? width : this.Width, height);
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
