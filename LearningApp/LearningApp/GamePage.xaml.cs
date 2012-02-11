using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private SpriteFont kootenay;

        private List<ImageFile> loadedPictures = new List<ImageFile>();

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            //kootenay = contentManager.Load<SpriteFont>("Kootenay");

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
                loadedPictures.AddRange(loadedPictures);
            }
            catch (Exception accessDenied)
            {
                
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // TODO: use this.content to load your game content here

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Add your update logic here
            TouchPanel.EnabledGestures = GestureType.VerticalDrag;
            if (TouchPanel.IsGestureAvailable)
            {
                GestureSample action = TouchPanel.ReadGesture();
                switch (action.GestureType)
                {
                    case GestureType.DoubleTap:
                        break;
                    case GestureType.DragComplete:
                        break;
                    case GestureType.Flick:
                        break;
                    case GestureType.FreeDrag:
                        break;
                    case GestureType.Hold:
                        break;
                    case GestureType.HorizontalDrag:
                        break;
                    case GestureType.None:
                        break;
                    case GestureType.Pinch:
                        break;
                    case GestureType.PinchComplete:
                        break;
                    case GestureType.Tap:
                        break;
                    case GestureType.VerticalDrag:
                        heightOffset += (int)(action.Delta - action.Delta2).Y;
                        //if (heightOffset > Default_Margin)
                        //{
                        //    heightOffset = Default_Margin;
                        //}
                        //else if (-heightOffset < bottomHeightBound)
                        //{
                        //    heightOffset = -bottomHeightBound;
                        //}
                        break;
                    default:
                        break;
                }
            }
        }

        const int Default_Margin = 5;
        private int heightOffset = Default_Margin;
        private int numPicturePerRow = 2;
        private int bottomHeightBound = 2 * Default_Margin;
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
                spriteBatch.Draw(loadedPictures[i].Picture
                    , new Microsoft.Xna.Framework.Rectangle(Default_Margin * (currentRowNum + 1) + currentRowNum * drawWidth
                        , currentHeightOffset
                        , drawWidth
                        , drawHeight)
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
            bottomHeightBound = currentHeightOffset;

            spriteBatch.DrawString(kootenay, "Debug:/n" + heightOffset, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }

    struct ImageFile
    {
        public Texture2D Picture;
        public int Width;
        public int Height;
    }
}