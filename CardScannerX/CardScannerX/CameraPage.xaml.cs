using Acr.XamForms.Mobile.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        private void OnPhotoReceived(IMediaFile file)
        {
            if (file == null)
            {
                //this.dialogs.Alert("Photo Cancelled");
            }
            else
            {
                this.Photo = ImageSource.FromFile(file.Path);
                imgCaptured.Source = file.Path;
            }
        }
    }
}
