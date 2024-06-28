using System;
using System.IO;
using System.Windows.Forms;

namespace MoldDetails
{
    public partial class LogForm : Form
    {
        private const string FilePath = @"data\ErrLog.txt";

        public LogForm()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            string content = File.ReadAllText(FilePath);
            textBox.Text = content;
        }
    }
}
