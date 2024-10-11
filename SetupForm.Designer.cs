namespace CtrlVolume
{
    partial class SetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.label1 = new System.Windows.Forms.Label();
            this.deviceDropdown = new System.Windows.Forms.ComboBox();
            this.deviceDropDownSave = new System.Windows.Forms.Button();
            this.errorMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(236, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose the secondary audio device for CtrlVolume";
            // 
            // deviceDropdown
            // 
            this.deviceDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceDropdown.FormattingEnabled = true;
            this.deviceDropdown.Location = new System.Drawing.Point(239, 118);
            this.deviceDropdown.Name = "deviceDropdown";
            this.deviceDropdown.Size = new System.Drawing.Size(242, 21);
            this.deviceDropdown.TabIndex = 1;
            this.deviceDropdown.DropDownClosed += new System.EventHandler(this.SetupFormValidate);
            // 
            // deviceDropDownSave
            // 
            this.deviceDropDownSave.Enabled = false;
            this.deviceDropDownSave.Location = new System.Drawing.Point(239, 209);
            this.deviceDropDownSave.Name = "deviceDropDownSave";
            this.deviceDropDownSave.Size = new System.Drawing.Size(242, 23);
            this.deviceDropDownSave.TabIndex = 2;
            this.deviceDropDownSave.Text = "Save";
            this.deviceDropDownSave.UseVisualStyleBackColor = true;
            this.deviceDropDownSave.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SetupFormSaveDevice);
            // 
            // errorMessage
            // 
            this.errorMessage.AutoSize = true;
            this.errorMessage.ForeColor = System.Drawing.Color.Red;
            this.errorMessage.Location = new System.Drawing.Point(209, 268);
            this.errorMessage.MaximumSize = new System.Drawing.Size(300, 0);
            this.errorMessage.MinimumSize = new System.Drawing.Size(300, 100);
            this.errorMessage.Name = "errorMessage";
            this.errorMessage.Size = new System.Drawing.Size(300, 100);
            this.errorMessage.TabIndex = 3;
            this.errorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 450);
            this.Controls.Add(this.errorMessage);
            this.Controls.Add(this.deviceDropDownSave);
            this.Controls.Add(this.deviceDropdown);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupForm";
            this.Text = "Setup CtrlVolume";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox deviceDropdown;
        private System.Windows.Forms.Button deviceDropDownSave;
        private System.Windows.Forms.Label errorMessage;
    }
}