using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RasingDeokPal.Components.Menu
{
    internal class DeokPalMenuPanel
    {
        AnimatedMenuPanel panel;

        public DeokPalMenuPanel(Canvas canvas, List<AnimatedMenuButton> buttons)
        {
            int pivotWidth = GameConfig.GetConfig().WindowWidth / 2;
            int pivotHeight = GameConfig.GetConfig().WindowHeight / 2;
            Point pivot = new Point(pivotWidth, pivotHeight);

            // 경로 Array
            BezierSegment[] pathArray = new BezierSegment[]
            {
                CreatePath(pivot,new Point(20, -180), new Point(40,  -160), new Point(80, -130)),
                CreatePath(pivot,new Point(70, -160), new Point(100, -120), new Point(140, -75)),
                CreatePath(pivot,new Point(150, -120), new Point(160, -30), new Point(160, 5)),
                CreatePath(pivot,new Point(160, -10), new Point(170, 40), new Point(140, 85))
            };
            panel = new AnimatedMenuPanel(
                canvas,
                buttons,
                CreateCenterPoint(pivot, new Point(-10, -190)),
                pathArray,
                CreatePath(pivot, new Point(-60, -200), new Point(-30, -180), new Point(5, -160)),
                CreatePath(pivot, new Point(170, 30), new Point(120, 90), new Point(80, 145)),
                true,
                4
            );
        }

        private BezierSegment CreatePath(Point pivot, Point first, Point second, Point third)
        {
            // 첫번째 지점
            Point firstPoint    = CreateCenterPoint(pivot, first);
            Point secondPoint   = CreateCenterPoint(pivot, second);
            Point thirdPoint    = CreateCenterPoint(pivot, third);

            return AnimatedMenuPanel.CreatePath(firstPoint, secondPoint, thirdPoint);
        }
        private Point CreateCenterPoint(Point pivot, Point point)
        {
            return new Point(pivot.X + point.X, pivot.Y + point.Y);
        }

        public void Hide()
        {
            panel.Hide();
        }
        public void Show()
        {
            panel.Show();
        }
        public void Toggle()
        {
            panel.Toggle();
        }
    }
}
