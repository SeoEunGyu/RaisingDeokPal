using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace RasingDeokPal.Components
{
    internal class ImageComponent
    {
        Image img;
        public ImageComponent(string uri)
        {
            img = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri, UriKind.Absolute);
            bitmap.EndInit();
            img.Source = bitmap;
        }

        public Image Get()
        {
            return img;
        }
    }
}
