using ImageMagick;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Task1
{

    public partial class Form1 : Form
    {
        bool auto = false;
        List<string> selectedImages = new List<string>();
        List<face> faceList = new List<face>();
        string imgpath = "";
        string oldnames = "";
        string newnames = "";
        bool DUP;
        int currentIndex = 0;
        int totalImages = 0;
        private bool? customTaggingChoice = null;

        List<string> idlist = new List<string>();
        public Form1()
        {
            InitializeComponent();
            this.Text = "GEO PHOTO TAGGING";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(imgpath))
            {
                MessageBox.Show("Please select photos first.");
                return;
            }

            // --- Step 1: Duplicate check ---
            // --- Step 1: Duplicate check ---
            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    var imageContent = new ByteArrayContent(File.ReadAllBytes(imgpath));
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    form.Add(imageContent, "file", Path.GetFileName(imgpath));

                    // ✅ Send title as well (required by FastAPI)
                    string title = Path.GetFileNameWithoutExtension(imgpath);
                    form.Add(new StringContent(title), "title");

                    var response = await client.PostAsync("http://127.0.0.1:8000/DUPLICATE", form);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonString);
                    DUP = result.duplicate;

                    if (DUP)
                    {
                        MessageBox.Show($"⚠️ Skipping {Path.GetFileName(imgpath)} — already on server.");
                        currentIndex++;
                        await LoadNextImage();
                        return;
                    }
                }
            }
            // --- Step 2: Upload with metadata ---
            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    var fileBytes = File.ReadAllBytes(imgpath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    form.Add(fileContent, "file", Path.GetFileName(imgpath));

                    var metadata = new
                    {
                        Person = tbperson.Text,
                        Event = tbevent.Text,
                        Location = tblocation.Text,
                        Date = dateTimePicker1.Value.ToString("yyyy-MM-dd")
                    };
                    string metadataJson = JsonConvert.SerializeObject(metadata);
                    form.Add(new StringContent(metadataJson, Encoding.UTF8, "application/json"), "metadata");

                    var response = await client.PostAsync("http://127.0.0.1:8000/upload_photo", form);
                    string serverResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        MessageBox.Show($"✅ Uploaded {Path.GetFileName(imgpath)} successfully!");
                    else
                        MessageBox.Show($"❌ Upload failed for {Path.GetFileName(imgpath)}: {serverResponse}");
                }
            }
            await databaseinsertion(imgpath);

            currentIndex++;
            await LoadNextImage();


            // --- Step 4: Handle any name updates automatically ---
            newnames = tbperson.Text;

            string[] oldArray = oldnames.Split(',').Select(x => x.Trim()).Where(x => x != "").ToArray();
            string[] newArray = newnames.Split(',').Select(x => x.Trim()).Where(x => x != "").ToArray();

            var addedNames = newArray.Except(oldArray).ToList();
            var removedNames = oldArray.Except(newArray).ToList();

            if (addedNames.Count > 0 && removedNames.Count > 0)
            {
                string updatename = addedNames.First();
                string purana = removedNames.First();
                string updateid = faceList.FirstOrDefault(f => f.name == purana)?.id ?? "";

                if (int.TryParse(updateid, out int idValue))
                    await SendUpdates(idValue, updatename);
            }



        }






        private async void button2_Click(object sender, EventArgs e)
        {
            tbperson.Text = "";
            tblocation.Text = string.Empty;
            tbevent.Text = string.Empty;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedImages = ofd.FileNames.ToList();
                if (selectedImages.Count == 0) return;

                totalImages = selectedImages.Count;
                currentIndex = 0;

                // Ask user only once
                if (customTaggingChoice == null)
                {
                    DialogResult result = MessageBox.Show(
                        "Do you want to give custom tags manually?",
                        "Custom Tagging",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    customTaggingChoice = (result == DialogResult.Yes);
                }

                if (customTaggingChoice == true)
                {
                    label7.Visible = true;
                    await LoadNextImage(); // manual tagging path
                }
                else
                {
                    MessageBox.Show("UPLOADING IT WILL TAKE YOUR FEW SECONDS");
                    auto = true;
                    await AutoUploadAllAsync(); // automatic path
                }
            }
        }

        private async Task LoadNextImage()
        {
            if (currentIndex >= totalImages)
            {
                MessageBox.Show("🎉 All photos processed!");
                currentIndex = 0;
                selectedImages.Clear();
                return;
            }

            imgpath = selectedImages[currentIndex];

            this.Text = $"Editing photo {currentIndex + 1}/{totalImages}";

            using (var stream = new FileStream(imgpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                pictureBox1.Image = Image.FromStream(stream);
            }
            tbevent.Text = string.Empty;
            tbevent.Text = string.Empty;
            tblocation.Text = string.Empty;
            tbperson.Text = string.Empty;
            DateTime fileDate = File.GetCreationTime(imgpath);
            dateTimePicker1.Value = fileDate;

            try
            {
                string personName = await IdentifyFaceAsync(imgpath);
                label7.Visible = false;
                tbperson.Text = personName;
                oldnames = personName;
            }
            catch
            {
                tbperson.Text = "";
            }

            DUP = false; // reset flag for this photo
        }
        private async Task AutoUploadAllAsync()
        {
            var uploadTasks = selectedImages.Select(imagePath => UploadSinglePhotoAsync(imagePath)).ToList();
            await Task.WhenAll(uploadTasks);

            // MessageBox.Show(" All photos auto-uploaded successfully!");
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
                            string date;
                            if (auto)
                                date = File.GetCreationTime(imagePath).ToString("yyyy-MM-dd");
                            else
                                date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                            string path = "C:\\Users\\Dogesh\\Desktop\\PHOTO_SERVER\\" + Path.GetFileName(imagePath);
                            var fileBytes = await File.ReadAllBytesAsync(imagePath);
                            var fileContent = new ByteArrayContent(fileBytes);
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                            formupload.Add(fileContent, "file", Path.GetFileName(imagePath));

                            var metadata = new { Person = "", Event = "", Location = "", Date = File.GetCreationTime(imagePath).ToString("yyyy-MM-dd") };
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
                        MessageBox.Show($"⚠️ Skipping {Path.GetFileName(imgpath)} — already on server.");


                    }

                }
            }

        }

        //private async Task UploadSinglePhotoAsync(string imagePath)
        //{
        //    using (var client = new HttpClient())
        //    using (var form = new MultipartFormDataContent())
        //    {
        //        var fileBytes = await File.ReadAllBytesAsync(imagePath);
        //        var fileContent = new ByteArrayContent(fileBytes);
        //        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        //        form.Add(fileContent, "file", Path.GetFileName(imagePath));

        //        DateTime fileDate = File.GetCreationTime(imagePath);

        //        var metadata = new
        //        {
        //            Person = "",
        //            Event = "",
        //            Location = "",
        //            Date = fileDate.ToString("yyyy-MM-dd")
        //        };

        //        string metadataJson = JsonConvert.SerializeObject(metadata);
        //        form.Add(new StringContent(metadataJson, Encoding.UTF8, "application/json"), "metadata");

        //        try
        //        {
        //            var response = await client.PostAsync("http://127.0.0.1:8000/upload_photo_MODEL", form);
        //            string result = await response.Content.ReadAsStringAsync();

        //            if (response.IsSuccessStatusCode)
        //                Console.WriteLine($"✅ Uploaded {Path.GetFileName(imagePath)} successfully.");
        //            else
        //                Console.WriteLine($"❌ Upload failed for {Path.GetFileName(imagePath)}: {result}");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"⚠️ Error uploading {Path.GetFileName(imagePath)}: {ex.Message}");
        //        }
        //    }
        //}

        public async Task<string> IdentifyFaceAsync(string imagePath)
        {
            faceList.Clear();

            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    var imageContent = new ByteArrayContent(File.ReadAllBytes(imagePath));
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    form.Add(imageContent, "file", Path.GetFileName(imagePath));

                    // FastAPI endpoint
                    var response = await client.PostAsync("http://127.0.0.1:8000/identify", form);
                    response.EnsureSuccessStatusCode();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonString);

                    string allPersons = "";
                    //foreach (var r in result.results)
                    //{
                    //    allPersons += r.name.ToString() + ", ";
                    //    faceList.Add(new face { name = r.name.ToString(),id=r.id.ToString() });

                    //}

                    foreach (var r in result.results)
                    {
                        allPersons += r.name.ToString() + ", ";
                        string idVal = r.id.ToString();
                        string nameVal = r.name.ToString();
                        // MessageBox.Show($"Got face: ID={idVal}, Name={nameVal}");
                        faceList.Add(new face { name = nameVal, id = idVal });
                    }
                    oldnames = allPersons;

                    return allPersons;
                }
            }
        }


        public async Task SendUpdates(int id, string name)
        {

            var data = new
            {
                id = id,
                name = name
            };

            string json = JsonConvert.SerializeObject(data);

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://127.0.0.1:8000/update_persons", content);
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

        private async Task databaseinsertion(string imagePath)
        {
            string title = Path.GetFileNameWithoutExtension(imagePath);
            string date;
            if (auto)
                date = File.GetCreationTime(imagePath).ToString("yyyy-MM-dd");
            else
                date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string path = "C:\\Users\\Dogesh\\Desktop\\PHOTO_SERVER\\" + Path.GetFileName(imagePath);
            string persons = tbperson.Text.Trim();
            string locations = "", events = "";
            if (tblocation.Text != null)
                locations = tblocation.Text.Trim();
            if (tbevent.Text != null)
                events = tbevent.Text.Trim();

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
                    var response = await client.PostAsync("http://127.0.0.1:8000/insert_into_database", form);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        MessageBox.Show($"✅ Metadata saved IN DATABASE {Path.GetFileName(imagePath)}");
                    else
                        MessageBox.Show($"❌ Metadata  NOT saved IN DATABASE  {Path.GetFileName(imagePath)}: {result}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"⚠️ Error saving metadata for {Path.GetFileName(imagePath)}: {ex.Message}");
                }
            }
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(imgpath) || !File.Exists(imgpath))
            //{
            //    MessageBox.Show("No modified image found. Please save first.");
            //    return;
            //}

            //try
            //{
            //    using (var image = new MagickImage(imgpath))
            //    {
            //        var profile = image.GetProfile("xmp");
            //        if (profile == null)
            //        {
            //            MessageBox.Show("❌ No XMP metadata found!");
            //            return;
            //        }

            //        string xmpXml = Encoding.UTF8.GetString(profile.ToByteArray());

            //        if (xmpXml.Contains("<custom:PhotoInfo>"))
            //        {
            //            int start = xmpXml.IndexOf("<custom:PhotoInfo>") + "<custom:PhotoInfo>".Length;
            //            int end = xmpXml.IndexOf("</custom:PhotoInfo>");
            //            string json = xmpXml.Substring(start, end - start);

            //            // Decode and deserialize
            //            string decodedJson = System.Net.WebUtility.HtmlDecode(json);
            //            try
            //            {
            //                dynamic meta = JsonConvert.DeserializeObject(decodedJson);
            //                tbperson.Text = meta.Person ?? "";
            //                tbevent.Text = meta.Event ?? "";
            //                tblocation.Text = meta.Location ?? "";
            //                dateTimePicker1.Value = meta.Date ?? DateTime.Now;
            //            }
            //            catch
            //            {
            //                MessageBox.Show("Corrupted or invalid metadata JSON!");
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("No PhotoInfo found inside XMP!");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error reading metadata: " + ex.Message);
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tblocation_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {


        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    var form = new Event();
        //    form.Show();
        //}

        //private void button7_Click(object sender, EventArgs e)
        //{
        //    var form = new DATE();
        //    form.Show();
        //}

        //private void button8_Click(object sender, EventArgs e)
        //{
        //    var f = new SEARCH();
        //    f.Show();
        //}
    }
    public class face
    {
        public string name { get; set; }
        public string id { get; set; }


    }


}
