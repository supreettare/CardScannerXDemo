using Acr.XamForms.Mobile.Media;
using Acr.XamForms.Mobile.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using PCLStorage;
using Abbyy.CloudOcrSdk;

namespace CardScannerX
{
    public partial class CameraPage : ContentPage
    {
        RestServiceClientAsync abbyyClient;
        public CameraPage()
        {
            InitializeComponent();
            //RestServiceClient syncClient = new RestServiceClient();
            //syncClient.ApplicationId = "APP_NAME";
            //syncClient.Password = "API_KEY";

            //abbyyClient = new RestServiceClientAsync(syncClient);

            //abbyyClient.UploadFileCompleted += UploadCompleted;
            //abbyyClient.TaskProcessingCompleted += ProcessingCompleted;
            //abbyyClient.DownloadFileCompleted += DownloadCompleted;

        }

        //private void UploadCompleted(object sender, UploadCompletedEventArgs e)
        //{
        //    //DisplayAlert("Upload Completed", "Processing", "Okay");

        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        displayMessage("Upload completed. Processing..");
        //    });
        //}

        //private void ProcessingCompleted(object sender, TaskEventArgs e)
        //{
        //    if (e.Error != null)
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            displayMessage("Processing error: " + e.Error.Message);
        //        });
                

        //        return;
        //    }

        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        displayMessage("Processing completed. Downloading..");
        //    });
            


        //    // Download a file
        //    string outputPath = "result.txt";
        //    Abbyy.CloudOcrSdk.Task task = e.Result;
        //    abbyyClient.DownloadFileAsync(task, outputPath, outputPath);
        //}

        //private void DownloadCompleted(object sender, TaskEventArgs e)
        //{
        //    string message = "";
        //    if (e.Error != null)
        //    {
        //        message = "Error downloading: " + e.Error.Message;
        //    }
        //    else
        //    {
        //        message = "Downloaded.\nResult:";

        //        string txtFilePath = e.UserState as string;
        //        var file = DependencyService.Get<ISaveStreamToStorage>().OpenFile(txtFilePath);
        //        using (StreamReader reader = new StreamReader(file))
        //        {
        //            message += reader.ReadToEnd();
        //        }
        //    }

        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        displayMessage(message);
        //    });

            
        //}

        private void displayMessage(string text)
        {
            statusMsg.Text = text;
        }

        public async void OnButtonClicked(object sender, EventArgs e)
        {
            //await this.Navigation.PushAsync(new ForgotPassword());

            if (DependencyService.Get<IMediaPicker>().IsCameraAvailable)
            {
                var result = await DependencyService.Get<IMediaPicker>().PickPhoto();
                this.OnPhotoReceived(result);
            }
        }

        public async void OnButtonClicked2(object sender, EventArgs e)
        {
            //await this.Navigation.PushAsync(new ForgotPassword());

            if (DependencyService.Get<IMediaPicker>().IsCameraAvailable)
            {
                var result = await DependencyService.Get<IMediaPicker>().TakePhoto();
                this.OnPhotoReceived(result);
            }
        }

        private ImageSource photoSource;
        public ImageSource Photo
        {
            get { return this.photoSource; }
            private set
            {
                this.photoSource = value;
                this.OnPropertyChanged();
            }
        }

        int size = 0;


        private async void OnPhotoReceived(IMediaFile file)
        {
            try
            {
                if (file == null)
                {
                    //this.dialogs.Alert("Photo Cancelled");
                }
                else
                {
                    this.Photo = ImageSource.FromFile(file.Path);
                    byte[] fileBytes = ReadFully(file.GetStream());
                    size = fileBytes.Length;

                    //string localPath = "image.jpg";
                    //DependencyService.Get<ISaveStreamToStorage>().SaveImageToFile(file.GetStream(), localPath);
                    //ProcessingSettings settings = new ProcessingSettings();
                    //settings.SetLanguage("English,Russian");
                    //settings.OutputFormat = OutputFormat.xml;
                    


                    //displayMessage("Uploading..");
                    //abbyyClient.ProcessImageAsync(localPath, settings, settings);

                    var response = await Upload(fileBytes, size);
                }
            }
            catch (Exception ex)
            {

                var m = ex.Message;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task<HttpResponseMessage> Upload(byte[] image, int size)
        {

            try
            {
                var client = new HttpClient();
                var requestContent = new MultipartFormDataContent();
                //    here you can specify boundary if you need---^

                var imageContent = new ByteArrayContent(image);
                imageContent.Headers.ContentType =
                    MediaTypeHeaderValue.Parse("image/jpg");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data,boundary=-------------------------acebdf13572468"));
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/x-vcard"));



                requestContent.Add(imageContent, "image", "image.jpg");

                string abbyyUrl = "https://cloud.ocrsdk.com/processBusinessCard";
                string toEncode = "MY_APP_NAME" + ":" + "MY_API_KEY";
                Encoding isoEncoding = Encoding.GetEncoding("iso-8859-1");
                var isoEncoded = isoEncoding.GetBytes(toEncode);
                String baseEncoded = Convert.ToBase64String(isoEncoded);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", baseEncoded);



                //string url = "http://bcr1.intsig.net/BCRService/BCR_VCF2?PIN=&user=supreet.tare@taritas.com&pass=6RPY9KPCCFGM54F9&lang=1&size=" + size;

                return await client.PostAsync(abbyyUrl, requestContent);

            }
            catch (Exception ex)
            {
                var t = ex.Message;
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

        }
    }
}
