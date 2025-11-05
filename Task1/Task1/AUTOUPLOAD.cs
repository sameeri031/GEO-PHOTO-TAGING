using ImageMagick.Drawing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task1
{

    public partial class AUTOUPLOAD : Form
    {
        bool DUP;
        List<string> selectedImages = new List<string>();
        public AUTOUPLOAD(List<string> selectedImages)
        {
            InitializeComponent();
            this.selectedImages = selectedImages;

            flowLayoutPanel1.Controls.Clear();

            foreach (var imgPath in selectedImages)
            {
                // GroupBox banaya container ki jaga
                GroupBox group = new GroupBox();
                group.Width = 170;
                group.Height = 190;    // thoda upar space taake title overlap na ho
                group.Text = "";       // groupbox title blank
                group.Margin = new Padding(10);

                // PictureBox
                PictureBox pb = new PictureBox();
                pb.Image = Image.FromFile(imgPath);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Dock = DockStyle.Fill;
                pb.Tag = imgPath;

                // CheckBox
                CheckBox cb = new CheckBox();
                cb.Text = "";
                cb.Width = 20;
                cb.Height = 20;
                cb.Checked = true;
                cb.BackColor = Color.Transparent;
                cb.Location = new Point(group.Width - 35, 5);  // top-right corner

                // Pehle checkbox add karo, phir picturebox
                group.Controls.Add(pb);
                group.Controls.Add(cb);

                flowLayoutPanel1.Controls.Add(group);
            }

        }
        private List<string> GetCheckedImages()
        {
            List<string> filtered = new List<string>();

            foreach (GroupBox group in flowLayoutPanel1.Controls)
            {
                CheckBox cb = group.Controls.OfType<CheckBox>().FirstOrDefault();
                PictureBox pb = group.Controls.OfType<PictureBox>().FirstOrDefault();

                if (cb != null && pb != null && cb.Checked == true)
                {
                    string path = pb.Tag.ToString();
                    filtered.Add(path);
                }
            }

            return filtered;
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AUTOUPLOAD_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private async Task AutoUploadAllAsync(List<string> imagesToUpload)
        {
            var uploadTasks = imagesToUpload.Select(imagePath => UploadSinglePhotoAsync(imagePath)).ToList();
            await Task.WhenAll(uploadTasks);

            MessageBox.Show(" All photos auto-uploaded successfully!");
        }
        private static readonly HttpClient client = new HttpClient();

        private async Task UploadSinglePhotoAsync(string imagePath)
        {
            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    var imageContent = new ByteArrayContent(File.ReadAllBytes(imagePath));
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    form.Add(imageContent, "file", Path.GetFileName(imagePath));
                    string title = Path.GetFileNameWithoutExtension(imagePath);
                    form.Add(new StringContent(title), "title");

                    var response = await client.PostAsync("http://127.0.0.1:8000/DUPLICATE", form);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonString);
                    DUP = result.duplicate;

                    if (!DUP)
                    {
                        using (var formupload = new MultipartFormDataContent())
                        {

                            string titleT = Path.GetFileNameWithoutExtension(imagePath);
                            string date = File.GetCreationTime(imagePath).ToString("yyyy-MM-dd");

                            string path = "C:\\Users\\Dogesh\\Desktop\\PHOTO_SERVER\\" + Path.GetFileName(imagePath);
                            var fileBytes = await File.ReadAllBytesAsync(imagePath);
                            var fileContent = new ByteArrayContent(fileBytes);
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                            formupload.Add(fileContent, "file", Path.GetFileName(imagePath));
                            var metadata = new { Person = "", Event = tbevent.Text != null ? tbevent.Text : "", Location = tblocation.Text != null ? tblocation.Text : "", Date = File.GetCreationTime(imagePath).ToString("yyyy-MM-dd") };
                            // ✅ Correct: Allow default Content-Type for form string (will be text/plain)
                            formupload.Add(new StringContent(JsonConvert.SerializeObject(metadata)), "metadata");
                            formupload.Add(new StringContent(titleT), "title");
                            formupload.Add(new StringContent(date), "date");
                            formupload.Add(new StringContent(path), "path");

                            try
                            {
                                var responseupload = await client.PostAsync("http://127.0.0.1:8000/upload_photo_MODEL", formupload);
                                responseupload.EnsureSuccessStatusCode();
                                Console.WriteLine($"✅ Uploaded {Path.GetFileName(imagePath)}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"⚠️ Error uploading {Path.GetFileName(imagePath)}: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"⚠️ Skipping — already on server.");


                    }

                }
            }

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            List<string> finalImages = GetCheckedImages();

            MessageBox.Show("Selected Images: " + finalImages.Count);


            await AutoUploadAllAsync(finalImages);
        }
    }
}
