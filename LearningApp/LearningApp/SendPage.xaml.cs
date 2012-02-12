using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
//used to save the picture
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone;
using Microsoft.Xna.Framework.Media;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;

namespace LearningApp
{
    public partial class SendPage : PhoneApplicationPage
    {
        public SendPage()
        {
            
            InitializeComponent();
        }
        
        private void share(object sender, RoutedEventArgs e)
        {
            //Create a filename for the JPEG file in isolated storage
            String tempJPEG = "TempJPEG";

            // Create a virtual store and file stream. Check for duplicate tempJPEG files.
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(tempJPEG))
            {
                myStore.DeleteFile(tempJPEG);
            }

            IsolatedStorageFileStream myFileStream = myStore.CreateFile(tempJPEG);


            // Create a stream out of the sample JPEG file.
            // For [Application Name] in the URI, use the project name that you entered
            // in the previous steps. Also, TestImage.jpg is an example;
            // you must enter your JPEG file name if it is different.
            StreamResourceInfo sri = null;
            Uri uri = new Uri("[Application Name];component/TestImage.jpg", UriKind.Relative);
            sri = Application.GetResourceStream(uri);

            // Create a new WriteableBitmap object and set it to the JPEG stream.
            BitmapImage bitmap = new BitmapImage();
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.SetSource(sri.Stream);
            WriteableBitmap wb = new WriteableBitmap(bitmap);

            // Encode the WriteableBitmap object to a JPEG stream.
            wb.SaveJpeg(myFileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
            myFileStream.Close();

            // Create a new stream from isolated storage, and save the JPEG file to the media library on Windows Phone.
            myFileStream = myStore.OpenFile(tempJPEG, FileMode.Open, FileAccess.Read);

            // Save the image to the camera roll or saved pictures album.
            MediaLibrary library = new MediaLibrary();

            // Save the image to the camera roll album.
            Picture pic = library.SavePictureToCameraRoll("SavedPicture.jpg", myFileStream);
            MessageBox.Show("Image saved to camera roll album");
            /* // Save the image to the saved pictures album.
Picture pic = library.SavePicture("SavedPicture.jpg", myFileStream);
MessageBox.Show("Image saved to saved pictures album");
*/

            myFileStream.Close();
        }
    }
}
