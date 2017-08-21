namespace HelperLibrary.Loading
{
    partial class InitialApplicationLoader
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
            this.pnl_app = new System.Windows.Forms.Panel();
            this.pb_close = new System.Windows.Forms.PictureBox();
            this.lbl_application_title = new System.Windows.Forms.Label();
            this.lbl_loading_description = new System.Windows.Forms.Label();
            this.lbl_window_title = new System.Windows.Forms.Label();
            this.pgb_loading = new System.Windows.Forms.ProgressBar();
            this.lbl_loading_max = new System.Windows.Forms.Label();
            this.lbl_loading_current = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnl_app.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_close)).BeginInit();
            this.SuspendLayout();
            // 
            // pnl_app
            // 
            this.pnl_app.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl_app.Controls.Add(this.lbl_window_title);
            this.pnl_app.Controls.Add(this.pb_close);
            this.pnl_app.Location = new System.Drawing.Point(0, 0);
            this.pnl_app.Name = "pnl_app";
            this.pnl_app.Size = new System.Drawing.Size(432, 25);
            this.pnl_app.TabIndex = 12;
            this.pnl_app.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnl_app_MouseDown);
            // 
            // pb_close
            // 
            this.pb_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_close.Image = global::HelperLibrary.Properties.Resources.close;
            this.pb_close.Location = new System.Drawing.Point(398, 0);
            this.pb_close.Name = "pb_close";
            this.pb_close.Size = new System.Drawing.Size(34, 25);
            this.pb_close.TabIndex = 0;
            this.pb_close.TabStop = false;
            this.pb_close.Click += new System.EventHandler(this.pb_close_Click);
            this.pb_close.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pb_close_MouseDown);
            this.pb_close.MouseEnter += new System.EventHandler(this.pb_close_MouseEnter);
            this.pb_close.MouseLeave += new System.EventHandler(this.pb_close_MouseLeave);
            this.pb_close.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pb_close_MouseUp);
            // 
            // lbl_application_title
            // 
            this.lbl_application_title.AutoSize = true;
            this.lbl_application_title.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_application_title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.lbl_application_title.Location = new System.Drawing.Point(9, 117);
            this.lbl_application_title.Name = "lbl_application_title";
            this.lbl_application_title.Size = new System.Drawing.Size(263, 37);
            this.lbl_application_title.TabIndex = 17;
            this.lbl_application_title.Text = "Application Title 123";
            // 
            // lbl_loading_description
            // 
            this.lbl_loading_description.AutoSize = true;
            this.lbl_loading_description.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lbl_loading_description.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.lbl_loading_description.Location = new System.Drawing.Point(12, 168);
            this.lbl_loading_description.Name = "lbl_loading_description";
            this.lbl_loading_description.Size = new System.Drawing.Size(196, 19);
            this.lbl_loading_description.TabIndex = 18;
            this.lbl_loading_description.Text = "Initalize Database connection...";
            // 
            // lbl_window_title
            // 
            this.lbl_window_title.AutoSize = true;
            this.lbl_window_title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.lbl_window_title.Location = new System.Drawing.Point(4, 6);
            this.lbl_window_title.Name = "lbl_window_title";
            this.lbl_window_title.Size = new System.Drawing.Size(187, 13);
            this.lbl_window_title.TabIndex = 6;
            this.lbl_window_title.Text = "ApplicationTitle | Initializing Application";
            // 
            // pgb_loading
            // 
            this.pgb_loading.Location = new System.Drawing.Point(0, 190);
            this.pgb_loading.Name = "pgb_loading";
            this.pgb_loading.Size = new System.Drawing.Size(432, 14);
            this.pgb_loading.TabIndex = 19;
            // 
            // lbl_loading_max
            // 
            this.lbl_loading_max.AutoSize = true;
            this.lbl_loading_max.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lbl_loading_max.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.lbl_loading_max.Location = new System.Drawing.Point(407, 171);
            this.lbl_loading_max.Name = "lbl_loading_max";
            this.lbl_loading_max.Size = new System.Drawing.Size(13, 13);
            this.lbl_loading_max.TabIndex = 20;
            this.lbl_loading_max.Text = "0";
            // 
            // lbl_loading_current
            // 
            this.lbl_loading_current.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_loading_current.AutoSize = true;
            this.lbl_loading_current.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lbl_loading_current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.lbl_loading_current.Location = new System.Drawing.Point(379, 171);
            this.lbl_loading_current.Name = "lbl_loading_current";
            this.lbl_loading_current.Size = new System.Drawing.Size(13, 13);
            this.lbl_loading_current.TabIndex = 21;
            this.lbl_loading_current.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.label2.Location = new System.Drawing.Point(399, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "/";
            // 
            // InitialApplicationLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(432, 204);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_loading_current);
            this.Controls.Add(this.lbl_loading_max);
            this.Controls.Add(this.pgb_loading);
            this.Controls.Add(this.pnl_app);
            this.Controls.Add(this.lbl_loading_description);
            this.Controls.Add(this.lbl_application_title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InitialApplicationLoader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InitialApplicationLoader";
            this.Load += new System.EventHandler(this.InitialApplicationLoader_Load);
            this.pnl_app.ResumeLayout(false);
            this.pnl_app.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_close)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnl_app;
        private System.Windows.Forms.Label lbl_window_title;
        private System.Windows.Forms.PictureBox pb_close;
        private System.Windows.Forms.Label lbl_application_title;
        private System.Windows.Forms.Label lbl_loading_description;
        private System.Windows.Forms.ProgressBar pgb_loading;
        private System.Windows.Forms.Label lbl_loading_max;
        private System.Windows.Forms.Label lbl_loading_current;
        private System.Windows.Forms.Label label2;
    }
}