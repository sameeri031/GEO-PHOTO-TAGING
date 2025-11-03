
namespace Task1
{
    partial class AlbumView
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
            button7 = new Button();
            panel1 = new Panel();
            linkLabel1 = new LinkLabel();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BackColor = Color.Silver;
            flowLayoutPanel1.BorderStyle = BorderStyle.Fixed3D;
            flowLayoutPanel1.Location = new Point(145, 105);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1707, 683);
            flowLayoutPanel1.TabIndex = 0;
            flowLayoutPanel1.Paint += flowLayoutPanel1_Paint;
            // 
            // button7
            // 
            button7.BackColor = SystemColors.ScrollBar;
            button7.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button7.ForeColor = Color.Black;
            button7.Location = new Point(12, 127);
            button7.Margin = new Padding(3, 2, 3, 2);
            button7.Name = "button7";
            button7.Size = new Size(112, 46);
            button7.TabIndex = 26;
            button7.Text = "BACK";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click_1;
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
            panel1.Size = new Size(1853, 88);
            panel1.TabIndex = 31;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Chartreuse;
            linkLabel1.Location = new Point(1646, 38);
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
            label1.Size = new Size(145, 38);
            label1.TabIndex = 0;
            label1.Text = "PICTURES";
            // 
            // AlbumView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gray;
            ClientSize = new Size(1853, 760);
            Controls.Add(panel1);
            Controls.Add(button7);
            Controls.Add(flowLayoutPanel1);
            ForeColor = Color.Black;
            Name = "AlbumView";
            Text = "AlbumView";
            Load += AlbumView_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var form = new ALBUM();
            form.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        //private Button button8;
        //private Button button1;
        //private Button button6;
        //private Button button5;
        //private Button button4;
        private Button button7;
        private Panel panel1;
        private Label label1;
        private LinkLabel linkLabel1;
    }
}