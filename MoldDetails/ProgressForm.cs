using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

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
            Exception exception = null;
            CancellationTokenSource cts = new CancellationTokenSource();

            Task task = Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex) 
                {
                    exception = ex; //record to throw exception outside the task
                    cts.Cancel(); //current task cancel
                }
                finally
                {
                    Close(); //form close
                }
            },
            cts.Token);

            Form.Record_Progress("");

            Form.ShowDialog(this.Control);

            task.Wait();

            if (exception != null) throw exception;
        }

        public void Close()
        {
            if (this.Form.InvokeRequired)
            {
                FormClose close = new FormClose(this.Form.Close);

                this.Control.Invoke(close);

                return;
            }

            this.Form.Close();
        }

        public void SetMsg(string msg)
        {
            if (this.Form.InvokeRequired)
            {
                FormSetMsg set_msg = new FormSetMsg(this.Form.Record_Progress);

                this.Control.Invoke(set_msg, msg);

                return;
            }

            this.Form.Record_Progress(msg);
        }
    }
}

