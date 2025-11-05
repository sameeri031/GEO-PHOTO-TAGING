using ImageMagick;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Task1
{

    public partial class ALBUM : Form
    {
        bool groupByMonth = true;
        bool isLoaded = false;

        private List<string> allImages = new List<string>();
        private Dictionary<string, dynamic> metadataCache = new();

        public ALBUM()
        {
            InitializeComponent();
            label1.Text = "PEOPLE";
            this.Text = "GEO PHOTO TAGGING";
            this.WindowState = FormWindowState.Maximized;
            flowLayoutPanel1.Controls.Clear();
            this.Load += ALBUM_Load;
        }
        private async void ALBUM_Load(object sender, EventArgs e)
        {
            try
            {
                if (isLoaded) return;
                isLoaded = true;
                allImages = await GetServerImagesAsync();
                LoadPeopleGroups();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading images: " + ex.Message);
            }
        }
        //***********************************************   
        private async Task LoadAlbumsAsync(string category, Func<dynamic, string> getTagFunc)
        {
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
                string tagValue = await ExtractTagAsync(imageUrl, getTagFunc);
                if (string.IsNullOrWhiteSpace(tagValue))
                    tagValue = "Unknown";

                foreach (var tag in tagValue.Split(',', ';').Select(t => t.Trim()).Where(t => t != ""))
                {
                    if (!grouped.ContainsKey(tag))
                        grouped[tag] = new List<string>();
                    grouped[tag].Add(imageUrl);
                }
            }

            RenderAlbums(grouped);
        }
        private async Task<string> ExtractTagAsync(string imageUrl, Func<dynamic, string> getTagFunc)
        {
            try
            {
                // Return from cache if available
                if (metadataCache.ContainsKey(imageUrl))
                {
                    dynamic cachedMeta = metadataCache[imageUrl];
                    try
                    {
                        string tag = getTagFunc(cachedMeta);
                        return tag ?? "";
                    }
                    catch { return ""; }
                }

                // Fetch JSON from server
                string json = await GetMetadataAsync(imageUrl);
                if (string.IsNullOrWhiteSpace(json))
                    return "";

                dynamic meta = null;
                try
                {
                    meta = JsonConvert.DeserializeObject(json);
                }
                catch
                {
                    return "";
                }

                // Save to cache for future use
                metadataCache[imageUrl] = meta;

                // Apply user-provided function to extract tag
                try
                {
                    string tag = getTagFunc(meta);
                    return tag ?? "";
                }
                catch
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        private void RenderAlbums(Dictionary<string, List<string>> grouped)
        {
            flowLayoutPanel1.Controls.Clear();

            // sort keys for stable order (optional)
            var keys = grouped.Keys.OrderBy(k => k).ToList();

            foreach (var key in keys)
            {
                string displayName = key;
                var images = grouped[key];
                string coverUrl = images.FirstOrDefault();

                // Album card
                Panel albumPanel = new Panel
                {
                    Width = 150,
                    Height = 190,
                    Margin = new Padding(20, 20, 20, 20),
                    BackColor = Color.DimGray,
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
                    Text = displayName,
                    AutoSize = false,
                    Width = 140,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(5, 150)
                };

                // Load album cover async — don't block UI thread
                if (!string.IsNullOrWhiteSpace(coverUrl))
                {
                    // fire-and-forget task to set image (wrap exceptions)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            using (HttpClient client = new HttpClient())
                            using (var stream = await client.GetStreamAsync(coverUrl))
                            {
                                var img = Image.FromStream(stream);
                                // Because this is background thread, marshal to UI thread
                                cover.Invoke((Action)(() =>
                                {
                                    cover.Image = img;
                                }));
                            }
                        }
                        catch
                        {
                            // ignore broken covers
                        }
                    });
                }

                // Open album on click
                EventHandler openAlbum = (s, e) =>
                {
                    // ensure closure captures current values
                    string name = displayName;
                    var imgs = images;
                    new AlbumView(name, imgs).ShowDialog();
                };

                // Attach handlers to all clickable parts
                albumPanel.Click += openAlbum;
                cover.Click += openAlbum;
                nameLabel.Click += openAlbum;

                albumPanel.Controls.Add(cover);
                albumPanel.Controls.Add(nameLabel);

                flowLayoutPanel1.Controls.Add(albumPanel);
            }
        }
        private async void LoadPeopleGroups() =>
    await LoadAlbumsAsync("People", meta => (string)meta.Person ?? "");


        private List<string> GetAllImageFiles(string root)
        {
            string[] exts = { ".jpg", ".jpeg", ".png", ".bmp" };
            return Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                            .Where(f => exts.Contains(Path.GetExtension(f).ToLower()))
                            .ToList();
        }



        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

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

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private async void button4_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            await LoadAlbumsAsync("People", meta => (string)meta.Person ?? "");
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            await LoadAlbumsAsync("Location", meta => (string)meta.Location ?? "");
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            await LoadAlbumsAsync("Event", meta => (string)meta.Event ?? "");
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            await LoadAlbumsAsync("Date", meta => (string)meta.Date ?? "");
        }
        private async void LoadDateGroups()
        {
            groupBox1.Visible = true;

            await LoadAlbumsAsync("Date", meta =>
            {
                string dateStr = (string)meta.Date ?? "";
                if (string.IsNullOrWhiteSpace(dateStr))
                    return "Unknown";

                // Try to parse date
                if (DateTime.TryParse(dateStr, out DateTime dt))
                {
                    if (groupByMonth)
                        return dt.ToString("MMMM"); // Example: "October 2025"
                    else
                        return dt.ToString("yyyy"); // Example: " 2025"
                }

                return "Unknown";
            });
        }

        private void button8_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            var f = new SEARCH(allImages, metadataCache);
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (radioButton2.Checked)
                groupByMonth = true;
            else if (radioButton1.Checked)
                groupByMonth = false;
            LoadDateGroups();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var F = new Form1();
            F.Show();
        }

        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private async void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {


                // 1️⃣ Reload all image filenames from server
                allImages = await GetServerImagesAsync();

                if (allImages.Count == 0)
                {
                    MessageBox.Show("No images found on server!");
                    linkLabel1.Text = "Refresh Metadata";
                    linkLabel1.Enabled = true;
                    return;
                }

                // 2️⃣ Clear the cache and rebuild it fresh
                metadataCache.Clear();

                using (HttpClient client = new HttpClient())
                {
                    foreach (var filename in allImages)
                    {
                        try
                        {
                            string apiUrl = $"http://127.0.0.1:8000/get_metadata?filename={filename}";
                            var response = await client.GetAsync(apiUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                string json = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(json))
                                {
                                    dynamic meta = JsonConvert.DeserializeObject(json);
                                    if (meta != null)
                                        metadataCache[$"http://127.0.0.1:8000/photos/{filename}"] = meta;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Just skip broken images instead of stopping the loop
                            Console.WriteLine($"Failed to update metadata for {filename}: {ex.Message}");
                        }
                    }
                }

                // 3️⃣ Optionally refresh current albums UI
                MessageBox.Show("✅ Metadata successfully updated from server!");


                // optional: reload albums automatically
                await LoadAlbumsAsync("People", meta => (string)meta.Person ?? "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing metadata: " + ex.Message);

            }
            finally
            {

            }
        }


        //private void button5_Click(object sender, EventArgs e)
        //{
        //    flowLayoutPanel1.Controls.Clear();
        //    groupBox1.Visible = false;
        //    label2.Visible = false;
        //    label1.Text = "LOCATION";
        //    loadlocationgroup();

        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    flowLayoutPanel1.Controls.Clear();
        //    groupBox1.Visible = false;
        //    label2.Visible = false;
        //    label1.Text = "PEOPLE";
        //    LoadPeopleGroups();
        //}

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    flowLayoutPanel1.Controls.Clear();
        //    groupBox1.Visible = false;
        //    label2.Visible = false;
        //    label1.Text = "EVENT";
        //    loadeventgroup();
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    flowLayoutPanel1.Controls.Clear();
        //    label1.Text = "DATE";
        //    groupBox1.Visible = true;
        //    label2.Visible = true;
        //    if (radioButton2.Checked)
        //        groupByMonth = true;
        //    else if (radioButton1.Checked)
        //        groupByMonth = false;
        //    loaddategroup();
        //}










        //LOCATION
        //private async void loadlocationgroup()
        //{
        //    // var allImages = await GetServerImagesAsync();
        //    if (allImages.Count == 0)
        //    {
        //        MessageBox.Show("No images found on server!");
        //        return;
        //    }

        //    var grouped = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        //    string baseUrl = "http://127.0.0.1:8000/photos/";

        //    foreach (var filename in allImages)
        //    {
        //        string imageUrl = baseUrl + filename;
        //        string location = await getlocation(imageUrl);
        //        if (string.IsNullOrWhiteSpace(location))
        //            location = "Unknown";

        //        var people = location
        //            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
        //            .Select(p => p.Trim())
        //            .Where(p => !string.IsNullOrEmpty(p))
        //            .ToList();

        //        if (people.Count == 0)
        //            people.Add("Unknown");

        //        foreach (var person in people)
        //        {
        //            if (!grouped.ContainsKey(person))
        //                grouped[person] = new List<string>();
        //            grouped[person].Add(imageUrl);
        //        }
        //    }

        //    flowLayoutPanel1.Controls.Clear();

        //    // 🧩 Create album cards
        //    foreach (var kv in grouped)
        //    {
        //        string loc = kv.Key;
        //        var images = kv.Value;
        //        string coverUrl = images.FirstOrDefault();

        //        // Album card
        //        Panel albumPanel = new Panel
        //        {
        //            Width = 150,
        //            Height = 190,
        //            Margin = new Padding(20, 20, 20, 20),
        //            BackColor = Color.DimGray,
        //            BorderStyle = BorderStyle.Fixed3D,

        //            Cursor = Cursors.Hand
        //        };

        //        PictureBox cover = new PictureBox
        //        {
        //            Width = 140,
        //            Height = 140,
        //            SizeMode = PictureBoxSizeMode.Zoom,
        //            Location = new Point(5, 5)
        //        };

        //        Label nameLabel = new Label
        //        {
        //            Text = loc,
        //            AutoSize = false,
        //            Width = 140,
        //            Height = 30,
        //            TextAlign = ContentAlignment.MiddleCenter,
        //            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        //            ForeColor = Color.White,

        //            Location = new Point(5, 150)
        //        };

        //        // Load album cover
        //        try
        //        {
        //            using (HttpClient client = new HttpClient())
        //            using (var stream = await client.GetStreamAsync(coverUrl))
        //            {
        //                cover.Image = Image.FromStream(stream);
        //            }
        //        }
        //        catch { /* ignore broken */ }

        //        // Open album on click
        //        EventHandler openAlbum = (s, e) =>
        //        {
        //            new AlbumView(loc, images).ShowDialog();
        //        };

        //        albumPanel.Click += openAlbum;
        //        cover.Click += openAlbum;
        //        nameLabel.Click += openAlbum;

        //        albumPanel.Controls.Add(cover);
        //        albumPanel.Controls.Add(nameLabel);
        //        flowLayoutPanel1.Controls.Add(albumPanel);
        //    }
        //}




        //private async Task<string> getlocation(string imageUrl)
        //{
        //    try
        //    {

        //        string json = await GetMetadataAsync(imageUrl); // ✅ wait for API response
        //        if (string.IsNullOrWhiteSpace(json))
        //        {
        //            MessageBox.Show("⚠️ No metadata found on server!");
        //            return "";
        //        }

        //        dynamic meta = JsonConvert.DeserializeObject(json);
        //        if (meta == null || meta.error != null)
        //        {

        //            return "";
        //        }

        //        // ✅ Fill UI fields
        //        if (meta.Location != null && !string.IsNullOrWhiteSpace(meta.Location.ToString()))
        //            return meta.Location.ToString();
        //        else
        //            return "";

        //    }
        //    catch { MessageBox.Show(" ggggg"); }
        //    return null;
        //}
        ////EVENT
        //private async void loadeventgroup()
        //{
        //    //  var allImages = await GetServerImagesAsync();
        //    if (allImages.Count == 0)
        //    {
        //        MessageBox.Show("No images found on server!");
        //        return;
        //    }

        //    var grouped = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        //    string baseUrl = "http://127.0.0.1:8000/photos/";

        //    foreach (var filename in allImages)
        //    {
        //        string imageUrl = baseUrl + filename;
        //        string eventname = await getevent(imageUrl);
        //        if (string.IsNullOrWhiteSpace(eventname))
        //            eventname = "Unknown";

        //        var people = eventname
        //            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
        //            .Select(p => p.Trim())
        //            .Where(p => !string.IsNullOrEmpty(p))
        //            .ToList();

        //        if (people.Count == 0)
        //            people.Add("Unknown");

        //        foreach (var person in people)
        //        {
        //            if (!grouped.ContainsKey(person))
        //                grouped[person] = new List<string>();
        //            grouped[person].Add(imageUrl);
        //        }
        //    }

        //    flowLayoutPanel1.Controls.Clear();

        //    // 🧩 Create album cards
        //    foreach (var kv in grouped)
        //    {
        //        string ev = kv.Key;
        //        var images = kv.Value;
        //        string coverUrl = images.FirstOrDefault();

        //        // Album card
        //        Panel albumPanel = new Panel
        //        {
        //            Width = 150,
        //            Height = 190,
        //            Margin = new Padding(20, 20, 20, 20),

        //            BackColor = Color.DimGray,
        //            BorderStyle = BorderStyle.Fixed3D,

        //            Cursor = Cursors.Hand
        //        };

        //        PictureBox cover = new PictureBox
        //        {
        //            Width = 140,
        //            Height = 140,
        //            SizeMode = PictureBoxSizeMode.Zoom,
        //            Location = new Point(5, 5)
        //        };

        //        Label nameLabel = new Label
        //        {
        //            Text = ev,
        //            AutoSize = false,
        //            Width = 140,
        //            Height = 30,
        //            TextAlign = ContentAlignment.MiddleCenter,
        //            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        //            ForeColor = Color.White,

        //            Location = new Point(5, 150)
        //        };

        //        // Load album cover
        //        try
        //        {
        //            using (HttpClient client = new HttpClient())
        //            using (var stream = await client.GetStreamAsync(coverUrl))
        //            {
        //                cover.Image = Image.FromStream(stream);
        //            }
        //        }
        //        catch { /* ignore broken */ }

        //        // Open album on click
        //        EventHandler openAlbum = (s, e) =>
        //        {
        //            new AlbumView(ev, images).ShowDialog();
        //        };

        //        albumPanel.Click += openAlbum;
        //        cover.Click += openAlbum;
        //        nameLabel.Click += openAlbum;

        //        albumPanel.Controls.Add(cover);
        //        albumPanel.Controls.Add(nameLabel);
        //        flowLayoutPanel1.Controls.Add(albumPanel);
        //    }
        //}
        //private async Task<string> getevent(string imageUrl)
        //{
        //    try
        //    {

        //        string json = await GetMetadataAsync(imageUrl); // ✅ wait for API response
        //        if (string.IsNullOrWhiteSpace(json))
        //        {
        //            MessageBox.Show("⚠️ No metadata found on server!");
        //            return "";
        //        }

        //        dynamic meta = JsonConvert.DeserializeObject(json);
        //        if (meta == null || meta.error != null)
        //        {
        //            return "";
        //        }

        //        // ✅ Fill UI fields
        //        if (meta.Event != null && !string.IsNullOrWhiteSpace(meta.Event.ToString()))
        //            return meta.Event.ToString();
        //        else
        //            return "";

        //    }
        //    catch { MessageBox.Show(" ggggg"); }
        //    return null;
        //}
        ////******************************************************************************8DATE
        //private async void loaddategroup()
        //{

        //    // var allImages = await GetServerImagesAsync();

        //    if (allImages.Count == 0)
        //    {
        //        MessageBox.Show("No images found on server!");
        //        return;
        //    }

        //    var grouped = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        //    string baseUrl = "http://127.0.0.1:8000/photos/";

        //    foreach (var filename in allImages)
        //    {
        //        string imageUrl = baseUrl + filename;
        //        string dateValue = await getdate(imageUrl);
        //        string groupKey = "Unknown";

        //        if (!string.IsNullOrWhiteSpace(dateValue))
        //        {
        //            if (DateTime.TryParse(dateValue, out DateTime dt))
        //            {
        //                if (groupByMonth)
        //                    groupKey = dt.ToString("MMMM"); // Month name
        //                else
        //                    groupKey = dt.Year.ToString(); // Year only
        //            }
        //        }

        //        var people = new List<string> { groupKey };



        //        if (people.Count == 0)
        //            people.Add("Unknown");

        //        foreach (var person in people)
        //        {
        //            if (!grouped.ContainsKey(person))
        //                grouped[person] = new List<string>();
        //            grouped[person].Add(imageUrl);
        //        }
        //    }

        //    flowLayoutPanel1.Controls.Clear();

        //    // 🧩 Create album cards
        //    foreach (var kv in grouped)
        //    {
        //        string ev = kv.Key;
        //        var images = kv.Value;
        //        string coverUrl = images.FirstOrDefault();

        //        // Album card
        //        Panel albumPanel = new Panel
        //        {
        //            Width = 150,
        //            Height = 190,
        //            Margin = new Padding(20, 20, 20, 20),

        //            BackColor = Color.DimGray,
        //            BorderStyle = BorderStyle.Fixed3D,

        //            Cursor = Cursors.Hand
        //        };

        //        PictureBox cover = new PictureBox
        //        {
        //            Width = 140,
        //            Height = 140,
        //            SizeMode = PictureBoxSizeMode.Zoom,
        //            Location = new Point(5, 5)
        //        };

        //        Label nameLabel = new Label
        //        {
        //            Text = ev,
        //            AutoSize = false,
        //            Width = 140,
        //            Height = 30,
        //            TextAlign = ContentAlignment.MiddleCenter,
        //            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        //            ForeColor = Color.White,

        //            Location = new Point(5, 150)
        //        };

        //        // Load album cover
        //        try
        //        {
        //            using (HttpClient client = new HttpClient())
        //            using (var stream = await client.GetStreamAsync(coverUrl))
        //            {
        //                cover.Image = Image.FromStream(stream);
        //            }
        //        }
        //        catch { /* ignore broken */ }

        //        // Open album on click
        //        EventHandler openAlbum = (s, e) =>
        //        {
        //            new AlbumView(ev, images).ShowDialog();
        //        };

        //        albumPanel.Click += openAlbum;
        //        cover.Click += openAlbum;
        //        nameLabel.Click += openAlbum;

        //        albumPanel.Controls.Add(cover);
        //        albumPanel.Controls.Add(nameLabel);
        //        flowLayoutPanel1.Controls.Add(albumPanel);
        //    }
        //}

        //private async Task<string> getdate(string imageUrl)
        //{
        //    try
        //    {

        //        string json = await GetMetadataAsync(imageUrl); // ✅ wait for API response
        //        if (string.IsNullOrWhiteSpace(json))
        //        {
        //            MessageBox.Show("⚠️ No metadata found on server!");
        //            return "";
        //        }

        //        dynamic meta = JsonConvert.DeserializeObject(json);
        //        if (meta == null || meta.error != null)
        //        {

        //            return "";
        //        }

        //        // ✅ Fill UI fields
        //        if (meta.Date != null && !string.IsNullOrWhiteSpace(meta.Date.ToString()))
        //            return meta.Date.ToString();
        //        else
        //            return "";

        //    }
        //    catch { MessageBox.Show(" ggggg"); }
        //    return null;
        //}
        //private async Task<string> GetPersonTagAsync(string imageUrl)
        //{

        //    string json = await GetMetadataAsync(imageUrl); // ✅ wait for API response
        //    if (string.IsNullOrWhiteSpace(json))
        //    {
        //        MessageBox.Show("⚠️ No metadata found on server!");
        //        return "";
        //    }

        //    dynamic meta = JsonConvert.DeserializeObject(json);
        //    if (meta == null || meta.error != null)
        //    {
        //        MessageBox.Show("⚠️ No PhotoInfo found inside XMP!");
        //        return "";
        //    }

        //    // ✅ Fill UI fields
        //    if (meta.Person != null && !string.IsNullOrWhiteSpace(meta.Person.ToString()))
        //        return meta.Person.ToString();
        //    else
        //        return "";



        //}
        //private Image GetThumbnail(string path)
        //{
        //    using (var img = Image.FromFile(path))
        //    {
        //        return (Image)(new Bitmap(img, new Size(100, 100)));
        //    }
        //}
        //    private async void LoadLocationGroups() =>
        //await LoadAlbumsAsync("Location", meta => (string)meta.Location ?? "");

        //    private async void LoadEventGroups() =>
        //await LoadAlbumsAsync("Event", meta => (string)meta.Event ?? "");



        //*****************************************************************8


        //private async void LoadPeopleGroups()
        //{

        //    if (allImages.Count == 0)
        //    {
        //        MessageBox.Show("No images found on server!");
        //        return;
        //    }

        //    var grouped = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        //    string baseUrl = "http://127.0.0.1:8000/photos/";

        //    foreach (var filename in allImages)
        //    {
        //        string imageUrl = baseUrl + filename;
        //        string personTag = await GetPersonTagAsync(imageUrl);
        //        if (string.IsNullOrWhiteSpace(personTag))
        //            personTag = "Unknown";

        //        var people = personTag
        //            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
        //            .Select(p => p.Trim())
        //            .Where(p => !string.IsNullOrEmpty(p))
        //            .ToList();

        //        if (people.Count == 0)
        //            people.Add("Unknown");

        //        foreach (var person in people)
        //        {
        //            if (!grouped.ContainsKey(person))
        //                grouped[person] = new List<string>();
        //            grouped[person].Add(imageUrl);
        //        }
        //    }

        //    flowLayoutPanel1.Controls.Clear();

        //    // 🧩 Create album cards
        //    foreach (var kv in grouped)
        //    {
        //        string personName = kv.Key;
        //        var images = kv.Value;
        //        string coverUrl = images.FirstOrDefault();

        //        // Album card
        //        Panel albumPanel = new Panel
        //        {
        //            Width = 150,
        //            Height = 190,
        //            Margin = new Padding(20, 20, 20, 20),
        //            BackColor = Color.DimGray,
        //            BorderStyle = BorderStyle.Fixed3D,

        //            Cursor = Cursors.Hand
        //        };

        //        PictureBox cover = new PictureBox
        //        {
        //            Width = 140,
        //            Height = 140,
        //            SizeMode = PictureBoxSizeMode.Zoom,
        //            Location = new Point(5, 5)
        //        };

        //        Label nameLabel = new Label
        //        {
        //            Text = personName,
        //            AutoSize = false,
        //            Width = 140,
        //            Height = 30,
        //            TextAlign = ContentAlignment.MiddleCenter,
        //            Font = new Font("Segoe UI", 10, FontStyle.Bold),
        //            ForeColor = Color.White,

        //            Location = new Point(5, 150)
        //        };

        //        // Load album cover
        //        try
        //        {
        //            using (HttpClient client = new HttpClient())
        //            using (var stream = await client.GetStreamAsync(coverUrl))
        //            {
        //                cover.Image = Image.FromStream(stream);
        //            }
        //        }
        //        catch { /* ignore broken */ }

        //        // Open album on click
        //        EventHandler openAlbum = (s, e) =>
        //        {
        //            new AlbumView(personName, images).ShowDialog();
        //        };

        //        albumPanel.Click += openAlbum;
        //        cover.Click += openAlbum;
        //        nameLabel.Click += openAlbum;

        //        albumPanel.Controls.Add(cover);
        //        albumPanel.Controls.Add(nameLabel);
        //        flowLayoutPanel1.Controls.Add(albumPanel);
        //    }
        //}


    }
}
