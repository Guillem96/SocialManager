using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfAnimatedGif;

namespace SocialManager_Client
{
    static class Loading
    {
        public static void StartLoading(double windowHeight, double windowWidth, Grid loadingGrid, String message, int loadingImageHeight = 150, int loadingImageWidth = 150)
        {
            // Set loading image
            loadingGrid.Height = windowHeight;
            loadingGrid.Width = windowWidth;
            loadingGrid.Background = System.Windows.Media.Brushes.White;
            StackPanel sp = new StackPanel()
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };

            // Load Image
            System.Windows.Controls.Image load = new System.Windows.Controls.Image()
            {
                Width = loadingImageWidth,
                Height = loadingImageHeight,
                Margin = new System.Windows.Thickness(-10)
            };
            ImageBehavior.SetAnimatedSource(load, PathUtilities.GetImageSource("loading.gif"));
            sp.Children.Add(load);

            // Label
            Label l = new Label()
            {
                Content = message,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };
            sp.Children.Add(l);

            loadingGrid.Children.Add(sp);
        }

        public static void EndLoading(Grid loadingGrid)
        {
            loadingGrid.Background = System.Windows.Media.Brushes.Transparent;
            loadingGrid.Children.Clear();
            loadingGrid.Height = 0;
            loadingGrid.Width = 0;
        }

    }
}
