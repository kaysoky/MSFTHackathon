using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace LearningApp
{
    public partial class GamePage : PhoneApplicationPage
    {
        private ContentManager contentManager;
        private GameTimer timer;
        private SpriteBatch spriteBatch;

        private Texture2D BlankWhiteTexture;
        private SpriteFont Kootenay;
        private List<ImageFile> loadedPictures = new List<ImageFile>();

        public static byte[] ImageData;

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadMedia));
        }

        /// <summary>
        /// Loads each picture (as a Texture2D) in the user's picture library and puts it into 'loadedPictures'
        /// </summary>
        /// <param name="argument">No argument taken</param>
        protected void LoadMedia(Object argument)
        {
            try
            {
                MediaSource librarySource = MediaSource.GetAvailableMediaSources().First<MediaSource>();
                MediaLibrary mediumSource = new MediaLibrary(librarySource);
                PictureCollection collection = mediumSource.Pictures;
                for (int i = 0; i < collection.Count; i++)
                {
                    ImageFile displayData = new ImageFile();
                    displayData.Picture = Texture2D.FromStream(SharedGraphicsDeviceManager.Current.GraphicsDevice
                        , collection[i].GetImage());
                    displayData.Width = collection[i].Width;
                    displayData.Height = collection[i].Height;
                    loadedPictures.Add(displayData);
                }
                loadedPictures.AddRange(loadedPictures);
            }
            catch (Exception) { }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // TODO: use this.content to load your game content here
            Kootenay = this.contentManager.Load<SpriteFont>("Kootenay");
            BlankWhiteTexture = new Texture2D(SharedGraphicsDeviceManager.Current.GraphicsDevice, 1, 1);
            BlankWhiteTexture.SetData<Color>( new Color[] { Color.White } );

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            //Save the image's data to a shared static variable
            ImageData = new byte[loadedPictures[selectedImage].Picture.Width * loadedPictures[selectedImage].Picture.Height];
            loadedPictures[selectedImage].Picture.GetData<byte>(ImageData, 0, ImageData.Length);

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        const int Default_Margin = 5;
        private int heightOffset = Default_Margin;
        private int numPicturePerRow = 2;
        private int bottomHeightBound = 2 * Default_Margin;
        private int selectedImage = -1;
        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            //Enable dragging of the screen
            TouchPanel.EnabledGestures = GestureType.VerticalDrag;
            if (TouchPanel.IsGestureAvailable)
            {
                GestureSample action = TouchPanel.ReadGesture();
                heightOffset += (int)(action.Delta - action.Delta2).Y;
                if (heightOffset < -bottomHeightBound)
                {
                    heightOffset = -bottomHeightBound;
                }
                if (heightOffset >= Default_Margin)
                {
                    heightOffset = Default_Margin;
                }
            }

            //Enable selection of images
            TouchPanel.EnabledGestures = GestureType.Tap;
            if (TouchPanel.IsGestureAvailable)
            {
                GestureSample action = TouchPanel.ReadGesture();
                Microsoft.Xna.Framework.Point tap = new Microsoft.Xna.Framework.Point((int)action.Position.X, (int)action.Position.Y);
                for (int i = 0; i < loadedPictures.Count; i++)
                {
                    //Only check picture within the frame
                    if (loadedPictures[i].DrawingRegion != null 
                        && loadedPictures[i].DrawingRegion.Y + loadedPictures[i].DrawingRegion.Height > 0
                        && loadedPictures[i].DrawingRegion.Y < SharedGraphicsDeviceManager.DefaultBackBufferHeight) 
                    {
                        if (loadedPictures[i].DrawingRegion.Contains(tap))
                        {
                            selectedImage = i;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Purple);

            int drawWidth = (SharedGraphicsDeviceManager.DefaultBackBufferWidth - (numPicturePerRow + 1) * Default_Margin) / numPicturePerRow;

            spriteBatch.Begin();

            int currentHeightOffset = heightOffset;
            int currentRowNum = 0;
            int currentloadedPictures = loadedPictures.Count;
            int largestDrawHeight = 0;

            for (int i = 0; i < currentloadedPictures; i++)
            {
                //Stack all pictures in rows and columns
                int drawHeight = (int)((double)drawWidth / loadedPictures[i].Width * loadedPictures[i].Height);
                loadedPictures[i].DrawingRegion = 
                    new Rectangle(Default_Margin * (currentRowNum + 1) + currentRowNum * drawWidth
                        , currentHeightOffset
                        , drawWidth
                        , drawHeight);
                spriteBatch.Draw(loadedPictures[i].Picture
                    , loadedPictures[i].DrawingRegion
                    , Color.White);

                //Keep track of rows
                if (drawHeight > largestDrawHeight)
                {
                    largestDrawHeight = drawHeight;
                }
                //Increment row/column
                currentRowNum++;
                if (currentRowNum >= numPicturePerRow)
                {
                    currentRowNum = 0;
                    currentHeightOffset += largestDrawHeight + Default_Margin;
                    largestDrawHeight = 0;
                }
            }
            //Determine how large the stack of images is
            currentHeightOffset += largestDrawHeight;  //Make sure that the last row is accounted for
            bottomHeightBound = currentHeightOffset - heightOffset - SharedGraphicsDeviceManager.DefaultBackBufferHeight;

            //Highlight the selected image
            if (selectedImage >= 0)
            {
                spriteBatch.Draw(BlankWhiteTexture, loadedPictures[selectedImage].DrawingRegion, new Color(1.0f, 1.0f, 1.0f, 0.2f));
            }
            spriteBatch.End();
        }
    }

    class ImageFile
    {
        public Texture2D Picture;
        public int Width;
        public int Height;
        public Rectangle DrawingRegion;
    }
}