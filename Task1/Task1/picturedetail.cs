using ImageMagick;
using ImageMagick.Drawing;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Task1
{
    public partial class picturedetail : Form
    {
        private string photos;
        string n, ev, l;
        DateTime d;

        public picturedetail(string pn, string ph)
        {
            InitializeComponent();
            photos = ph;
            this.Text = "GEO PHOTO TAGGING";
        }

        private async void picturedetail_Load(object sender, EventArgs e)
        {
            try
            {
                if (photos.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    using (HttpClient client = new HttpClient())
                    using (var stream = await client.GetStreamAsync(photos))
                    {
                        pictureBox1.Image = Image.FromStream(stream);
                    }
                }
                else
                {
                    pictureBox1.Image = Image.FromFile(photos);
                }
            }
            catch
            {
                MessageBox.Show("⚠️ Image broken or unreachable");
            }

            readmetadata();
        }

        private async void /*UPDATE*/ button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == n && textBox2.Text == l && textBox3.Text == ev && dateTimePicker1.Value == d)
            {
                MessageBox.Show("✅ No changes detected, no update needed!");
                return;
            }
            try
            {

                using (var client = new HttpClient())
                using (var form = new MultipartFormDataContent())
                {
                    var metadata = new
                    {
                        Person = textBox1.Text,
                        Event = textBox3.Text,
                        Location = textBox2.Text,
                        Date = dateTimePicker1.Value.ToString("yyyy-MM-dd")
                    };
                    string metadataJson = JsonConvert.SerializeObject(metadata);

                    form.Add(new StringContent(Path.GetFileName(photos)), "filename");
                    form.Add(new StringContent(metadataJson), "metadata");

                    var response = await client.PostAsync("http://127.0.0.1:8000/update_metadata", form);
                    string serverResponse = await response.Content.ReadAsStringAsync();

                    MessageBox.Show(response.IsSuccessStatusCode
                        ? "✅ Metadata updated successfully!\n" + serverResponse
                        : "❌ Metadata update failed:\n" + serverResponse);
                }

                // update original values

                string[] oldArray = n.Split(',').Select(x => x.Trim()).Where(x => x != "").ToArray();
                string[] newArray = textBox1.Text.Split(',').Select(x => x.Trim()).Where(x => x != "").ToArray();

                var addedNames = newArray.Except(oldArray).ToList();
                var removedNames = oldArray.Except(newArray).ToList();

                if (addedNames.Count > 0 && removedNames.Count > 0)
                {
                    string updatename = addedNames.First();
                    string purana = removedNames.First();


                    await SendUpdates(purana, updatename);
                }
                n = textBox1.Text;
                ev = textBox3.Text;
                l = textBox2.Text;
                d = dateTimePicker1.Value;



            }
            catch (Exception ex)
            {
                MessageBox.Show("💥 Error saving metadata: " + ex.Message);
            }
            databaseupdation(photos);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
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

        public async void readmetadata()
        {
            try
            {
                string json = await GetMetadataAsync(photos); // ✅ wait for API response
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("⚠️ No metadata found on server!");
                    return;
                }

                dynamic meta = JsonConvert.DeserializeObject(json);
                if (meta == null || meta.error != null)
                {
                    MessageBox.Show("⚠️ No PhotoInfo found inside XMP!");
                    return;
                }

                // ✅ Fill UI fields
                textBox1.Text = meta.Person != null ? (string)meta.Person : "";
                n = textBox1.Text;


                textBox3.Text = meta.Event != null ? (string)meta.Event : "";
                ev = textBox3.Text;

                textBox2.Text = meta.Location != null ? (string)meta.Location : "";
                l = textBox2.Text;

                try
                {
                    d = meta.Date != null ? DateTime.Parse(meta.Date.ToString()) : DateTime.Now;
                    dateTimePicker1.Value = d;
                }
                catch
                {
                    d = DateTime.Now;
                    dateTimePicker1.Value = d;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading metadata: " + ex.Message);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var form = new LOCATION();
            form.Show();


        }

        private void button7_Click(object sender, EventArgs e)
        {
            var form = new ALBUM();
            form.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var form = new Event();
            form.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var form = new DATE();
            form.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var f = new SEARCH();
            f.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button3_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(photos))
            {
                MessageBox.Show("⚠️ No photo selected!");
                return;
            }

            string filename = Path.GetFileName(photos);

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete '{filename}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;
            string title = Path.GetFileNameWithoutExtension(photos);
            try
            {
                using (var client = new HttpClient())
                using (var form = new MultipartFormDataContent())
                {
                    form.Add(new StringContent(filename), "filename");
                    form.Add(new StringContent(title), "title");

                    var response = await client.PostAsync("http://127.0.0.1:8000/delete_photo", form);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"✅ Photo '{filename}' deleted successfully!\n{result}");

                        // Optional: delete local copy if exists
                        if (File.Exists(photos))
                        {
                            try
                            {
                                File.Delete(photos);
                                MessageBox.Show("🗑️ Local copy deleted too!");
                            }
                            catch
                            {
                                MessageBox.Show("⚠️ Could not delete local copy (maybe in use).");
                            }
                        }

                        this.Close(); // close form after deletion
                    }
                    else
                    {
                        MessageBox.Show($"❌ Server error while deleting:\n{result}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"💥 Error deleting photo: {ex.Message}");
            }
        }
        public async Task SendUpdates(string old, string name)
        {

            var data = new
            {
                OLDN = old,
                name = name
            };

            string json = JsonConvert.SerializeObject(data);

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://127.0.0.1:8000/update_persons2", content);
                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Server Response:\n" + result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private async Task databaseupdation(string imagePath)
        {
            string title = Path.GetFileNameWithoutExtension(imagePath);
            string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string path = "C:\\Users\\Dogesh\\Desktop\\PHOTO_SERVER\\" + Path.GetFileName(imagePath);
            string persons = textBox1.Text.Trim();
            string locations = "", events = "";

            locations = textBox2.Text.Trim();

            events = textBox3.Text.Trim();

            using (var client = new HttpClient())
            {
                var form = new MultipartFormDataContent();

                form.Add(new StringContent(title), "title");
                form.Add(new StringContent(date), "date");
                form.Add(new StringContent(path), "path");
                form.Add(new StringContent(persons), "persons");
                form.Add(new StringContent(locations), "locations");
                form.Add(new StringContent(events), "events");

                try
                {
                    var response = await client.PostAsync("http://127.0.0.1:8000/update_database", form);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        Console.WriteLine($" Metadata saved for {Path.GetFileName(imagePath)}");
                    else
                        Console.WriteLine($" Metadata save failed for {Path.GetFileName(imagePath)}: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Error saving metadata for {Path.GetFileName(imagePath)}: {ex.Message}");
                }
            }
        }



    }
}
