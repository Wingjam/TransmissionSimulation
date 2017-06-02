namespace TransmissionSimulation
{
    partial class MainForm
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
            this.transfertBar = new System.Windows.Forms.ProgressBar();
            this.nbErrors = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnInject = new System.Windows.Forms.Button();
            this.txtReception = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nbErrors)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // transfertBar
            // 
            this.transfertBar.Location = new System.Drawing.Point(12, 404);
            this.transfertBar.Name = "transfertBar";
            this.transfertBar.Size = new System.Drawing.Size(1065, 23);
            this.transfertBar.TabIndex = 0;
            // 
            // nbErrors
            // 
            this.nbErrors.Location = new System.Drawing.Point(526, 180);
            this.nbErrors.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nbErrors.Name = "nbErrors";
            this.nbErrors.ReadOnly = true;
            this.nbErrors.Size = new System.Drawing.Size(34, 20);
            this.nbErrors.TabIndex = 1;
            this.nbErrors.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(482, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Number of errors to inject";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtReception);
            this.groupBox1.Location = new System.Drawing.Point(613, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 360);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data received";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 388);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Transfert progress...";
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(487, 207);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(120, 23);
            this.btnInject.TabIndex = 5;
            this.btnInject.Text = "Inject Error(s)";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // txtReception
            // 
            this.txtReception.Location = new System.Drawing.Point(6, 19);
            this.txtReception.Name = "txtReception";
            this.txtReception.ReadOnly = true;
            this.txtReception.Size = new System.Drawing.Size(452, 335);
            this.txtReception.TabIndex = 0;
            this.txtReception.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(464, 360);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data send";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(6, 19);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(452, 335);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 448);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nbErrors);
            this.Controls.Add(this.transfertBar);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nbErrors)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar transfertBar;
        private System.Windows.Forms.NumericUpDown nbErrors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.RichTextBox txtReception;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}