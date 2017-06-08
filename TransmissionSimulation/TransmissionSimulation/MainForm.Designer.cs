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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.transfertBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtReception = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnInject = new System.Windows.Forms.Button();
            this.dataSendGroup = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDataSend = new System.Windows.Forms.RichTextBox();
            this.posError1 = new System.Windows.Forms.NumericUpDown();
            this.posError2 = new System.Windows.Forms.NumericUpDown();
            this.posError3 = new System.Windows.Forms.NumericUpDown();
            this.posError4 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.dataSendGroup.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtReception);
            this.groupBox1.Location = new System.Drawing.Point(613, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 409);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data received";
            // 
            // txtReception
            // 
            this.txtReception.CausesValidation = false;
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
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Transfer progress...";
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
            // dataSendGroup
            // 
            this.dataSendGroup.Controls.Add(this.label3);
            this.dataSendGroup.Controls.Add(this.txtDataSend);
            this.dataSendGroup.Location = new System.Drawing.Point(12, 12);
            this.dataSendGroup.Name = "dataSendGroup";
            this.dataSendGroup.Size = new System.Drawing.Size(464, 409);
            this.dataSendGroup.TabIndex = 4;
            this.dataSendGroup.TabStop = false;
            this.dataSendGroup.Text = "Data sent";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(288, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "ID  | Type |#Ack| Data Lengh";
            // 
            // txtDataSend
            // 
            this.txtDataSend.CausesValidation = false;
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(288, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "ID  | Type |#Ack| Data Lengh";
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
            this.Controls.Add(this.dataSendGroup);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.transfertBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Transmission Simulation. Kappa Team";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.dataSendGroup.ResumeLayout(false);
            this.dataSendGroup.PerformLayout();
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
        private System.Windows.Forms.GroupBox dataSendGroup;
        private System.Windows.Forms.RichTextBox txtDataSend;
        private System.Windows.Forms.NumericUpDown posError1;
        private System.Windows.Forms.NumericUpDown posError2;
        private System.Windows.Forms.NumericUpDown posError3;
        private System.Windows.Forms.NumericUpDown posError4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}