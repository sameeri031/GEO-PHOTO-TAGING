namespace Task1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            label7 = new Label();
            label6 = new Label();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            label4 = new Label();
            dateTimePicker1 = new DateTimePicker();
            label3 = new Label();
            label2 = new Label();
            tbevent = new TextBox();
            tblocation = new TextBox();
            tbperson = new TextBox();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            button4 = new Button();
            panel1 = new Panel();
            linkLabel1 = new LinkLabel();
            label5 = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Silver;
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(tbevent);
            groupBox1.Controls.Add(tblocation);
            groupBox1.Controls.Add(tbperson);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.Black;
            groupBox1.Location = new Point(210, 128);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(1338, 757);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.DarkGreen;
            label7.Location = new Point(156, 134);
            label7.Name = "label7";
            label7.Size = new Size(304, 25);
            label7.TabIndex = 15;
            label7.Text = "EXTRACTING FACES FRON IMAGE......";
            label7.Visible = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.DarkGreen;
            label6.Location = new Point(29, 646);
            label6.Name = "label6";
            label6.Size = new Size(632, 25);
            label6.TabIndex = 14;
            label6.Text = "If there is more than one value of any property  separate them by a comma \",\"";
            label6.Click += label6_Click;
            // 
            // button3
            // 
            button3.BackColor = SystemColors.ScrollBar;
            button3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button3.ForeColor = SystemColors.ActiveCaptionText;
            button3.Location = new Point(401, 513);
            button3.Margin = new Padding(3, 2, 3, 2);
            button3.Name = "button3";
            button3.Size = new Size(112, 46);
            button3.TabIndex = 13;
            button3.Text = "READ";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ScrollBar;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = SystemColors.ActiveCaptionText;
            button2.Location = new Point(894, 668);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(129, 46);
            button2.TabIndex = 10;
            button2.Text = "UPLOAD IMAGE";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ScrollBar;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ActiveCaptionText;
            button1.Location = new Point(190, 513);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(112, 46);
            button1.TabIndex = 9;
            button1.Text = "SAVE";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(18, 435);
            label4.Name = "label4";
            label4.Size = new Size(68, 32);
            label4.TabIndex = 8;
            label4.Text = "DATE";
            label4.Click += label4_Click;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.CalendarMonthBackground = SystemColors.ScrollBar;
            dateTimePicker1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dateTimePicker1.Location = new Point(156, 435);
            dateTimePicker1.Margin = new Padding(3, 2, 3, 2);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(300, 39);
            dateTimePicker1.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(20, 338);
            label3.Name = "label3";
            label3.Size = new Size(84, 32);
            label3.TabIndex = 6;
            label3.Text = "EVENT";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(18, 211);
            label2.Name = "label2";
            label2.Size = new Size(125, 32);
            label2.TabIndex = 5;
            label2.Text = "LOCATION";
            // 
            // tbevent
            // 
            tbevent.BackColor = SystemColors.ScrollBar;
            tbevent.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbevent.Location = new Point(156, 329);
            tbevent.Margin = new Padding(3, 2, 3, 2);
            tbevent.Name = "tbevent";
            tbevent.Size = new Size(456, 39);
            tbevent.TabIndex = 4;
            // 
            // tblocation
            // 
            tblocation.BackColor = SystemColors.ScrollBar;
            tblocation.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tblocation.Location = new Point(156, 202);
            tblocation.Margin = new Padding(3, 2, 3, 2);
            tblocation.Name = "tblocation";
            tblocation.Size = new Size(456, 39);
            tblocation.TabIndex = 3;
            tblocation.TextChanged += tblocation_TextChanged;
            // 
            // tbperson
            // 
            tbperson.BackColor = SystemColors.ScrollBar;
            tbperson.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tbperson.Location = new Point(156, 81);
            tbperson.Margin = new Padding(3, 2, 3, 2);
            tbperson.Name = "tbperson";
            tbperson.Size = new Size(456, 39);
            tbperson.TabIndex = 2;
            tbperson.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(20, 85);
            label1.Name = "label1";
            label1.Size = new Size(102, 32);
            label1.TabIndex = 1;
            label1.Text = "PERSON";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ScrollBar;
            pictureBox1.Location = new Point(630, 59);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(633, 566);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // button4
            // 
            button4.BackColor = SystemColors.ScrollBar;
            button4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.ForeColor = SystemColors.ActiveCaptionText;
            button4.Location = new Point(40, 218);
            button4.Margin = new Padding(3, 2, 3, 2);
            button4.Name = "button4";
            button4.Size = new Size(112, 46);
            button4.TabIndex = 14;
            button4.Text = "BACK";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DimGray;
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(label5);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1689, 88);
            panel1.TabIndex = 32;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Chartreuse;
            linkLabel1.Location = new Point(1485, 36);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(63, 28);
            linkLabel1.TabIndex = 28;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "SYNC";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ButtonFace;
            label5.Location = new Point(0, 28);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(129, 38);
            label5.TabIndex = 0;
            label5.Text = "UPLOAD";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gray;
            ClientSize = new Size(1689, 919);
            Controls.Add(panel1);
            Controls.Add(button4);
            Controls.Add(groupBox1);
            ForeColor = SystemColors.ActiveCaptionText;
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private PictureBox pictureBox1;
        private TextBox tbperson;
        private Label label4;
        private DateTimePicker dateTimePicker1;
        private Label label3;
        private Label label2;
        private TextBox tbevent;
        private TextBox tblocation;
        private Button button2;
        private Button button1;
        private Button button3;
        private Button button4;
        //private Button button5;
        //private Button button6;
        //private Button button7;
        //private Button button8;
        private Panel panel1;
        private Label label5;
        private LinkLabel linkLabel1;
        private Label label6;
        private Label label7;
    }
}
