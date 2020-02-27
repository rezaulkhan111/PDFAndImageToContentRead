using System;
using System.Collections.Generic;
using System.Text;

namespace ImageToContentRead
{
    public interface ITesseractOrc
    {
        string RecognizeOneImage(ImageClass image);
        List<string> RecognizeFewImages(List<ImageClass> images);
        void ChangeLanguage(string lang);
    }
}
