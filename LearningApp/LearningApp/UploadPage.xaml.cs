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
using System.Windows.Media.Imaging;

namespace LearningApp
{
    public partial class UploadPage : PhoneApplicationPage
    {
        public static byte[] HiddenData;

        public UploadPage()
        {
            InitializeComponent();
        }

        private void navToPictures(object Sender, RoutedEventArgs)
        {
            MediaLibrary ml = new MediaLibrary();

            if (ml.Pictures.Count > 0)
            {
                System.IO.Stream sm = ml.Pictures[0].GetImage();
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(sm);
                imagecontrol.Source = bmp;
            }            
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            
            base.OnBackKeyPress(e);
        }
    }
}
