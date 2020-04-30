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
            this.MessageTextBox = new System.Windows.Forms.TextBox();
            this.messageListBox = new System.Windows.Forms.ListBox();
            this.messageBLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.queTextBox = new System.Windows.Forms.TextBox();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.topicTextBox = new System.Windows.Forms.TextBox();
            this.topicLabel = new System.Windows.Forms.Label();
            this.choiceLabel = new System.Windows.Forms.Label();
            this.warningLabel = new System.Windows.Forms.Label();
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
            // MessageTextBox
            // 
            this.MessageTextBox.Location = new System.Drawing.Point(141, 233);
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(162, 26);
            this.MessageTextBox.TabIndex = 7;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 394);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Queue ID";
            // 
            // queTextBox
            // 
            this.queTextBox.Location = new System.Drawing.Point(136, 388);
            this.queTextBox.Name = "queTextBox";
            this.queTextBox.Size = new System.Drawing.Size(235, 26);
            this.queTextBox.TabIndex = 11;
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(137, 28);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(162, 26);
            this.userNameTextBox.TabIndex = 12;
            // 
            // topicTextBox
            // 
            this.topicTextBox.Location = new System.Drawing.Point(136, 340);
            this.topicTextBox.Name = "topicTextBox";
            this.topicTextBox.Size = new System.Drawing.Size(235, 26);
            this.topicTextBox.TabIndex = 13;
            // 
            // topicLabel
            // 
            this.topicLabel.AutoSize = true;
            this.topicLabel.Location = new System.Drawing.Point(52, 346);
            this.topicLabel.Name = "topicLabel";
            this.topicLabel.Size = new System.Drawing.Size(47, 20);
            this.topicLabel.TabIndex = 14;
            this.topicLabel.Text = "Topic";
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 450);
            this.Controls.Add(this.warningLabel);
            this.Controls.Add(this.choiceLabel);
            this.Controls.Add(this.topicLabel);
            this.Controls.Add(this.topicTextBox);
            this.Controls.Add(this.userNameTextBox);
            this.Controls.Add(this.queTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.messageBLabel);
            this.Controls.Add(this.messageListBox);
            this.Controls.Add(this.MessageTextBox);
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
        private System.Windows.Forms.TextBox MessageTextBox;
        private System.Windows.Forms.ListBox messageListBox;
        private System.Windows.Forms.Label messageBLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox queTextBox;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.TextBox topicTextBox;
        private System.Windows.Forms.Label topicLabel;
        private System.Windows.Forms.Label choiceLabel;
        private System.Windows.Forms.Label warningLabel;
    }
}

