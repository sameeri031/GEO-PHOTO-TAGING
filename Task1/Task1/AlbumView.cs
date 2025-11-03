using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace Task1
{
    public partial class AlbumView : Form
    {
        private string personName;
        private List<string> photos;

        public AlbumView(string personName, List<string> photos)
        {
            InitializeComponent();
            this.personName = personName;
            this.photos = photos;
            this.Text = $"GEO PHOTO TAGGING";
            this.WindowState = FormWindowState.Maximized;

        }
        private void AlbumView_Load(object sender, EventArgs e)
        {
            LoadAlbum();  // call your method here
        }
        private async void LoadAlbum()
        {

            foreach (var imgUrl in photos)
            {
                PictureBox pb = new PictureBox
                {
                    Width = 200,
                    Height = 200,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Margin = new Padding(20)
                };

                try
                {
                    using (HttpClient client = new HttpClient())
                    using (var stream = await client.GetStreamAsync(imgUrl))
                    {
                        pb.Image = Image.FromStream(stream);
                    }
                }
                catch { MessageBox.Show(" image broken"); }

                pb.Click += (s, e) =>
                {
                    new picturedetail(personName, imgUrl).ShowDialog();
                };

                flowLayoutPanel1.Controls.Add(pb);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            var f = new SEARCH();
            f.Show();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
