using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ImageToContentRead
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ImageClass> _imagesList;
        public List<ImageClass> ImagesList
        {
            get { return _imagesList; }
            set
            {
                _imagesList = value;
                OnPropertyChanged();
            }
        }
        private readonly string _pathToTestData = @"E:\\Office Working Folder C#\\Document Management Main Source Code\\tesseract-ocr DLL\\tessdata";
        TesseractOcr _tesseractOrc;


        public MainWindow()
        {
            InitializeComponent();
            _tesseractOrc = new TesseractOcr("eng");
            _imagesList = new List<ImageClass>();
        }

        private void Btn_select_image_Click(object sender, RoutedEventArgs e)
        {
            OpenImages(sender);
        }

        public void OpenImages(object obj)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                {
                    Multiselect = true,
                };
                bool? result = dlg.ShowDialog();
                if (result != true)
                    return;

                var filenames = dlg.FileNames;


                ImagesList.Clear();

                var imageLoader = new ImageLoader();
                ImagesList.AddRange(imageLoader.LoadImages(filenames.ToList()));


                //NextImage(obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        public void BeginOcr(object obj)
        {
            if (!Directory.Exists(_pathToTestData))
            {
                MessageBox.Show("Ooops. Test image not found (no big deal, everything else should still work fine).\nI looked in: " + _pathToTestData);
                return;
            }


            if (_imagesList.Count == 0)
                return;

            Task.Factory.StartNew(delegate
            {
                try
                {
                    RecognizedText = @"OCR started... ";

                    OnRecoginedEvent(_recognizedText);

                    var recognizedText = new List<string>();
                    if (IsRecognizeAll)
                    {
                        var tempText = _tesseractOrc.RecognizeFewImages(_imagesList);
                        recognizedText.AddRange(tempText.Where(text => text != null));
                        int cnt = 1;
                        foreach (var imageClass in _imagesList)
                        {
                            OnRecoginedEvent(String.Format("Image {0} of {1}", cnt++, _imagesList.Count));
                            var tt = _tesseractOrc.RecognizeOneImage(imageClass);
                            recognizedText.Add(tt);
                        }
                    }
                    else
                    {
                        var tempText = _tesseractOrc.RecognizeOneImage(_imagesList[0]);
                        if (tempText == null)
                            return;

                        recognizedText.Add(tempText);
                    }

                    _recognizedText = string.Empty;
                    foreach (var text in recognizedText)
                    {
                        _recognizedText = string.Concat(_recognizedText, text);
                    }
                    RecognizedText = _recognizedText;
                    OnRecoginedEvent(_recognizedText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            });
        }

        private int _currentImageNumber;
        public int CurrentImageNumber
        {
            get
            {
                if (_imagesList.Count != 0)
                    CurrentImage = _imagesList[_currentImageNumber];
                return _currentImageNumber;
            }
            set { _currentImageNumber = value; }
        }

        private ImageClass _currentImage;
        public ImageClass CurrentImage
        {
            get
            {
                return _currentImage;
            }
            set
            {
                _currentImage = value;
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("CurrentImage");
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("CurrentImageText");
            }
        }

        private String _recognizedText;
        public String RecognizedText
        {
            get
            {
                return _recognizedText;
            }
            set
            {
                _recognizedText = value;
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("RecognizedText");
            }
        }
        private bool _isRecognizeAll;
        public bool IsRecognizeAll
        {
            get
            {
                return _isRecognizeAll;
            }
            set
            {
                if (_isRecognizeAll == value)
                    return;
                _isRecognizeAll = value;
                // ReSharper disable once RedundantArgumentDefaultValue
                OnPropertyChanged("IsRecognizeAll");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event RecognizedTextChanged RecoginedEvent;
        protected virtual void OnRecoginedEvent(string text)
        {
            var handler = RecoginedEvent;
            if (handler != null)
            {
                var ev = new RecognizedTextEventArgs { ChangedText = text };
                handler(this, ev);
            }
        }

        private void Btn_process_ocr_Click(object sender, RoutedEventArgs e)
        {
            BeginOcr(sender);
        }
    }

    public delegate void RecognizedTextChanged(object sender, RecognizedTextEventArgs args);
    public class RecognizedTextEventArgs : EventArgs
    {
        public string ChangedText;
    }
}
