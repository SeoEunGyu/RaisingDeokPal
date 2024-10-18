using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RasingDeokPal.Components.Chat
{
    internal class ChatInput
    {
        Image img;
        TextBox textBox;
        Canvas canvas;

        int bgWidth = 300;
        int bgHeight = 50;
        int margin = 10;
        int InputMarginBottom = 100;

        public ChatInput(Canvas canvas)
        {
            this.canvas = canvas;

            // 이미지
            this.img = new Image();
            BitmapImage bitmap = UIControl.CreateBitmap("pack://application:,,,/asset/ui/ui_alert.png");
            img.Source = bitmap;
            img.Stretch = Stretch.Fill;
            img.Width = bgWidth;
            img.Height = bgHeight;

            // 텍스트 박스
            textBox = new TextBox();
            textBox.Width = bgWidth - (margin *2);
            textBox.Height = bgHeight - (margin *2);
            textBox.Margin = new Thickness(margin);
            textBox.KeyDown += TextBox_KeyDown; ;


            // 위치 조정
            int width = (int)(GameConfig.Instance.WindowWidth / 2 - img.Width / 2);
            int height = (int)(GameConfig.Instance.WindowHeight - img.Height - InputMarginBottom);

            UIControl.SetCanvasMargin(img, width, height);
            UIControl.SetCanvasMargin(textBox, width, height);

            canvas.Children.Add(img);
            canvas.Children.Add(textBox);
        }

        /// <summary>
        /// Key Down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string inputText = textBox.Text;
                if (string.IsNullOrEmpty(inputText))
                {
                    return;
                }
                // 텍스트 박스 비우기
                textBox.Clear();

                // GPT 문의 실행
                GameManager.Instance.GetGameView().AnswerChatUI(" . . . ");
                await GameManager.Instance.GetGameView().ChatWithGPT(inputText);
                
            }
        }

        public void Hide()
        {
            img.Visibility = Visibility.Hidden;
            textBox.Visibility = Visibility.Hidden;
            SetText();   
        }

        public void Show()
        {
            img.Visibility = Visibility.Visible;
            textBox.Visibility = Visibility.Visible;
        }

        public void SetText(string text = "")
        {
            textBox.Text = text;
        }
    }
}
