using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Calc.MVVM.Helpers
{
    public static class ImageHelper
    {
        private static readonly BitmapImage noImage = new BitmapImage(new Uri("pack://application:,,,/CalcMVVM;component/Resources/no_image.png"));


        public static BitmapImage ByteArrayToBitmap(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            using (var ms = new MemoryStream(imageData))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public static BitmapImage GetNoImage()
        {
            return noImage;
        }
    }
}
