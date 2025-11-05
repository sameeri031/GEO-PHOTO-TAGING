namespace Task1
{
    partial class AUTOUPLOAD
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
            flowLayoutPanel1 = new FlowLayoutPanel();
            panel1 = new Panel();
            linkLabel1 = new LinkLabel();
            label1 = new Label();
            button7 = new Button();
            button1 = new Button();
            groupBox1 = new GroupBox();
            label6 = new Label();
            label3 = new Label();
            label2 = new Label();
            tbevent = new TextBox();
            tblocation = new TextBox();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BackColor = Color.Silver;
            flowLayoutPanel1.Location = new Point(116, 372);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1751, 435);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DimGray;
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1591, 88);
            panel1.TabIndex = 20;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Chartreuse;
            linkLabel1.Location = new Point(1680, 35);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(63, 28);
            linkLabel1.TabIndex = 27;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "SYNC";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(0, 28);
            label1.Margin = new Padding(10);
            label1.Name = "label1";
            label1.Size = new Size(213, 38);
            label1.TabIndex = 0;
            label1.Text = "AUTO UPLOAD";
            label1.Click += label1_Click;
            // 
            // button7
            // 
            button7.BackColor = SystemColors.ScrollBar;
            button7.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button7.Location = new Point(875, 833);
            button7.Margin = new Padding(3, 2, 3, 2);
            button7.Name = "button7";
            button7.Size = new Size(135, 46);
            button7.TabIndex = 22;
            button7.Text = "BACK";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ScrollBar;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(1049, 833);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(135, 46);
            button1.TabIndex = 23;
            button1.Text = "UPLOAD";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Silver;
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(tbevent);
            groupBox1.Controls.Add(tblocation);
            groupBox1.Location = new Point(616, 125);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(684, 222);
            groupBox1.TabIndex = 24;
            groupBox1.TabStop = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.DarkGreen;
            label6.Location = new Point(113, 27);
            label6.Name = "label6";
            label6.Size = new Size(455, 25);
            label6.TabIndex = 29;
            label6.Text = "IF YOU WANT TO ADD COMMON LOCATION OR EVENT";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(52, 150);
            label3.Name = "label3";
            label3.Size = new Size(84, 32);
            label3.TabIndex = 28;
            label3.Text = "EVENT";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(38, 75);
            label2.Name = "label2";
            label2.Size = new Size(125, 32);
            label2.TabIndex = 27;
            label2.Text = "LOCATION";
            // 
            // tbevent
            // 
            tbevent.BackColor = SystemColors.ScrollBar;
            tbevent.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbevent.Location = new Point(199, 143);
            tbevent.Margin = new Padding(3, 2, 3, 2);
            tbevent.Name = "tbevent";
            tbevent.Size = new Size(456, 39);
            tbevent.TabIndex = 26;
            // 
            // tblocation
            // 
            tblocation.BackColor = SystemColors.ScrollBar;
            tblocation.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tblocation.Location = new Point(199, 72);
            tblocation.Margin = new Padding(3, 2, 3, 2);
            tblocation.Name = "tblocation";
            tblocation.Size = new Size(456, 39);
            tblocation.TabIndex = 25;
            // 
            // AUTOUPLOAD
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gray;
            ClientSize = new Size(1591, 899);
            Controls.Add(groupBox1);
            Controls.Add(button1);
            Controls.Add(button7);
            Controls.Add(panel1);
            Controls.Add(flowLayoutPanel1);
            Name = "AUTOUPLOAD";
            Text = "AUTOUPLOAD";
            WindowState = FormWindowState.Maximized;
            Load += AUTOUPLOAD_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Panel panel1;
        private LinkLabel linkLabel1;
        private Label label1;
        private Button button7;
        private Button button1;
        private GroupBox groupBox1;
        private TextBox tblocation;
        private TextBox tbevent;
        private Label label2;
        private Label label3;
        private Label label6;
    }
}