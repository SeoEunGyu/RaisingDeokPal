using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace RasingDeokPal.Components.Menu
{
    internal class PotMenu
    {
        AnimatedMenuPanel panel;
        Image imageStarDustPanel;
        TextBlock textBlockStarDust;

        public PotMenu(Canvas canvas, List<AnimatedMenuButton> buttons)
        {
            int pivotWidth = GameConfig.GetConfig().WindowWidth / 2;
            int pivotHeight = GameConfig.GetConfig().WindowHeight / 2;
            Point pivot = new Point(pivotWidth, pivotHeight);

            // 경로 Array
            BezierSegment[] pathArray = new BezierSegment[]
            {
                CreatePath(pivot, new Point(-180, 10), new Point(-190, -40), new Point(-170, -85)),
                CreatePath(pivot, new Point(-170, 120), new Point(-180, 30), new Point(-185, 0)),
                CreatePath(pivot, new Point(-90, 160), new Point(-120, 120), new Point(-160, 75))
            };

            panel = new (
               canvas,
               buttons,
               CreateCenterPoint(pivot, new Point(-160, 20)),
               pathArray,
               CreatePath(pivot, new Point(-170, -30), new Point(-120, -90), new Point(-80, -140)),
               CreatePath(pivot, new Point(60, 200), new Point(30, 180), new Point(-5, 160)),
               false
           );

            // 별가루 패널
            imageStarDustPanel = UIControl.CreateImage("pack://application:,,,/asset/ui/ui_stardust.png");
            Point stardustPanelPoint = CreateCenterPoint(pivot, new Point(-imageStarDustPanel.Width/2, 200));
            UIControl.SetCanvasMargin(imageStarDustPanel, (int)stardustPanelPoint.X, (int)stardustPanelPoint.Y);
            canvas.Children.Add(imageStarDustPanel);
            UIControl.SetVisibility(imageStarDustPanel, false);

            textBlockStarDust = new TextBlock();
            textBlockStarDust.Width = imageStarDustPanel.Width;
            textBlockStarDust.Height = imageStarDustPanel.Height;
            UIControl.SetCanvasMargin(textBlockStarDust, (int)stardustPanelPoint.X, (int)stardustPanelPoint.Y);
            textBlockStarDust.TextAlignment = TextAlignment.Right;
            textBlockStarDust.Padding = new Thickness(15,10,15,10);
            textBlockStarDust.FontSize = 16;
            canvas.Children.Add(textBlockStarDust);
            UIControl.SetVisibility(textBlockStarDust, false);
        }

        private BezierSegment CreatePath(Point pivot, Point first, Point second, Point third)
        {
            // 첫번째 지점
            Point firstPoint = CreateCenterPoint(pivot, first);
            Point secondPoint = CreateCenterPoint(pivot, second);
            Point thirdPoint = CreateCenterPoint(pivot, third);

            return AnimatedMenuPanel.CreatePath(firstPoint, secondPoint, thirdPoint);
        }
        private Point CreateCenterPoint(Point pivot, Point point)
        {
            return new Point(pivot.X + point.X, pivot.Y + point.Y);
        }

        public void Hide()
        {
            panel.Hide();
            UIControl.SetVisibility(imageStarDustPanel, false);
            UIControl.SetVisibility(textBlockStarDust, false);
        }
        public void Show()
        {
            panel.Show();

            //별가루 출력
            UIControl.SetVisibility(imageStarDustPanel, true);
            SetStarDustText();
            UIControl.SetVisibility(textBlockStarDust, true);
        }
        public void Toggle()
        {
            panel.Toggle();
            // 별가루 영역
            bool isShow = imageStarDustPanel.Visibility != Visibility.Visible;
            if(isShow)
            {
                SetStarDustText();
            }
            UIControl.SetVisibility(imageStarDustPanel, isShow);
            UIControl.SetVisibility(textBlockStarDust, isShow);
        }

        internal void SetStarDustText()
        {
            int stardustValue = GameManager.Instance.playerData.GetStardust();
            textBlockStarDust.Text = string.Format("{0:N0}", stardustValue);

        }
    }
}
