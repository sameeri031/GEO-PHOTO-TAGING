using ImageMagick;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task1
{
    public partial class DATE : Form
    {
        bool groupByMonth = true;
        public DATE()
        {
            InitializeComponent();
            this.Text = "DATE Grouping";
            this.WindowState = FormWindowState.Maximized;
            flowLayoutPanel1.Controls.Clear();

            loaddategroup();

        }
        private async void loaddategroup()
        {
            var allImages = await GetServerImagesAsync();
            if (allImages.Count == 0)
            {
                MessageBox.Show("No images found on server!");
                return;
            }

            var grouped = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            string baseUrl = "http://127.0.0.1:8000/photos/";

            foreach (var filename in allImages)
            {
                string imageUrl = baseUrl + filename;
                string dateValue = await getdate(imageUrl);
                string groupKey = "Unknown";

                if (!string.IsNullOrWhiteSpace(dateValue))
                {
                    if (DateTime.TryParse(dateValue, out DateTime dt))
                    {
                        if (groupByMonth)
                            groupKey = dt.ToString("MMMM"); // Month name
                        else
                            groupKey = dt.Year.ToString(); // Year only
                    }
                }

                var people = new List<string> { groupKey };



                if (people.Count == 0)
                    people.Add("Unknown");

                foreach (var person in people)
                {
                    if (!grouped.ContainsKey(person))
                        grouped[person] = new List<string>();
                    grouped[person].Add(imageUrl);
                }
            }

            flowLayoutPanel1.Controls.Clear();

            // 🧩 Create album cards
            foreach (var kv in grouped)
            {
                string ev = kv.Key;
                var images = kv.Value;
                string coverUrl = images.FirstOrDefault();

                // Album card
                Panel albumPanel = new Panel
                {
                    Width = 150,
                    Height = 190,
                    Margin = new Padding(20, 20, 20, 20),

                    BackColor = Color.Gray,
                    BorderStyle = BorderStyle.Fixed3D,

                    Cursor = Cursors.Hand
                };

                PictureBox cover = new PictureBox
                {
                    Width = 140,
                    Height = 140,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(5, 5)
                };

                Label nameLabel = new Label
                {
                    Text = ev,
                    AutoSize = false,
                    Width = 140,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White,

                    Location = new Point(5, 150)
                };

                // Load album cover
                try
                {
                    using (HttpClient client = new HttpClient())
                    using (var stream = await client.GetStreamAsync(coverUrl))
                    {
                        cover.Image = Image.FromStream(stream);
                    }
                }
                catch { /* ignore broken */ }

                // Open album on click
                EventHandler openAlbum = (s, e) =>
                {
                    new AlbumView(ev, images).ShowDialog();
                };

                albumPanel.Click += openAlbum;
                cover.Click += openAlbum;
                nameLabel.Click += openAlbum;

                albumPanel.Controls.Add(cover);
                albumPanel.Controls.Add(nameLabel);
                flowLayoutPanel1.Controls.Add(albumPanel);
            }
        }


        private List<string> GetAllImageFiles(string root)
        {
            string[] exts = { ".jpg", ".jpeg", ".png", ".bmp" };
            return Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                            .Where(f => exts.Contains(Path.GetExtension(f).ToLower()))
                            .ToList();
        }

        private async Task<string> getdate(string imageUrl)
        {
            try
            {

                string json = await GetMetadataAsync(imageUrl); // ✅ wait for API response
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("⚠️ No metadata found on server!");
                    return "";
                }

                dynamic meta = JsonConvert.DeserializeObject(json);
                if (meta == null || meta.error != null)
                {
                    MessageBox.Show("⚠️ No PhotoInfo found inside XMP!");
                    return "";
                }

                // ✅ Fill UI fields
                if (meta.Date != null && !string.IsNullOrWhiteSpace(meta.Date.ToString()))
                    return meta.Date.ToString();
                else
                    return "";

            }
            catch { MessageBox.Show(" ggggg"); }
            return null;
        }
        private async Task<List<string>> GetServerImagesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("http://127.0.0.1:8000/get_all_photos");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    List<string> photos = new List<string>();

                    foreach (var p in data.photos)
                        photos.Add((string)p);

                    return photos;
                }
                else
                {
                    MessageBox.Show("Server not reachable");
                    return new List<string>();
                }
            }
        }
        private void DATE_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                groupByMonth = true;
            else if (radioButton1.Checked)
                groupByMonth = false;
            loaddategroup();
        }
        private async Task<string> GetMetadataAsync(string imageUrl)
        {
            try
            {
                string filename = Path.GetFileName(imageUrl); // extract only filename

                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"http://127.0.0.1:8000/get_metadata?filename={filename}";
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        return json;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Metadata fetch error: {ex.Message}");
            }
            return null;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var form = new LOCATION();
            form.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var form = new ALBUM();
            form.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var form = new Event();
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new DATE();
            form.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var f = new SEARCH();
            f.Show();
        }
    }
}
