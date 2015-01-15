using Acr.XamForms.Mobile.Media;
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

namespace CardScannerX
{
    public partial class CameraPage : ContentPage
    {
        public CameraPage()
        {
            InitializeComponent();
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
                    imgCaptured.Source = file.Path;
                    byte[] fileBytes = ReadFully(file.GetStream());
                    size = fileBytes.Length;

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
                    MediaTypeHeaderValue.Parse("image/jpeg");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data,boundary=-------------------------acebdf13572468"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/x-vcard"));



                requestContent.Add(imageContent, "image", "image.jpg");
                string url = "http://bcr1.intsig.net/BCRService/BCR_VCF2?PIN=&user=supreet.tare@taritas.com&pass=6RPY9KPCCFGM54F9&lang=1&size=" + size;

                return await client.PostAsync(url, requestContent);

            }
            catch (Exception ex)
            {
                var t = ex.Message;
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
            
        }
    }
}
