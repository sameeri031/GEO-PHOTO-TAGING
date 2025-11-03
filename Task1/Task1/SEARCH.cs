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
    public partial class SEARCH : Form
    {
        string baseUrl = "http://127.0.0.1:8000/photos/";
        private List<(string Url, dynamic Meta)> matchedMetadata = new List<(string, dynamic)>();
        private List<string> currentFiltered = new List<string>();
        private System.Timers.Timer searchDelayTimer;
        string query = "";

        public SEARCH()
        {
            InitializeComponent();
            this.Text = "GEO PHOTO TAGGING";

            // 🔹 Initialize timer for delay
            searchDelayTimer = new System.Timers.Timer(500);
            searchDelayTimer.AutoReset = false;
            searchDelayTimer.Elapsed += async (s, ev) =>
            {
                // Use BeginInvoke to safely call UI code
                this.BeginInvoke(new Action(async () => await PerformSearch()));
            };
        }

        private async void textBox1_TextChanged(object sender, EventArgs e)
        {
            searchDelayTimer.Stop();
            searchDelayTimer.Start();
            //string query = textBox1.Text.Trim().ToLower();
            //if (string.IsNullOrWhiteSpace(query))
            //{
            //    MessageBox.Show("Please enter something to search!");
            //    return;
            //}

            //flowLayoutPanel1.Controls.Clear();
            //matchedMetadata.Clear();

            //var allImages = await GetServerImagesAsync();
            //if (allImages.Count == 0)
            //{
            //    MessageBox.Show("No images found on server!");
            //    return;
            //}

            //foreach (var filename in allImages)
            //{
            //    string imageUrl = baseUrl + filename;
            //    string metaJson = await GetMetadataAsync(imageUrl);

            //    if (!string.IsNullOrWhiteSpace(metaJson))
            //    {
            //        dynamic meta = JsonConvert.DeserializeObject(metaJson);
            //        string searchable = $"{meta.Person} {meta.Event} {meta.Location} {meta.Date}".ToLower();

            //        if (searchable.Contains(query))
            //            matchedMetadata.Add((imageUrl, meta));
            //    }
            //}

            //if (matchedMetadata.Count == 0)
            //{
            //    MessageBox.Show("No matching images found!");
            //    return;
            //}

            //currentFiltered = matchedMetadata.Select(x => x.Url).ToList();
            //DisplayImages(currentFiltered);
        }
        private async Task PerformSearch()
        {
            query = textBox1.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(query))
            {
                flowLayoutPanel1.Controls.Clear();
                return;
            }

            flowLayoutPanel1.Controls.Clear();
            matchedMetadata.Clear();

            var allImages = await GetServerImagesAsync();
            if (allImages.Count == 0)
            {
                MessageBox.Show("No images found on server!");
                return;
            }

            foreach (var filename in allImages)
            {
                string imageUrl = baseUrl + filename;
                string metaJson = await GetMetadataAsync(imageUrl);
                if (!string.IsNullOrWhiteSpace(metaJson))
                {
                    dynamic meta = JsonConvert.DeserializeObject(metaJson);
                    string searchable = $"{meta.Person} {meta.Event} {meta.Location} {meta.Date}".ToLower();

                    if (searchable.Contains(query))
                        matchedMetadata.Add((imageUrl, meta));
                }
            }

            if (matchedMetadata.Count == 0)
            {
                MessageBox.Show("No matching images found!");
                return;
            }

            currentFiltered = matchedMetadata.Select(x => x.Url).ToList();
            DisplayImages(currentFiltered);
        }
        private async void button7_Click(object sender, EventArgs e)
        {
            string query = textBox1.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Please enter something to search!");
                return;
            }

            flowLayoutPanel1.Controls.Clear();
            matchedMetadata.Clear();

            var allImages = await GetServerImagesAsync();
            if (allImages.Count == 0)
            {
                MessageBox.Show("No images found on server!");
                return;
            }

            foreach (var filename in allImages)
            {
                string imageUrl = baseUrl + filename;
                string metaJson = await GetMetadataAsync(imageUrl);

                if (!string.IsNullOrWhiteSpace(metaJson))
                {
                    dynamic meta = JsonConvert.DeserializeObject(metaJson);
                    string searchable = $"{meta.Person} {meta.Event} {meta.Location} {meta.Date}".ToLower();

                    if (searchable.Contains(query))
                        matchedMetadata.Add((imageUrl, meta));
                }
            }

            if (matchedMetadata.Count == 0)
            {
                MessageBox.Show("No matching images found!");
                return;
            }

            currentFiltered = matchedMetadata.Select(x => x.Url).ToList();
            DisplayImages(currentFiltered);
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
                return new List<string>();
            }
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
        //private async Task<string> GetMetadataAsync(string imageUrl)
        //{
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        using (var stream = await client.GetStreamAsync(imageUrl))
        //        using (var img = new MagickImage(stream))
        //        {
        //            var profile = img.GetProfile("xmp");
        //            if (profile == null) return null;

        //            string xmpXml = Encoding.UTF8.GetString(profile.ToByteArray());
        //            if (xmpXml.Contains("<custom:PhotoInfo>"))
        //            {
        //                int start = xmpXml.IndexOf("<custom:PhotoInfo>") + "<custom:PhotoInfo>".Length;
        //                int end = xmpXml.IndexOf("</custom:PhotoInfo>");
        //                string json = xmpXml.Substring(start, end - start);
        //                string decodedJson = System.Net.WebUtility.HtmlDecode(json);
        //                return decodedJson;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        // silently ignore broken images
        //    }
        //    return null;
        //}
        private async void DisplayImages(List<string> imageUrls)
        {
            foreach (string imageUrl in imageUrls)
            {
                Panel imgPanel = new Panel
                {
                    Width = 150,
                    Height = 150,
                    Margin = new Padding(10, 10, 10, 10),
                    BackColor = Color.Gray,
                    BorderStyle = BorderStyle.Fixed3D
                };

                PictureBox pic = new PictureBox
                {
                    Width = 140,
                    Height = 140,
                    Location = new Point(5, 5),
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                try
                {
                    using (HttpClient client = new HttpClient())
                    using (var stream = await client.GetStreamAsync(imageUrl))
                    {
                        pic.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    // skip broken images
                }


                imgPanel.Controls.Add(pic);

                pic.Click += (s, e) =>
                {
                    new picturedetail("SEARCH", imageUrl).ShowDialog();
                };
                flowLayoutPanel1.Controls.Add(imgPanel);
            }
        }

        private void PEOPLE_Click(object sender, EventArgs e)
        {

            flowLayoutPanel1.Controls.Clear();
            var filtered = matchedMetadata
                .Where(x => x.Meta.Person != null && x.Meta.Person.ToString().ToLower().Contains(textBox1.Text.ToLower()))
                .Select(x => x.Url)
                .ToList();
            if (filtered.Count == 0)
                MessageBox.Show("No  related person on server!");
            DisplayImages(filtered);
        }

        private void EVENT_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            var filtered = matchedMetadata
                .Where(x => x.Meta.Event != null && x.Meta.Event.ToString().ToLower().Contains(textBox1.Text.ToLower()))
                .Select(x => x.Url)
                .ToList();
            if (filtered.Count == 0)
                MessageBox.Show("No related event on server!");
            DisplayImages(filtered);

        }

        private void location_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            var filtered = matchedMetadata
                .Where(x => x.Meta.Location != null && x.Meta.Location.ToString().ToLower().Contains(textBox1.Text.ToLower()))
                .Select(x => x.Url)
                .ToList();
            if (filtered.Count == 0)
                MessageBox.Show("No related location on server!");
            DisplayImages(filtered);
        }

        private void SHOWALL_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            currentFiltered = matchedMetadata.Select(x => x.Url).ToList();
            DisplayImages(currentFiltered);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



    }
}
