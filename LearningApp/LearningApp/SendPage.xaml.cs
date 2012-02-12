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
using Microsoft.Xna.Framework.Media;

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
            try
            {
                MediaSource librarySource = MediaSource.GetAvailableMediaSources().First<MediaSource>();
                MediaLibrary gallery = new MediaLibrary(librarySource);
                gallery.SavePicture("Title", GamePage.ImageData);
            }
            catch (Exception)
            {
                ExceptionCallout.Content = "Save failed...\nPlease try again";
                FadeIn.Begin();
            }
        }
    }
}
