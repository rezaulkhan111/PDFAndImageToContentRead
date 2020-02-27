using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageToContentRead
{
   public class ImageClass
    {
        public string FilePath { get; set; }

        public BitmapImage Image { get; set; }

        public ImageClass()
        {
            Image = new BitmapImage();
        }
    }
}
