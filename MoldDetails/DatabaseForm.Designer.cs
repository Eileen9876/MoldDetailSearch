namespace MoldDetails
{
    partial class DatabaseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.add_button = new System.Windows.Forms.Button();
            this.delete_button = new System.Windows.Forms.Button();
            this.edit_button = new System.Windows.Forms.Button();
            this.ok_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.setup_button = new System.Windows.Forms.Button();
            this.usedFile_textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(14, 15);
            this.listView.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(709, 282);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // add_button
            // 
            this.add_button.Location = new System.Drawing.Point(738, 15);
            this.add_button.Name = "add_button";
            this.add_button.Size = new System.Drawing.Size(122, 37);
            this.add_button.TabIndex = 1;
            this.add_button.Text = "新增";
            this.add_button.UseVisualStyleBackColor = true;
            this.add_button.Click += new System.EventHandler(this.add_button_Click);
            // 
            // delete_button
            // 
            this.delete_button.Location = new System.Drawing.Point(738, 68);
            this.delete_button.Name = "delete_button";
            this.delete_button.Size = new System.Drawing.Size(122, 37);
            this.delete_button.TabIndex = 2;
            this.delete_button.Text = "刪除";
            this.delete_button.UseVisualStyleBackColor = true;
            this.delete_button.Click += new System.EventHandler(this.delete_button_Click);
            // 
            // edit_button
            // 
            this.edit_button.Location = new System.Drawing.Point(738, 121);
            this.edit_button.Name = "edit_button";
            this.edit_button.Size = new System.Drawing.Size(122, 37);
            this.edit_button.TabIndex = 3;
            this.edit_button.Text = "編輯";
            this.edit_button.UseVisualStyleBackColor = true;
            this.edit_button.Click += new System.EventHandler(this.edit_button_Click);
            // 
            // ok_button
            // 
            this.ok_button.Location = new System.Drawing.Point(601, 359);
            this.ok_button.Name = "ok_button";
            this.ok_button.Size = new System.Drawing.Size(122, 37);
            this.ok_button.TabIndex = 4;
            this.ok_button.Text = "確認";
            this.ok_button.UseVisualStyleBackColor = true;
            this.ok_button.Click += new System.EventHandler(this.ok_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(738, 359);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(122, 37);
            this.cancel_button.TabIndex = 5;
            this.cancel_button.Text = "取消";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // setup_button
            // 
            this.setup_button.Location = new System.Drawing.Point(14, 359);
            this.setup_button.Name = "setup_button";
            this.setup_button.Size = new System.Drawing.Size(369, 37);
            this.setup_button.TabIndex = 7;
            this.setup_button.Text = "設置為目前要使用的資料庫檔案";
            this.setup_button.UseVisualStyleBackColor = true;
            this.setup_button.Click += new System.EventHandler(this.setup_button_Click);
            // 
            // usedFile_textBox
            // 
            this.usedFile_textBox.Enabled = false;
            this.usedFile_textBox.Location = new System.Drawing.Point(14, 306);
            this.usedFile_textBox.Name = "usedFile_textBox";
            this.usedFile_textBox.Size = new System.Drawing.Size(709, 38);
            this.usedFile_textBox.TabIndex = 8;
            // 
            // DatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 413);
            this.Controls.Add(this.usedFile_textBox);
            this.Controls.Add(this.setup_button);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.ok_button);
            this.Controls.Add(this.edit_button);
            this.Controls.Add(this.delete_button);
            this.Controls.Add(this.add_button);
            this.Controls.Add(this.listView);
            this.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "DatabaseForm";
            this.Text = "資料庫路徑設置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Button add_button;
        private System.Windows.Forms.Button delete_button;
        private System.Windows.Forms.Button edit_button;
        private System.Windows.Forms.Button ok_button;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.Button setup_button;
        private System.Windows.Forms.TextBox usedFile_textBox;
    }
}