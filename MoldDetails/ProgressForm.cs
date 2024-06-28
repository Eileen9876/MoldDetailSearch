using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoldDetails
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;

            label.Text = "";
        }

        public void Record_Progress(string msg)
        {
            label.Text = msg;
        }
    }

    public class ProgressTrack
    {
        private delegate void FormClose();

        private delegate void FormSetMsg(string msg);

        private delegate bool BoxShow(Control control);

        public delegate void TaskAction();

        private Control Control;

        private ProgressForm Form;

        public ProgressTrack(Control control)
        {
            this.Control = control;
            this.Form = new ProgressForm();
        }

        ~ProgressTrack() 
        {
            this.Control = null;
            this.Form.Dispose();
            this.Form = null; 
        }

        public void Run(TaskAction action)
        {
            try
            {
                Task task = new Task(() => 
                { 
                    action();

                    Close();
                });

                task.Start();

                Form.Record_Progress("");

                Form.ShowDialog(this.Control);

                task.Wait();
            }
            catch
            {
                throw;
            }
        }

        public void Close()
        {
            FormClose close = new FormClose(this.Form.Close);
            this.Control.Invoke(close);
        }

        public void SetMsg(string msg)
        {
            FormSetMsg set_msg = new FormSetMsg(this.Form.Record_Progress);
            this.Control.Invoke(set_msg, msg);
        }

        public bool MsgBoxShow(string msg, MessageBoxButtons btn = MessageBoxButtons.OK,  MessageBoxIcon icon = MessageBoxIcon.None)
        {
            BoxShow box_show = new BoxShow((control) =>
            {
                return MsgBox.Show(control, msg, btn, icon);
            });

            return (bool)this.Control.Invoke(box_show, this.Form);
        }
    }
}

