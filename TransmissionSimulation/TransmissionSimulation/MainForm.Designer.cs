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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtReception = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnInject = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDataSend = new System.Windows.Forms.RichTextBox();
            this.posError1 = new System.Windows.Forms.NumericUpDown();
            this.posError2 = new System.Windows.Forms.NumericUpDown();
            this.posError3 = new System.Windows.Forms.NumericUpDown();
            this.posError4 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posError1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError4)).BeginInit();
            this.SuspendLayout();
            // 
            // transfertBar
            // 
            this.transfertBar.Location = new System.Drawing.Point(12, 440);
            this.transfertBar.Name = "transfertBar";
            this.transfertBar.Size = new System.Drawing.Size(1065, 23);
            this.transfertBar.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtReception);
            this.groupBox1.Location = new System.Drawing.Point(613, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 409);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data received";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(122, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Data";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(82, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "isNak";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(43, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "isAck";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 52);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(18, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "ID";
            // 
            // txtReception
            // 
            this.txtReception.Location = new System.Drawing.Point(6, 68);
            this.txtReception.Name = "txtReception";
            this.txtReception.ReadOnly = true;
            this.txtReception.Size = new System.Drawing.Size(452, 335);
            this.txtReception.TabIndex = 0;
            this.txtReception.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 424);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Transfert progress...";
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(482, 208);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(120, 23);
            this.btnInject.TabIndex = 5;
            this.btnInject.Text = "Inject Error(s)";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtDataSend);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(464, 409);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data send";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(122, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Data";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(82, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "isNak";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "isAck";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "ID";
            // 
            // txtDataSend
            // 
            this.txtDataSend.Location = new System.Drawing.Point(3, 68);
            this.txtDataSend.Name = "txtDataSend";
            this.txtDataSend.ReadOnly = true;
            this.txtDataSend.Size = new System.Drawing.Size(452, 335);
            this.txtDataSend.TabIndex = 0;
            this.txtDataSend.Text = "";
            // 
            // posError1
            // 
            this.posError1.Location = new System.Drawing.Point(482, 104);
            this.posError1.Name = "posError1";
            this.posError1.Size = new System.Drawing.Size(120, 20);
            this.posError1.TabIndex = 6;
            // 
            // posError2
            // 
            this.posError2.Location = new System.Drawing.Point(482, 130);
            this.posError2.Name = "posError2";
            this.posError2.Size = new System.Drawing.Size(120, 20);
            this.posError2.TabIndex = 7;
            // 
            // posError3
            // 
            this.posError3.Location = new System.Drawing.Point(482, 156);
            this.posError3.Name = "posError3";
            this.posError3.Size = new System.Drawing.Size(120, 20);
            this.posError3.TabIndex = 8;
            // 
            // posError4
            // 
            this.posError4.Location = new System.Drawing.Point(482, 182);
            this.posError4.Name = "posError4";
            this.posError4.Size = new System.Drawing.Size(120, 20);
            this.posError4.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(483, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 32);
            this.label1.TabIndex = 10;
            this.label1.Text = "Position(s) of errors to inject.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 475);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.posError4);
            this.Controls.Add(this.posError3);
            this.Controls.Add(this.posError2);
            this.Controls.Add(this.posError1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.transfertBar);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posError1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar transfertBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.RichTextBox txtReception;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox txtDataSend;
        private System.Windows.Forms.NumericUpDown posError1;
        private System.Windows.Forms.NumericUpDown posError2;
        private System.Windows.Forms.NumericUpDown posError3;
        private System.Windows.Forms.NumericUpDown posError4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}