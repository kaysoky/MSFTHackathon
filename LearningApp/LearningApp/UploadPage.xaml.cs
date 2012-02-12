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

namespace LearningApp
{
    public partial class UploadPage : PhoneApplicationPage
    {
        public static byte[] HiddenData = new byte[0];

        //private TextBox ToBeHidden = new TextBox();
        public UploadPage()
        {
            //ToBeHidden.Name = "ToBeHidden";
            InitializeComponent();
        }

        private void ToBeHidden_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ToBeHidden.Text == "Insert text to be steganographically hidden...")
            {
                ToBeHidden.Text = "";
                //SolidColorBrush Brush1 = new SolidColorBrush();
                //Brush1.Color = Colors.Magenta;
                //ToBeHidden.Foreground = Brush1;
            }
        }

        private void ToBeHidden_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ToBeHidden.Text == String.Empty)
            {
                ToBeHidden.Text = "Insert text to be steganographically hidden...";
                //SolidColorBrush Brush2 = new SolidColorBrush();
                //Brush2.Color = Colors.Blue;
                //ToBeHidden.Foreground = Brush2;
            }
        }
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            char[] intermediateConversion = ToBeHidden.Text.ToCharArray();
            HiddenData = new byte[intermediateConversion.Length * sizeof(char) / sizeof(byte)];
            int byteCounter = 0;
            for (int i = 0; i < intermediateConversion.Length; i++)
            {
                byte[] converted = BitConverter.GetBytes(intermediateConversion[i]);
                for (int j = 0; j < converted.Length; j++)
                {
                    HiddenData[byteCounter++] = converted[j];
                }
            }

            base.OnNavigatingFrom(e);
        }
    }
}
