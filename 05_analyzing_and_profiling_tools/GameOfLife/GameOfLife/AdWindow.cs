using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOfLife
{
    class AdWindow : Window
    {
        private static readonly Random rnd = new Random();
        private readonly DispatcherTimer adTimer;
        private int imgNmb;     // the number of the image currently shown
        private string link;    // the URL where the currently shown ad leads to        

        private readonly ImageBrush[] adBrushes;
        private readonly string[] adLinks;

        public AdWindow(Window owner)
        {
            Owner = owner;
            Width = 350;
            Height = 100;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
            Title = "Support us by clicking the ads";
            Cursor = Cursors.Hand;
            ShowActivated = false;
            MouseDown += OnClick;

            adBrushes = new ImageBrush[3];
            adLinks = new string[3];

            InitAdBrushes();

            imgNmb = rnd.Next(1, 3);
            ChangeAds(this, new EventArgs());

            // Run the timer that changes the ad's image 
            adTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            adTimer.Tick += ChangeAds;
            adTimer.Start();
        }

        private void InitAdBrushes()
        {
            for (int imageNumber = 0; imageNumber < 3; imageNumber++)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri($"ad{imageNumber + 1}.jpg", UriKind.Relative);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                adBrushes[imageNumber] = new ImageBrush(bitmap);
                adBrushes[imageNumber].Freeze();
                adLinks[imageNumber] = "http://example.com";
            }
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(link);
            Close();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            Unsubscribe();
            Owner = null;
            base.OnClosed(e);
        } 

        public void Unsubscribe()
        {
            adTimer.Stop();
            adTimer.Tick -= ChangeAds;
            MouseDown -= OnClick;
        }

        private void ChangeAds(object sender, EventArgs eventArgs)
        {
            Background = adBrushes[imgNmb];
            link = adLinks[imgNmb];
            imgNmb = (imgNmb + 1) % 3;
        }
    }
}