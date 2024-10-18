using RasingDeokPal.Character.Unit;
using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static RasingDeokPal.Common.Animations;

namespace RasingDeokPal.Character
{
    internal class NoPowerSun : SubUnit
    {
        ScaleTransform scaleTransform;
        private const int createDuration = 150;
        private int margin = -200;

        /// <summary>
        /// 힘이없썬
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="uri"></param>
        /// <param name="zIndex"></param>
        public NoPowerSun(Canvas canvas, string uri, int zIndex) :base(canvas, uri, zIndex)
        {
            SetHitBox(false);
            SetUIElementSize(130, 130);

            Point pivot = new Point(
                (GameConfig.GetConfig().WindowWidth/2)  + margin,
                (GameConfig.GetConfig().WindowHeight/2) + margin
            );
            
            SetUIMargin((int)pivot.X, (int)pivot.Y);

            SetTransform();
            CreateAnimationPlay();
        }

        private void SetTransform()
        {
            // Sub 유닛의 경우 렌더 트랜스폼은 적용되어있음
            this.scaleTransform = new ScaleTransform();
            image.RenderTransform = scaleTransform;
        }

        public void CreateAnimationPlay()
        {
            // 생성 애니메이션
            SingleAnimation<ScaleTransform> ScaleXAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleXProperty, 0, 1.0, createDuration);
            SingleAnimation<ScaleTransform> ScaleYAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleYProperty, 0, 1.0, createDuration);
            ScaleXAnimation.Play();
            ScaleYAnimation.Play();
        }

        public void RemoveAnimationPlay()
        {
            // 생성 애니메이션
            SingleAnimation<ScaleTransform> ScaleXAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleXProperty, 1.0, 0, createDuration);
            SingleAnimation<ScaleTransform> ScaleYAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleYProperty, 1.0, 0, createDuration);
            ScaleXAnimation.Play();
            ScaleYAnimation.Play();
        }

        /// <summary>
        /// 유닛 제거
        /// </summary>
        public async override void RemoveSelf()
        {
            RemoveAnimationPlay();
            // 이미지 제거
            await Task.Delay(createDuration);
            canvas.Children.Remove(image);
        }
    }
}
