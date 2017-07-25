using System;
using System.Windows.Media.Imaging;

namespace SocialManager_Client
{
    static class PathUtilities
    {
        public static BitmapImage GetImageSource(string imageName)
        {
            return new BitmapImage(
                            new Uri(Environment.CurrentDirectory + @"\..\..\..\Images\" + imageName, 
                                    UriKind.Absolute));
        }

        public static BitmapImage GetImageSourceFromUri(string uri)
        {
            return new BitmapImage(
                            new Uri(uri,
                                    UriKind.RelativeOrAbsolute));
        }
    }
}