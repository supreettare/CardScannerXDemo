using CardScannerX.WinPhone;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

[assembly: Xamarin.Forms.Dependency(typeof(ISaveStreamToStorage_WP))]
namespace CardScannerX.WinPhone
{   
    class ISaveStreamToStorage_WP : ISaveStreamToStorage
    {
        public void SaveImageToFile(Stream imageStream, string localPath)
        {
            imageStream.Seek(0, SeekOrigin.Begin);

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(localPath))
                    storage.DeleteFile(localPath);

                using (IsolatedStorageFileStream file = storage.CreateFile(localPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(imageStream);
                    WriteableBitmap wb = new WriteableBitmap(bitmap);
                    wb.SaveJpeg(file, wb.PixelWidth, wb.PixelHeight, 0, 85);
                }
            }
        }

        public Stream OpenFile(string txtFilePath)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream file = storage.OpenFile(txtFilePath, FileMode.Open, FileAccess.Read);

            return file;
        }
    }
}
