using RasingDeokPal.Common;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;




namespace RasingDeokPal.Character.Unit
{
    internal class SubUnit
    {
        protected Image image;
        protected Canvas canvas;
        public int bitmapWidth;
        public int bitmapHeight;
        protected BitmapImage bitmapOrigin;
        private int zIndex = 10;    // 그림자 zindex 레벨

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="imgUri"></param>
        public SubUnit(Canvas canvas, string imgUri)
        {
            this.canvas = canvas;
            this.image = new Image();
            // 비트맵 설정
            SetImageSource(image, imgUri);
            SetUIElementSize(bitmapWidth, bitmapHeight);
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            Panel.SetZIndex(image, zIndex);
            canvas.Children.Add(image);
        }
        public SubUnit(Canvas canvas, string? imgUri, int zIndex = 10)
        {
            this.canvas = canvas;
            this.image = new Image();
            // 비트맵 설정
            if(imgUri != null)
            {
                SetImageSource(image, imgUri);
            }
            SetUIElementSize(bitmapWidth, bitmapHeight);
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            this.zIndex = zIndex;
            Panel.SetZIndex(image, zIndex);
            canvas.Children.Add(image);
        }
        public SubUnit(Canvas canvas)
        {
            this.canvas = canvas;
            this.image = new Image();
            canvas.Children.Add(image);
        }


        /// <summary>
        /// 비트맵 설정
        /// </summary>
        /// <param name="imgUri"></param>
        protected void SetImageSource(Image target, string imgUri)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imgUri, UriKind.Absolute);
            bitmap.EndInit();
            target.Source = bitmap;
            bitmapOrigin = bitmap;
            bitmapWidth = (int)bitmap.Width;
            bitmapHeight = (int)bitmap.Height;
        }

        /// <summary>
        /// 이미지 반환
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            return this.image;
        }

        /// <summary>
        /// 이미지 컬러 더하기
        /// </summary>
        /// <param name="alpah"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        protected void CalmageColor(int deltaR, int deltaG, int deltaB, bool isAdd = true)
        {
            WriteableBitmap writeableBitmap;
            object bitmap = image.Source;
            
            if(bitmap is BitmapImage)
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

                if (isAdd)
                {
                    // 색상 더하기
                    pixelData[i] = (byte)Math.Min(255, blue + (byte)deltaB);
                    pixelData[i + 1] = (byte)Math.Min(255, green + (byte)deltaG);
                    pixelData[i + 2] = (byte)Math.Min(255, red + (byte)deltaR);
                }
                else
                {
                    // 색상 빼기
                    pixelData[i] = (byte)Math.Max(0, blue - (byte)deltaB);
                    pixelData[i + 1] = (byte)Math.Max(0, green - (byte)deltaG);
                    pixelData[i + 2] = (byte)Math.Max(0, red - (byte)deltaR);
                }
            }

            // 수정된 픽셀 데이터를 WriteableBitmap에 적용
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

            // 변환된 이미지를 Image 컨트롤에 설정
            image.Source = writeableBitmap;
        }
        


        /// <summary>
        /// 이미지 컬러 반환
        /// </summary>
        protected int[] GetImageColor()
        {
            WriteableBitmap writeableBitmap;
            int x = bitmapWidth / 2;
            int y = bitmapHeight / 2;

            if (this.image.Source is BitmapSource bitmapSource)
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

            return new int[] {red, green, blue};
        }
        /// <summary>
        /// 컬러 복원
        /// </summary>
        protected void ResetImageColor()
        {
            image.Source = bitmapOrigin;
        }

        /// <summary>
        /// 타겟 사이즈 조절
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetUIElementSize(int width = 100, int height = 100)
        {
            this.image.Width = width;
            this.image.Height = height;
        }

        /// <summary>
        /// 이미지 UI 마진
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void SetUIMargin(int left, int top)
        {
            UIControl.SetCanvasMargin(this.image, left, top);
        }

        /// <summary>
        /// z-index 설정
        /// </summary>
        /// <param name="zIndex"></param>
        public void SetZIndex(int zIndex)
        {
            this.zIndex = zIndex;
            Panel.SetZIndex(image, zIndex);
        }

        /// <summary>
        /// 히트박스 설정
        /// </summary>
        /// <param name="value"></param>
        public void SetHitBox(bool value)
        {
            image.IsHitTestVisible = value;
        }

        /// <summary>
        /// 이미지 객체에 핸들러 추가
        /// </summary>
        /// <param name="routeEvent"></param>
        /// <param name="handler"></param>
        public void AddHandler(RoutedEvent routeEvent, Delegate handler)
        {
            image.AddHandler(routeEvent, handler, true);
        }
        public void RaiseEvent(RoutedEventArgs e)
        {
            image.RaiseEvent(e);
        }

        /// <summary>
        /// 유닛 제거
        /// </summary>
        public virtual void RemoveSelf()
        {
            // 이미지 제거
            canvas.Children.Remove(image);
        }

        /// <summary>
        /// UI 출력 여부 설정
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value)
        {
            UIControl.SetVisibility(image, value);
        }
    }
}
