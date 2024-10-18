using RasingDeokPal.Common;
using System.Windows.Controls;
using System.Windows.Media;

namespace RasingDeokPal.View
{
    internal class UIView
    {
        protected Canvas canvas;
        protected Image imgBackground;

        public UIView(Canvas canvas)
        {
            
            this.imgBackground = new Image();
            this.canvas = canvas;

            imgBackground.Width = GameConfig.GetConfig().WindowWidth;
            imgBackground.Height = GameConfig.GetConfig().WindowHeight;
            this.canvas.Width = GameConfig.GetConfig().WindowWidth;
            this.canvas.Height =GameConfig.GetConfig().WindowHeight;
            imgBackground.IsHitTestVisible = false;
            //this.canvas.Background = Brushes.Transparent;
//#if DEBUG
            //this.canvas.Background = new SolidColorBrush(Colors.Chocolate);
//#endif
            this.canvas.Children.Add(imgBackground);

        }


        /// <summary>
        /// 캔버스 출력 여부 조절
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value)
        {
            Update();
            UIControl.SetVisibility(canvas, value);
        }

        /// <summary>
        /// 자기 요소 삭제
        /// </summary>
        public virtual void RemoveSelf()
        {

        }

        internal virtual void Update()
        {

        }
    }
}
