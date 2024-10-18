using RasingDeokPal.Effect;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RasingDeokPal.Common
{
    /// <summary>
    /// 전역으로 적용되는 UI 컨트롤 관련 함수
    /// </summary>
    internal class UIControl
    {
        /// <summary>
        /// 이미지 생성
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Image CreateImage(string uri)
        {
            Image img = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri, UriKind.Absolute);
            bitmap.EndInit();
            img.Source = bitmap;
            img.Width = bitmap.Width;
            img.Height = bitmap.Height;
            return img;
        }
        public static Image CreateImage(string uri, int width, int height)
        {
            Image img = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri, UriKind.Absolute);
            bitmap.EndInit();
            img.Source = bitmap;
            img.Width = width;
            img.Height = height;
            return img;
        }
        public static Image CreateImage(BitmapImage bitmap)
        {
            Image img = new Image();
            img.Source = bitmap;
            img.Width = bitmap.Width;
            img.Height = bitmap.Height;
            return img;
        }
        /// <summary>
        /// 비트맵 이미지 생성
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static BitmapImage CreateBitmap(string uri)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri, UriKind.Absolute);
            bitmap.EndInit();
            
            return bitmap;
        }
        public static BitmapImage CreateBitmap(string uri, int width, int height)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri, UriKind.Absolute);
            bitmap.DecodePixelWidth = width;
            bitmap.DecodePixelHeight = height;
            bitmap.EndInit();

            return bitmap;
        }

        /// <summary>
        /// Media 요소 반환
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MediaElement CreateMediaAbsolute(string path)
        {
            //string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            //string videoPath = Path.Combine(projectRoot, path);

            MediaElement media = new MediaElement();
            media.Source = new Uri(path, UriKind.Absolute);
            media.LoadedBehavior = MediaState.Manual;
            media.UnloadedBehavior = MediaState.Stop;
            media.Stretch = Stretch.Uniform;
            return media;
        }
        public static MediaElement CreateMediaRelative(string path)
        {
            MediaElement media = new MediaElement();
            media.Source = new Uri(path, UriKind.Relative);
            media.LoadedBehavior = MediaState.Manual;
            media.UnloadedBehavior = MediaState.Stop;
            media.Stretch = Stretch.Uniform;
            return media;
        }
        /// <summary>
        /// 미디어 소스 설정
        /// </summary>
        /// <param name="media"></param>
        /// <param name="path"></param>
        public static void SetMediaSource(MediaElement media, string path)
        {
            media.Source = new Uri(path, UriKind.Relative);
        }

        /// <summary>
        /// UI visible 설정 함수
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public static void SetVisibility(UIElement target, bool value)
        {
            target.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Canvas 내부 left, top 설정
        /// </summary>
        /// <param name="target"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public static void SetCanvasMargin(UIElement target, int left, int top) 
        {
            Canvas.SetLeft(target, left);
            Canvas.SetTop(target, top);
        }

        /// <summary>
        /// Image 중심 좌표 RGB 값 반환
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int[] GetImageColor(Image image)
        {
            WriteableBitmap writeableBitmap;
           
            int x = (int)(image.Source.Width / 2);
            int y = (int)(image.Source.Height / 2);

            if (image.Source is BitmapSource bitmapSource)
            {
                writeableBitmap = new WriteableBitmap(bitmapSource);
            }
            else
            {
                writeableBitmap = (WriteableBitmap)image.Source;
            }
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixelData = new byte[height * stride];
            writeableBitmap.CopyPixels(pixelData, stride, 0);

            // 픽셀 데이터 접근
            int index = (y * stride) + (x * 4);
            byte blue = pixelData[index];
            byte green = pixelData[index + 1];
            byte red = pixelData[index + 2];
            byte alpha = pixelData[index + 3];

            return new int[] { red, green, blue };
        }

        /// <summary>
        /// 이미지 컬려 변환
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="rgb"></param>
        public static void SetImageColor(Image image, int[] pivot, int[] rgb)
        {
            // 컬러값 차이 계산
            int nextRed = rgb[0] - pivot[0];
            int nextGreen= rgb[1] - pivot[1]; 
            int nextBlue = rgb[2] - pivot[2]; 

            WriteableBitmap writeableBitmap;
            object bitmap = image.Source;

            if (bitmap is BitmapImage)
            {
                // WriteableBitmap으로 변환
                writeableBitmap = new WriteableBitmap((BitmapImage)bitmap);
            }
            else
            {
                writeableBitmap = (WriteableBitmap)bitmap;
            }

            // 픽셀 데이터 접근
            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;
            int stride = width * ((writeableBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixelData = new byte[height * stride];
            writeableBitmap.CopyPixels(pixelData, stride, 0);

            // 색상 변환 로직
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte blue = pixelData[i];
                byte green = pixelData[i + 1];
                byte red = pixelData[i + 2];

                // 색상 더하기
                // Red
                if (nextRed > 0)
                {
                    pixelData[i + 2] = (byte)Math.Min(255, red + (byte)nextRed);
                }
                else
                {
                    pixelData[i + 2] = (byte)Math.Max(0, red + (byte)nextRed);
                }

                if (nextGreen > 0)
                {
                    pixelData[i + 1] = (byte)Math.Min(255, green + (byte)nextGreen);
                }
                else
                {
                    pixelData[i + 1] = (byte)Math.Max(0, green + (byte)nextGreen);
                }

                if (nextBlue > 0)
                {
                    pixelData[i] = (byte)Math.Min(255, blue + (byte)nextBlue);
                }
                else
                {
                    pixelData[i] = (byte)Math.Max(0, blue + (byte)nextBlue);
                }
            }

            // 수정된 픽셀 데이터를 WriteableBitmap에 적용
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

            // 변환된 이미지를 Image 컨트롤에 설정
            image.Source = writeableBitmap;
        }
        public static void SetImageColor(Image image, int[] rgb)
        {
            int[] pivot = GetImageColor(image);
            SetImageColor(image, pivot, rgb);
        }
        /// <summary>
        /// 그레이스케일 쉐이더 적용
        /// </summary>
        /// <param name="image"></param>
        public static void SetGrayScaleShader()
        {
            GrayscaleEffect grayscaleEffect = new GrayscaleEffect();
            Application.Current.MainWindow.Effect = grayscaleEffect;
        }
        /// <summary>
        /// 쉐이더 초기화
        /// </summary>
        public static void ClearShader()
        {
            Application.Current.MainWindow.Effect = null;
        }

        /// <summary>
        /// Z-Index 지정
        /// </summary>
        /// <param name="target"></param>
        /// <param name="zIndex"></param>
        public static void SetZindex(UIElement target, int zIndex)
        {
            Canvas.SetZIndex(target, zIndex);
        }
    }
}
