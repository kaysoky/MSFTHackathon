﻿using System;
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

            //Convert the ImageData into JPEG format
            Texture2D converter = new Texture2D(SharedGraphicsDeviceManager.Current.GraphicsDevice
                , GamePage.ImageWidth
                , GamePage.ImageHeight);
            converter.SetData<byte>(GamePage.ImageData);
            System.IO.Stream tempStream = new System.IO.MemoryStream();
            converter.SaveAsJpeg(tempStream, converter.Height, converter.Width);
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(tempStream))
            {
                GamePage.ImageData = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }
    }
}