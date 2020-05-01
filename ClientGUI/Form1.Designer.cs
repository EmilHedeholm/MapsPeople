namespace ClientGUI {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.userNameLabel = new System.Windows.Forms.Label();
            this.databaseTextBox = new System.Windows.Forms.TextBox();
            this.databaseLabel = new System.Windows.Forms.Label();
            this.databaseListBox = new System.Windows.Forms.ListBox();
            this.dataListLabel = new System.Windows.Forms.Label();
            this.messageLabel = new System.Windows.Forms.Label();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.messageListBox = new System.Windows.Forms.ListBox();
            this.messageBLabel = new System.Windows.Forms.Label();
            this.QueTopicLabel = new System.Windows.Forms.Label();
            this.queTopTextBox = new System.Windows.Forms.TextBox();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.choiceLabel = new System.Windows.Forms.Label();
            this.warningLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // userNameLabel
            // 
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(52, 34);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(83, 20);
            this.userNameLabel.TabIndex = 1;
            this.userNameLabel.Text = "Username";
            // 
            // databaseTextBox
            // 
            this.databaseTextBox.Location = new System.Drawing.Point(141, 86);
            this.databaseTextBox.Name = "databaseTextBox";
            this.databaseTextBox.Size = new System.Drawing.Size(162, 26);
            this.databaseTextBox.TabIndex = 2;
            // 
            // databaseLabel
            // 
            this.databaseLabel.AutoSize = true;
            this.databaseLabel.Location = new System.Drawing.Point(52, 86);
            this.databaseLabel.Name = "databaseLabel";
            this.databaseLabel.Size = new System.Drawing.Size(79, 20);
            this.databaseLabel.TabIndex = 3;
            this.databaseLabel.Text = "Database";
            // 
            // databaseListBox
            // 
            this.databaseListBox.FormattingEnabled = true;
            this.databaseListBox.ItemHeight = 20;
            this.databaseListBox.Location = new System.Drawing.Point(331, 86);
            this.databaseListBox.Name = "databaseListBox";
            this.databaseListBox.Size = new System.Drawing.Size(138, 104);
            this.databaseListBox.TabIndex = 4;
            this.databaseListBox.SelectedIndexChanged += new System.EventHandler(this.databaseListBox_SelectedIndexChanged);
            // 
            // dataListLabel
            // 
            this.dataListLabel.AutoSize = true;
            this.dataListLabel.Location = new System.Drawing.Point(336, 63);
            this.dataListLabel.Name = "dataListLabel";
            this.dataListLabel.Size = new System.Drawing.Size(79, 20);
            this.dataListLabel.TabIndex = 5;
            this.dataListLabel.Text = "Database";
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(16, 239);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(119, 20);
            this.messageLabel.TabIndex = 6;
            this.messageLabel.Text = "Messagebroker";
            // 
            // messageTextBox
            // 
            this.messageTextBox.Location = new System.Drawing.Point(141, 233);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(162, 26);
            this.messageTextBox.TabIndex = 7;
            // 
            // messageListBox
            // 
            this.messageListBox.FormattingEnabled = true;
            this.messageListBox.ItemHeight = 20;
            this.messageListBox.Location = new System.Drawing.Point(331, 233);
            this.messageListBox.Name = "messageListBox";
            this.messageListBox.Size = new System.Drawing.Size(138, 84);
            this.messageListBox.TabIndex = 8;
            this.messageListBox.SelectedIndexChanged += new System.EventHandler(this.messageListBox_SelectedIndexChanged);
            // 
            // messageBLabel
            // 
            this.messageBLabel.AutoSize = true;
            this.messageBLabel.Location = new System.Drawing.Point(327, 210);
            this.messageBLabel.Name = "messageBLabel";
            this.messageBLabel.Size = new System.Drawing.Size(119, 20);
            this.messageBLabel.TabIndex = 9;
            this.messageBLabel.Text = "Messagebroker";
            // 
            // QueTopicLabel
            // 
            this.QueTopicLabel.AutoSize = true;
            this.QueTopicLabel.Location = new System.Drawing.Point(53, 368);
            this.QueTopicLabel.Name = "QueTopicLabel";
            this.QueTopicLabel.Size = new System.Drawing.Size(0, 20);
            this.QueTopicLabel.TabIndex = 10;
            // 
            // queTopTextBox
            // 
            this.queTopTextBox.Location = new System.Drawing.Point(141, 365);
            this.queTopTextBox.Name = "queTopTextBox";
            this.queTopTextBox.Size = new System.Drawing.Size(235, 26);
            this.queTopTextBox.TabIndex = 11;
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(137, 28);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(162, 26);
            this.userNameTextBox.TabIndex = 12;
            // 
            // choiceLabel
            // 
            this.choiceLabel.AutoSize = true;
            this.choiceLabel.Location = new System.Drawing.Point(52, 307);
            this.choiceLabel.Name = "choiceLabel";
            this.choiceLabel.Size = new System.Drawing.Size(0, 20);
            this.choiceLabel.TabIndex = 15;
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.Location = new System.Drawing.Point(68, 433);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(0, 20);
            this.warningLabel.TabIndex = 16;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(331, 401);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(99, 37);
            this.okButton.TabIndex = 17;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 450);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.warningLabel);
            this.Controls.Add(this.choiceLabel);
            this.Controls.Add(this.userNameTextBox);
            this.Controls.Add(this.queTopTextBox);
            this.Controls.Add(this.QueTopicLabel);
            this.Controls.Add(this.messageBLabel);
            this.Controls.Add(this.messageListBox);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.messageLabel);
            this.Controls.Add(this.dataListLabel);
            this.Controls.Add(this.databaseListBox);
            this.Controls.Add(this.databaseLabel);
            this.Controls.Add(this.databaseTextBox);
            this.Controls.Add(this.userNameLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.TextBox databaseTextBox;
        private System.Windows.Forms.Label databaseLabel;
        private System.Windows.Forms.ListBox databaseListBox;
        private System.Windows.Forms.Label dataListLabel;
        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.ListBox messageListBox;
        private System.Windows.Forms.Label messageBLabel;
        private System.Windows.Forms.Label QueTopicLabel;
        private System.Windows.Forms.TextBox queTopTextBox;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label choiceLabel;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.Button okButton;
    }
}

