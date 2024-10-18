using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;

namespace RasingDeokPal.View
{
    internal class TitleView : UIView
    {
        Image imgTitle;
        Button btnStart;

        public TitleView(Canvas canvas) : base(canvas)
        {
            // 타이틀 이미지
            imgTitle = UIControl.CreateImage("pack://application:,,,/asset/title.png");
            UIControl.SetCanvasMargin(imgTitle, 65,100);
            this.canvas.Children.Add(imgTitle);

            // 분양 받기 버튼
            btnStart = new Button();
            btnStart.Content = "분양 받기";
            btnStart.Width = 200;
            btnStart.Height = 50;
            UIControl.SetCanvasMargin(btnStart, 100,300);
            this.canvas.Children.Add(btnStart);
            btnStart.Click += Start;
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start(object sender, RoutedEventArgs e)
        {
            UIControl.SetVisibility(imgTitle, false);
            UIControl.SetVisibility(btnStart, false);

            //SetVisible(false);
            RemoveSelf();

            // 플레이어 데이터 생성
            GameManager.Instance.LoadSaveData();
            // 픽업 화면 이동
            GameManager.Instance.GoPickUp();
        }

        private void RemoveSelf()
        {
            canvas.Children.Remove(imgTitle);
            canvas.Children.Remove(btnStart);
        }
    }
}
