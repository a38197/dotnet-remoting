namespace SuperSoftware.Client
{
    partial class ClientForm
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSearchFamily = new System.Windows.Forms.Button();
            this.txtSearchFamily = new System.Windows.Forms.TextBox();
            this.lbRequests = new System.Windows.Forms.ListBox();
            this.lbFamilies = new System.Windows.Forms.ListBox();
            this.lbProducts = new System.Windows.Forms.ListBox();
            this.lbRemoteManagers = new System.Windows.Forms.ListBox();
            this.btnDisconect = new System.Windows.Forms.Button();
            this.lbRemoteProd = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 9);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Ligar";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSearchFamily
            // 
            this.btnSearchFamily.Location = new System.Drawing.Point(247, 24);
            this.btnSearchFamily.Name = "btnSearchFamily";
            this.btnSearchFamily.Size = new System.Drawing.Size(107, 23);
            this.btnSearchFamily.TabIndex = 1;
            this.btnSearchFamily.Text = "Pesquisa Familia";
            this.btnSearchFamily.UseVisualStyleBackColor = true;
            this.btnSearchFamily.Click += new System.EventHandler(this.btnSearchFamily_Click);
            // 
            // txtSearchFamily
            // 
            this.txtSearchFamily.Location = new System.Drawing.Point(6, 26);
            this.txtSearchFamily.Name = "txtSearchFamily";
            this.txtSearchFamily.Size = new System.Drawing.Size(239, 20);
            this.txtSearchFamily.TabIndex = 2;
            // 
            // lbRequests
            // 
            this.lbRequests.FormattingEnabled = true;
            this.lbRequests.Location = new System.Drawing.Point(12, 316);
            this.lbRequests.Name = "lbRequests";
            this.lbRequests.Size = new System.Drawing.Size(743, 134);
            this.lbRequests.TabIndex = 6;
            // 
            // lbFamilies
            // 
            this.lbFamilies.FormattingEnabled = true;
            this.lbFamilies.Location = new System.Drawing.Point(6, 19);
            this.lbFamilies.Name = "lbFamilies";
            this.lbFamilies.Size = new System.Drawing.Size(118, 238);
            this.lbFamilies.TabIndex = 3;
            this.lbFamilies.SelectedIndexChanged += new System.EventHandler(this.lbFamilies_SelectedIndexChanged);
            // 
            // lbProducts
            // 
            this.lbProducts.FormattingEnabled = true;
            this.lbProducts.Location = new System.Drawing.Point(130, 19);
            this.lbProducts.Name = "lbProducts";
            this.lbProducts.Size = new System.Drawing.Size(241, 238);
            this.lbProducts.TabIndex = 4;
            this.lbProducts.SelectedIndexChanged += new System.EventHandler(this.lbProducts_SelectedIndexChanged);
            // 
            // lbRemoteManagers
            // 
            this.lbRemoteManagers.FormattingEnabled = true;
            this.lbRemoteManagers.Location = new System.Drawing.Point(6, 58);
            this.lbRemoteManagers.Name = "lbRemoteManagers";
            this.lbRemoteManagers.Size = new System.Drawing.Size(144, 199);
            this.lbRemoteManagers.TabIndex = 9;
            this.lbRemoteManagers.SelectedIndexChanged += new System.EventHandler(this.lbRemoteManagers_SelectedIndexChanged);
            // 
            // btnDisconect
            // 
            this.btnDisconect.Location = new System.Drawing.Point(93, 9);
            this.btnDisconect.Name = "btnDisconect";
            this.btnDisconect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconect.TabIndex = 12;
            this.btnDisconect.Text = "Desligar";
            this.btnDisconect.UseVisualStyleBackColor = true;
            this.btnDisconect.Click += new System.EventHandler(this.btnDisconect_Click);
            // 
            // lbRemoteProd
            // 
            this.lbRemoteProd.FormattingEnabled = true;
            this.lbRemoteProd.Location = new System.Drawing.Point(156, 58);
            this.lbRemoteProd.Name = "lbRemoteProd";
            this.lbRemoteProd.Size = new System.Drawing.Size(198, 199);
            this.lbRemoteProd.TabIndex = 13;
            this.lbRemoteProd.SelectedIndexChanged += new System.EventHandler(this.lbRemoteProd_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbFamilies);
            this.groupBox1.Controls.Add(this.lbProducts);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 272);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtSearchFamily);
            this.groupBox2.Controls.Add(this.btnSearchFamily);
            this.groupBox2.Controls.Add(this.lbRemoteProd);
            this.groupBox2.Controls.Add(this.lbRemoteManagers);
            this.groupBox2.Location = new System.Drawing.Point(395, 38);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 272);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remoto";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 454);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnDisconect);
            this.Controls.Add(this.lbRequests);
            this.Controls.Add(this.btnConnect);
            this.MaximizeBox = false;
            this.Name = "ClientForm";
            this.Text = "Cliente";
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_Closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSearchFamily;
        private System.Windows.Forms.TextBox txtSearchFamily;
        private System.Windows.Forms.ListBox lbRequests;
        private System.Windows.Forms.ListBox lbFamilies;
        private System.Windows.Forms.ListBox lbProducts;
        private System.Windows.Forms.ListBox lbRemoteManagers;
        private System.Windows.Forms.Button btnDisconect;
        private System.Windows.Forms.ListBox lbRemoteProd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

