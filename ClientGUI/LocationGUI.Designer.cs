namespace ClientGUI {
    partial class LocationGUI {
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
            this.locationListBox = new System.Windows.Forms.ListBox();
            this.locationLabel = new System.Windows.Forms.Label();
            this.sourceListBox = new System.Windows.Forms.ListBox();
            this.sourceLabel = new System.Windows.Forms.Label();
            this.stateDataGridView = new System.Windows.Forms.DataGridView();
            this.stateLabel = new System.Windows.Forms.Label();
            this.warningLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.stateDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // locationListBox
            // 
            this.locationListBox.FormattingEnabled = true;
            this.locationListBox.ItemHeight = 20;
            this.locationListBox.Location = new System.Drawing.Point(32, 36);
            this.locationListBox.Name = "locationListBox";
            this.locationListBox.Size = new System.Drawing.Size(177, 384);
            this.locationListBox.TabIndex = 0;
            this.locationListBox.SelectedIndexChanged += new System.EventHandler(this.locationListBox_SelectedIndexChanged);
            // 
            // locationLabel
            // 
            this.locationLabel.AutoSize = true;
            this.locationLabel.Location = new System.Drawing.Point(28, 13);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.Size = new System.Drawing.Size(107, 20);
            this.locationLabel.TabIndex = 1;
            this.locationLabel.Text = "Locations List";
            // 
            // sourceListBox
            // 
            this.sourceListBox.FormattingEnabled = true;
            this.sourceListBox.ItemHeight = 20;
            this.sourceListBox.Location = new System.Drawing.Point(257, 36);
            this.sourceListBox.Name = "sourceListBox";
            this.sourceListBox.Size = new System.Drawing.Size(133, 244);
            this.sourceListBox.TabIndex = 2;
            // 
            // sourceLabel
            // 
            this.sourceLabel.AutoSize = true;
            this.sourceLabel.Location = new System.Drawing.Point(253, 13);
            this.sourceLabel.Name = "sourceLabel";
            this.sourceLabel.Size = new System.Drawing.Size(97, 20);
            this.sourceLabel.TabIndex = 3;
            this.sourceLabel.Text = "Sources List";
            // 
            // stateDataGridView
            // 
            this.stateDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stateDataGridView.Location = new System.Drawing.Point(449, 76);
            this.stateDataGridView.Name = "stateDataGridView";
            this.stateDataGridView.RowHeadersWidth = 62;
            this.stateDataGridView.RowTemplate.Height = 28;
            this.stateDataGridView.Size = new System.Drawing.Size(328, 204);
            this.stateDataGridView.TabIndex = 4;
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Location = new System.Drawing.Point(463, 36);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(91, 20);
            this.stateLabel.TabIndex = 5;
            this.stateLabel.Text = "State Table";
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.Location = new System.Drawing.Point(309, 400);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(0, 20);
            this.warningLabel.TabIndex = 6;
            // 
            // LocationGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.warningLabel);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.stateDataGridView);
            this.Controls.Add(this.sourceLabel);
            this.Controls.Add(this.sourceListBox);
            this.Controls.Add(this.locationLabel);
            this.Controls.Add(this.locationListBox);
            this.Name = "LocationGUI";
            this.Text = "LocationForm";
            this.Load += new System.EventHandler(this.LocationGUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.stateDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox locationListBox;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.ListBox sourceListBox;
        private System.Windows.Forms.Label sourceLabel;
        private System.Windows.Forms.DataGridView stateDataGridView;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.Label warningLabel;
    }
}