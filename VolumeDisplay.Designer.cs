namespace CtrlVolume
{
    partial class VolumeDisplay
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
            this.volumeBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // volumeBar
            // 
            this.volumeBar.Location = new System.Drawing.Point(12, 12);
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Size = new System.Drawing.Size(776, 23);
            this.volumeBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.volumeBar.TabIndex = 0;
            // 
            // VolumeDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 42);
            this.Controls.Add(this.volumeBar);
            this.Name = "VolumeDisplay";
            this.Text = "VolumeDisplay";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar volumeBar;
    }
}