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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Media.Imaging;

namespace LearningApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void medium(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }
        private void encode(object sender, RoutedEventArgs e)
        {
            uint one = 1;
            byte full = 0xFF;
            byte empty = 0xFE;
            //Convert the UploadPage.HiddenData to a bitwise-AND compatible array of bytes
            byte[] ands = new byte[UploadPage.HiddenData.Length * 8];
            int andsCounter = 0;
            for (int i = 0; i < UploadPage.HiddenData.Length; i++)
            {
                int shifty = UploadPage.HiddenData[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((shifty & one) == one)
                    {
                        ands[andsCounter++] = full;
                    }
                    else
                    {
                        ands[andsCounter++] = empty;
                    }
                    shifty = shifty >> 1;
                }
            }

            //Compare the two byte arrays
            for (int i = 0; i < GamePage.ImageData.Length && i < ands.Length; i++)
            {
                GamePage.ImageData[i] = (byte)(GamePage.ImageData[i] & ands[i]);
            }
            WriteableBitmap converter = new WriteableBitmap(GamePage.ImageWidth, GamePage.ImageHeight);
            int[] copy = converter.Pixels;
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = BitConverter.ToInt32(GamePage.ImageData, i * 4);
            }
            System.IO.MemoryStream tempStream = new System.IO.MemoryStream();
            converter.SaveJpeg(tempStream, GamePage.ImageWidth, GamePage.ImageHeight, 0, 100);

            tempStream.Seek(0, System.IO.SeekOrigin.Begin);
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(tempStream))
            {
                GamePage.ImageData = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void decode(object sender, RoutedEventArgs e)
        {
            //Construct the result
            int count1 = 0;
            int count2 = 0;
            uint one = 1;
            byte[] result = new byte[GamePage.ImageData.Length / 8];
            byte temp = 0;
            for (int i = 0; i < GamePage.ImageData.Length; i++)
            {
                if (i % 4 == 0)
                {
                    i += 2;
                }
                else if (i % 4 == 2)
                {
                    i -= 2;
                }

                if ((GamePage.ImageData[i] & one) == 1)
                {
                    temp = (byte)((temp << 1) + 1);
                    count1++;
                }
                else
                {
                    temp = (byte)(temp << 1);
                    count2++;
                }
                if (i % 8 == 7)
                {
                    result[i / 8] = temp;
                    temp = 0;
                }

                if (i % 4 == 2)
                {
                    i -= 2;
                }
                else if (i % 4 == 0)
                {
                    i += 2;
                }
            }

            for (int i = 1; i < result.Length; i += 2)
            {
                result[i] = 0;
            }

            String message = "";
            //Parse the result
            for (int i = 0; i < (result.Length) / 2 - 1; i += 2)
            {
                message += BitConverter.ToChar(result, i * 2);
            }
            MessageCallout.Content = message;
            FadeIn.Begin();
        }
    }
}