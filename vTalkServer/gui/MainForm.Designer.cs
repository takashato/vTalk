namespace vTalkServer
{
    partial class mainForm
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
            this.switchButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // switchButton
            // 
            this.switchButton.Location = new System.Drawing.Point(13, 13);
            this.switchButton.Name = "switchButton";
            this.switchButton.Size = new System.Drawing.Size(96, 58);
            this.switchButton.TabIndex = 0;
            this.switchButton.Text = "START";
            this.switchButton.UseVisualStyleBackColor = true;
            this.switchButton.Click += new System.EventHandler(this.switchButton_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.switchButton);
            this.Name = "mainForm";
            this.Text = "vTalk Server";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button switchButton;
    }
}

