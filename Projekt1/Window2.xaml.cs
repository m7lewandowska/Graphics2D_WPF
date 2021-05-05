using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using Microsoft.Win32;

namespace Projekt1
{
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            infoLabel.Content = "please wait a few seconds for the image to load";
            if (op.ShowDialog() == true)
            {
                basicImage.Source = new BitmapImage(new Uri(op.FileName));
                
                Bitmap bitmap;
                int[,] R, G, B;

                string path = ((BitmapImage)basicImage.Source).UriSource.AbsolutePath;
                path = path.Replace("/", "\\");

                bitmap = new Bitmap(path);

                R = new int[bitmap.Width, bitmap.Height];
                G = new int[bitmap.Width, bitmap.Height];
                B = new int[bitmap.Width, bitmap.Height];

                for (int i = 0; i < bitmap.Width; i++)
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        R[i, j] = bitmap.GetPixel(i, j).R;
                        G[i, j] = bitmap.GetPixel(i, j).G;
                        B[i, j] = bitmap.GetPixel(i, j).B;
                    }

                Bitmap newBitmap = new Bitmap(bitmap);
                int[,] kernel = new int[,] { { 1, 1, 1 }, { 1, 12, 1 }, { 1, 1, 1 } };
                double red, green, blue;

                for (int i = 1; i < bitmap.Width - 1; i++)
                    for (int j = 1; j < bitmap.Height - 1; j++)
                    {
                        red = 0.0;
                        green = 0.0;
                        blue = 0.0;

                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                red += R[i - 1 + l, j - 1 + k] * kernel[k, l] / 16;
                                green += G[i - 1 + l, j - 1 + k] * kernel[k, l] / 16;
                                blue += B[i - 1 + l, j - 1 + k] * kernel[k, l] / 16;
                            }
                        }

                        red %= 255;
                        green %= 255;
                        blue %= 255;
                        newBitmap.SetPixel(i, j, System.Drawing.Color.FromArgb((int)red, (int)green, (int)blue));
                    }

                finalImage.Source = BitmapToImageSource(newBitmap);
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
