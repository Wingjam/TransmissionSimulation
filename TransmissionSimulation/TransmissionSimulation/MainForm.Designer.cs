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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnInjectTypeError = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numIrrecoverable = new System.Windows.Forms.NumericUpDown();
            this.numErrorDetectable = new System.Windows.Forms.NumericUpDown();
            this.numErrorCorrectible = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.dataSendGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posError1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError4)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIrrecoverable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numErrorDetectable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numErrorCorrectible)).BeginInit();
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
            this.groupBox1.Location = new System.Drawing.Point(566, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 409);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Données reçues";
            // 
            // txtReception
            // 
            this.txtReception.CausesValidation = false;
            this.txtReception.Location = new System.Drawing.Point(6, 68);
            this.txtReception.Name = "txtReception";
            this.txtReception.ReadOnly = true;
            this.txtReception.Size = new System.Drawing.Size(499, 335);
            this.txtReception.TabIndex = 0;
            this.txtReception.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 424);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Progression du transfert";
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(5, 175);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(120, 23);
            this.btnInject.TabIndex = 5;
            this.btnInject.Text = "Injecter erreur(s)";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // dataSendGroup
            // 
            this.dataSendGroup.Controls.Add(this.label3);
            this.dataSendGroup.Controls.Add(this.txtDataSend);
            this.dataSendGroup.Location = new System.Drawing.Point(12, 12);
            this.dataSendGroup.Name = "dataSendGroup";
            this.dataSendGroup.Size = new System.Drawing.Size(335, 409);
            this.dataSendGroup.TabIndex = 4;
            this.dataSendGroup.TabStop = false;
            this.dataSendGroup.Text = "Données envoyées";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(308, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "ID  | Type |#Ack| Data Lengh |";
            // 
            // txtDataSend
            // 
            this.txtDataSend.CausesValidation = false;
            this.txtDataSend.Location = new System.Drawing.Point(3, 68);
            this.txtDataSend.Name = "txtDataSend";
            this.txtDataSend.ReadOnly = true;
            this.txtDataSend.Size = new System.Drawing.Size(326, 335);
            this.txtDataSend.TabIndex = 0;
            this.txtDataSend.Text = "";
            // 
            // posError1
            // 
            this.posError1.Location = new System.Drawing.Point(5, 71);
            this.posError1.Name = "posError1";
            this.posError1.Size = new System.Drawing.Size(120, 20);
            this.posError1.TabIndex = 6;
            // 
            // posError2
            // 
            this.posError2.Location = new System.Drawing.Point(5, 97);
            this.posError2.Name = "posError2";
            this.posError2.Size = new System.Drawing.Size(120, 20);
            this.posError2.TabIndex = 7;
            // 
            // posError3
            // 
            this.posError3.Location = new System.Drawing.Point(5, 123);
            this.posError3.Name = "posError3";
            this.posError3.Size = new System.Drawing.Size(120, 20);
            this.posError3.TabIndex = 8;
            // 
            // posError4
            // 
            this.posError4.Location = new System.Drawing.Point(5, 149);
            this.posError4.Name = "posError4";
            this.posError4.Size = new System.Drawing.Size(120, 20);
            this.posError4.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 32);
            this.label1.TabIndex = 10;
            this.label1.Text = "Position des erreurs à insérer";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnInject);
            this.groupBox2.Controls.Add(this.posError4);
            this.groupBox2.Controls.Add(this.posError1);
            this.groupBox2.Controls.Add(this.posError3);
            this.groupBox2.Controls.Add(this.posError2);
            this.groupBox2.Location = new System.Drawing.Point(396, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(131, 206);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Insertion avec position";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnInjectTypeError);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.numIrrecoverable);
            this.groupBox3.Controls.Add(this.numErrorDetectable);
            this.groupBox3.Controls.Add(this.numErrorCorrectible);
            this.groupBox3.Location = new System.Drawing.Point(353, 229);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(207, 192);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Insertion aléatoire";
            // 
            // btnInjectTypeError
            // 
            this.btnInjectTypeError.Location = new System.Drawing.Point(48, 163);
            this.btnInjectTypeError.Name = "btnInjectTypeError";
            this.btnInjectTypeError.Size = new System.Drawing.Size(120, 23);
            this.btnInjectTypeError.TabIndex = 11;
            this.btnInjectTypeError.Text = "Injecter erreur(s)";
            this.btnInjectTypeError.UseVisualStyleBackColor = true;
            this.btnInjectTypeError.Click += new System.EventHandler(this.btnInjectTypeError_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(57, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(146, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Erreurs irrécupérables. (3 bits)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(57, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Erreurs détectables. (2 bits)";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(195, 31);
            this.label6.TabIndex = 11;
            this.label6.Text = "Choisir le nombre d\'erreurs de chaque type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(57, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Erreurs corrigibles. (1 bit)";
            // 
            // numIrrecoverable
            // 
            this.numIrrecoverable.Location = new System.Drawing.Point(11, 137);
            this.numIrrecoverable.Name = "numIrrecoverable";
            this.numIrrecoverable.Size = new System.Drawing.Size(40, 20);
            this.numIrrecoverable.TabIndex = 13;
            // 
            // numErrorDetectable
            // 
            this.numErrorDetectable.Location = new System.Drawing.Point(11, 111);
            this.numErrorDetectable.Name = "numErrorDetectable";
            this.numErrorDetectable.Size = new System.Drawing.Size(40, 20);
            this.numErrorDetectable.TabIndex = 12;
            // 
            // numErrorCorrectible
            // 
            this.numErrorCorrectible.Location = new System.Drawing.Point(11, 83);
            this.numErrorCorrectible.Name = "numErrorCorrectible";
            this.numErrorCorrectible.Size = new System.Drawing.Size(40, 20);
            this.numErrorCorrectible.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(378, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "ID  | Type |#Ack| Data Lengh | Erreur";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 472);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.dataSendGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.transfertBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "IFT585 - Télématique - Équipe Kappa";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.dataSendGroup.ResumeLayout(false);
            this.dataSendGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posError1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posError4)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIrrecoverable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numErrorDetectable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numErrorCorrectible)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numIrrecoverable;
        private System.Windows.Forms.NumericUpDown numErrorDetectable;
        private System.Windows.Forms.NumericUpDown numErrorCorrectible;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnInjectTypeError;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
    }
}