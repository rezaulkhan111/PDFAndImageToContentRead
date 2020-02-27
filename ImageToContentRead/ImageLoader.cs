using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageToContentRead
{
    class ImageLoader
    {
        public List<ImageClass> LoadImages(List<string> paths)
        {
            var imagesList = new List<ImageClass>();
            foreach (var name in paths)
            {
                try
                {
                    var uri = new Uri(name);
                    var bitmap = new BitmapImage(uri);
                    imagesList.Add(
                        new ImageClass
                        {
                            FilePath = name,
                            Image = bitmap
                        });
                }
                catch
                {
                    // ignored
                }
            }
            return imagesList;
        }
    }
}
