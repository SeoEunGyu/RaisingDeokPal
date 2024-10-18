using System.Windows.Controls;
using static RasingDeokPal.Common.Animations;
using RasingDeokPal.Common;
using System.Windows.Media;
using System.Windows;


namespace RasingDeokPal.Character.Unit
{
    internal class SpriteUnit
    {
        protected SpriteAnimation animation;
        protected Image image;
        public Canvas canvas;
        protected ScaleTransform scaleTransform;

        public delegate void Operation();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="imgUri"></param>
        /// <param name="zIndex"></param>
        public SpriteUnit(Canvas canvas, string imgUri,int size, int zIndex = 11)
        {
            this.canvas = canvas;
            image = new Image();
            animation = new SpriteAnimation(image, imgUri, 150, 150, 9, 1);
            SetUIElementSize(size,size);
            animation.SetSize(size, size);
            SetImage(0);
            Panel.SetZIndex(image, zIndex);
            canvas.Children.Add(image);
        }

        public SpriteUnit(Canvas canvas, string imgUri, int frameWidth, int frameHeight, int zIndex = 11) 
        {
            this.canvas = canvas;
            image = new Image();
            animation = new SpriteAnimation(image, imgUri, frameWidth, frameHeight, 3,3);
            SetUIElementSize();
            SetImage(0);
            Panel.SetZIndex(image, zIndex);
            canvas.Children.Add(image);
        }

        public SpriteUnit(Canvas canvas, string imgUri, int frameWidth, int frameHeight, int frameColumn, int frameRow, int zIndex = 11)
        {
            this.canvas = canvas;
            image = new Image();
            animation = new SpriteAnimation(image, imgUri, frameWidth, frameHeight, frameColumn, frameRow);
            SetUIElementSize();
            SetImage(0);
            Panel.SetZIndex(image, zIndex);
            canvas.Children.Add(image);

            // 스케일 적용
            scaleTransform = new ScaleTransform();
            image.RenderTransform = scaleTransform;
            image.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        /// <summary>
        /// 이미지 위치 조절
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void SetUIPosition(int left, int top)
        {
            UIControl.SetCanvasMargin(image, left, top);
        }

        /// <summary>
        /// 이미지 반전 (오른쪽이 기본)
        /// </summary>
        /// <param name="value"></param>
        public void SetImageScaleRight(bool value)
        {
            scaleTransform.ScaleX = value ? 1 : -1;
        }

        /// <summary>
        /// 이미지 지정
        /// </summary>
        /// <param name="frame"></param>
        private void SetImage(int frame)
        {
            this.image.Source = animation.GetFrameSnapShot(frame);
        }

        /// <summary>
        /// UI 사이즈 조절
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetUIElementSize(int width = 100, int height = 100)
        {
            this.image.Width = width;
            this.image.Height = height;
        }

        /// <summary>
        /// 히트 박스 설정
        /// </summary>
        public void SetHitBox(bool value)
        {
            image.IsHitTestVisible = value;
        }


        /// <summary>
        /// 스프라이트 애니메이션 재생
        /// </summary>
        public void AnimationStart()
        {
            animation.Start();
        }

        /// <summary>
        /// 애니메이션 스피드 조절
        /// </summary>
        /// <param name="ms"></param>
        public void SetAnimationSpeed(int ms = 100)
        {
            animation.SetAnimationSpeed(ms);
        }

        public void SetAnimationLoop(bool value)
        {
            animation.SetAnimationLoop(value);
        }

        /// <summary>
        /// 객체 삭제
        /// </summary>
        public int RemoveSelf()
        {
            // 이미지 제거
            canvas.Children.Remove(image);
            return 0;
        }

        /// <summary>
        /// 애니메이션 정지 이벤트 핸들러
        /// </summary>
        /// <param name="callback"></param>
        public void SetAnimationStopHandler(Func<int> callback)
        {
            animation.SetStopHandler(callback);
        }
    }
}
