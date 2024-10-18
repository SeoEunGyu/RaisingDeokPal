using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace RasingDeokPal.Components.Chat
{
    internal class ChatBubble
    {
        Image imgBubble;
        Image imgDownArrow;
        TextBlock textBlock;
        Canvas canvas;

        int bubbleMinWidth = 100;
        int bubbleMaxWidth = 400;

        int marginLeft = 40;
        int marginTop = 50;

        int arrowMarginTop = 5;
        int top = -100;

        int fontSize = 14;
        int lineHeight = 16;

        public ChatBubble(Canvas canvas, string text)
        {
            this.canvas = canvas;

            // 이미지
            imgBubble = new Image();
            imgDownArrow = new Image();
            BitmapImage bitmap = UIControl.CreateBitmap("pack://application:,,,/asset/ui/ui_alert.png");
            BitmapImage bitmapDownArrow = UIControl.CreateBitmap("pack://application:,,,/asset/ui/ui_down_wood.png");
            imgBubble.Source = bitmap;
            imgBubble.Stretch = Stretch.Fill;

            // 아래 화살표
            imgDownArrow.Width = 15;
            imgDownArrow.Height = 15;
            imgDownArrow.Source = bitmapDownArrow;
            imgDownArrow.Stretch = Stretch.Fill;

            // 텍스트
            textBlock = new TextBlock();
            textBlock.Text = text;
            //textBlock.Margin = new Thickness(10);
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.SizeChanged += DynamicBubbleImageSize;
            //textBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/asset/font/"), "#던파 비트비트체 v2");
            textBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/asset/font/"), "#ONE 모바일POP");
            textBlock.FontSize = fontSize;     // 폰트 사이즈
            textBlock.LineHeight = lineHeight;   // 라인 크기

            canvas.Children.Add(imgBubble);
            canvas.Children.Add(textBlock);
            canvas.Children.Add(imgDownArrow);
        }

        /// <summary>
        /// 텍스트 박스 사이즈 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DynamicBubbleImageSize(object sender, SizeChangedEventArgs e)
        {
            // 텍스트 블록의 너비와 높이에 따라 이미지 크기 조정
            double bubbleWidth = textBlock.ActualWidth;
            if (bubbleWidth < bubbleMinWidth)
            {
                bubbleWidth = bubbleMinWidth;
            }
            if(bubbleWidth > bubbleMaxWidth)
            {
                //bubbleWidth = bubbleMaxWidth;
            }

            imgBubble.Width = bubbleWidth + marginLeft;
            imgBubble.Height = textBlock.ActualHeight + marginTop;

            int width = (int)(GameConfig.Instance.WindowWidth / 2 - imgBubble.Width / 2);
            int height = (int)(GameConfig.Instance.WindowHeight / 2 - imgBubble.Height / 2);

            int arrowWidth = width + (int)(imgBubble.Width / 2) - (int)(imgDownArrow.Width / 2);
            int arrowHeight = height + (int)imgBubble.Height;


            UIControl.SetCanvasMargin(imgBubble, width, height + top);
            UIControl.SetCanvasMargin(textBlock, width + marginLeft / 2, height + marginTop / 2 + top);
            UIControl.SetCanvasMargin(imgDownArrow, arrowWidth, arrowHeight + top + arrowMarginTop);

        }

        public void Hide()
        {
            imgBubble.Visibility = Visibility.Hidden;
            imgDownArrow.Visibility = Visibility.Hidden;
            textBlock.Visibility = Visibility.Hidden;
        }

        public void Show()
        {
            imgBubble.Visibility = Visibility.Visible;
            imgDownArrow.Visibility = Visibility.Visible;
            textBlock.Visibility = Visibility.Visible;
        }
        public void SetText(string text)
        {
            textBlock.Text = text;
        }
    }
}
