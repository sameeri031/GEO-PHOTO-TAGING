namespace Task1
{
    partial class SEARCH
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
            textBox1 = new TextBox();
            button7 = new Button();
            PEOPLE = new Button();
            EVENT = new Button();
            location = new Button();
            SHOWALL = new Button();
            button1 = new Button();
            panel1 = new Panel();
            linkLabel1 = new LinkLabel();
            label1 = new Label();
            label2 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.BackColor = Color.Silver;
            flowLayoutPanel1.BorderStyle = BorderStyle.Fixed3D;
            flowLayoutPanel1.Location = new Point(181, 189);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1749, 726);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(679, 28);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(255, 45);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // button7
            // 
            button7.BackColor = SystemColors.ScrollBar;
            button7.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button7.Location = new Point(969, 27);
            button7.Margin = new Padding(3, 2, 3, 2);
            button7.Name = "button7";
            button7.Size = new Size(112, 46);
            button7.TabIndex = 19;
            button7.Text = "SEARCH";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // PEOPLE
            // 
            PEOPLE.BackColor = SystemColors.ScrollBar;
            PEOPLE.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PEOPLE.Location = new Point(560, 137);
            PEOPLE.Margin = new Padding(3, 2, 3, 2);
            PEOPLE.Name = "PEOPLE";
            PEOPLE.Size = new Size(112, 46);
            PEOPLE.TabIndex = 20;
            PEOPLE.Text = "PEOPLE";
            PEOPLE.UseVisualStyleBackColor = false;
            PEOPLE.Click += PEOPLE_Click;
            // 
            // EVENT
            // 
            EVENT.BackColor = SystemColors.ScrollBar;
            EVENT.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            EVENT.Location = new Point(695, 137);
            EVENT.Margin = new Padding(3, 2, 3, 2);
            EVENT.Name = "EVENT";
            EVENT.Size = new Size(112, 46);
            EVENT.TabIndex = 21;
            EVENT.Text = "EVENT";
            EVENT.UseVisualStyleBackColor = false;
            EVENT.Click += EVENT_Click;
            // 
            // location
            // 
            location.BackColor = SystemColors.ScrollBar;
            location.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            location.Location = new Point(857, 137);
            location.Margin = new Padding(3, 2, 3, 2);
            location.Name = "location";
            location.Size = new Size(143, 46);
            location.TabIndex = 22;
            location.Text = "LOCATION";
            location.UseVisualStyleBackColor = false;
            location.Click += location_Click;
            // 
            // SHOWALL
            // 
            SHOWALL.BackColor = SystemColors.ScrollBar;
            SHOWALL.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SHOWALL.Location = new Point(1026, 137);
            SHOWALL.Margin = new Padding(3, 2, 3, 2);
            SHOWALL.Name = "SHOWALL";
            SHOWALL.Size = new Size(147, 46);
            SHOWALL.TabIndex = 23;
            SHOWALL.Text = "SHOWALL";
            SHOWALL.UseVisualStyleBackColor = false;
            SHOWALL.Click += SHOWALL_Click;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ScrollBar;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(23, 223);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(112, 46);
            button1.TabIndex = 24;
            button1.Text = "BACK";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DimGray;
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(button7);
            panel1.Controls.Add(textBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1924, 88);
            panel1.TabIndex = 44;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Chartreuse;
            linkLabel1.Location = new Point(1727, 36);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(63, 28);
            linkLabel1.TabIndex = 28;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "SYNC";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(0, 28);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(124, 38);
            label1.TabIndex = 0;
            label1.Text = "SEARCH";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ButtonFace;
            label2.Location = new Point(408, 139);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(102, 38);
            label2.TabIndex = 20;
            label2.Text = "FILTER";
            // 
            // SEARCH
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gray;
            ClientSize = new Size(1924, 927);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(SHOWALL);
            Controls.Add(location);
            Controls.Add(EVENT);
            Controls.Add(PEOPLE);
            Controls.Add(flowLayoutPanel1);
            Name = "SEARCH";
            Text = "SEARCH";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox textBox1;
        private Button button7;
        private Button PEOPLE;
        private Button EVENT;
        private Button location;
        private Button SHOWALL;
        private Button button1;
        private Panel panel1;
        private Label label1;
        //private Button button8;
        //private Button button2;
        //private Button button6;
        //private Button button5;
        //private Button button3;
        private Label label2;
        private LinkLabel linkLabel1;
    }
}