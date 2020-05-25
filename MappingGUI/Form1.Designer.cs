namespace MappingGUI {
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
            this.AddBtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.EntriesLbl = new System.Windows.Forms.Label();
            this.IdTxt = new System.Windows.Forms.TextBox();
            this.IdLbl = new System.Windows.Forms.Label();
            this.ConsumerIdLbl = new System.Windows.Forms.Label();
            this.ConsumerIdTxt = new System.Windows.Forms.TextBox();
            this.ExternalIdTxt = new System.Windows.Forms.TextBox();
            this.ExternalIdLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // AddBtn
            // 
            this.AddBtn.Location = new System.Drawing.Point(671, 393);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(75, 23);
            this.AddBtn.TabIndex = 0;
            this.AddBtn.Text = "Add";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 54);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(427, 362);
            this.dataGridView1.TabIndex = 1;
            // 
            // EntriesLbl
            // 
            this.EntriesLbl.AutoSize = true;
            this.EntriesLbl.Location = new System.Drawing.Point(13, 31);
            this.EntriesLbl.Name = "EntriesLbl";
            this.EntriesLbl.Size = new System.Drawing.Size(52, 17);
            this.EntriesLbl.TabIndex = 2;
            this.EntriesLbl.Text = "Entries";
            // 
            // IdTxt
            // 
            this.IdTxt.Location = new System.Drawing.Point(477, 83);
            this.IdTxt.Name = "IdTxt";
            this.IdTxt.Size = new System.Drawing.Size(269, 22);
            this.IdTxt.TabIndex = 3;
            // 
            // IdLbl
            // 
            this.IdLbl.AutoSize = true;
            this.IdLbl.Location = new System.Drawing.Point(474, 63);
            this.IdLbl.Name = "IdLbl";
            this.IdLbl.Size = new System.Drawing.Size(19, 17);
            this.IdLbl.TabIndex = 4;
            this.IdLbl.Text = "Id";
            // 
            // ConsumerIdLbl
            // 
            this.ConsumerIdLbl.AutoSize = true;
            this.ConsumerIdLbl.Location = new System.Drawing.Point(474, 161);
            this.ConsumerIdLbl.Name = "ConsumerIdLbl";
            this.ConsumerIdLbl.Size = new System.Drawing.Size(83, 17);
            this.ConsumerIdLbl.TabIndex = 5;
            this.ConsumerIdLbl.Text = "ConsumerId";
            // 
            // ConsumerIdTxt
            // 
            this.ConsumerIdTxt.Location = new System.Drawing.Point(477, 181);
            this.ConsumerIdTxt.Name = "ConsumerIdTxt";
            this.ConsumerIdTxt.Size = new System.Drawing.Size(269, 22);
            this.ConsumerIdTxt.TabIndex = 6;
            // 
            // ExternalIdTxt
            // 
            this.ExternalIdTxt.Location = new System.Drawing.Point(477, 270);
            this.ExternalIdTxt.Name = "ExternalIdTxt";
            this.ExternalIdTxt.Size = new System.Drawing.Size(269, 22);
            this.ExternalIdTxt.TabIndex = 7;
            // 
            // ExternalIdLbl
            // 
            this.ExternalIdLbl.AutoSize = true;
            this.ExternalIdLbl.Location = new System.Drawing.Point(474, 250);
            this.ExternalIdLbl.Name = "ExternalIdLbl";
            this.ExternalIdLbl.Size = new System.Drawing.Size(70, 17);
            this.ExternalIdLbl.TabIndex = 8;
            this.ExternalIdLbl.Text = "ExternalId";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ExternalIdLbl);
            this.Controls.Add(this.ExternalIdTxt);
            this.Controls.Add(this.ConsumerIdTxt);
            this.Controls.Add(this.ConsumerIdLbl);
            this.Controls.Add(this.IdLbl);
            this.Controls.Add(this.IdTxt);
            this.Controls.Add(this.EntriesLbl);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.AddBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label EntriesLbl;
        private System.Windows.Forms.TextBox IdTxt;
        private System.Windows.Forms.Label IdLbl;
        private System.Windows.Forms.Label ConsumerIdLbl;
        private System.Windows.Forms.TextBox ConsumerIdTxt;
        private System.Windows.Forms.TextBox ExternalIdTxt;
        private System.Windows.Forms.Label ExternalIdLbl;
    }
}

