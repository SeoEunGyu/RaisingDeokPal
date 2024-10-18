using RasingDeokPal.Character.Unit;
using RasingDeokPal.Common;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static RasingDeokPal.Common.Animations;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace RasingDeokPal.effect.Unit
{
    internal class UnitNote : SubUnit
    {
        string uriQuarterNote = "pack://application:,,,/asset/effect/quarter_note.png";
        string uriEighthNote = "pack://application:,,,/asset/effect/eighth_note.png";
        string uriSixteenthNote = "pack://application:,,,/asset/effect/sixteenth_note.png";

        private RotateTransform? rotateTransform;
        private TranslateTransform? translateTransform;

        private DispatcherTimer? timer;
        private int lifeTime;       // 오브젝트 생존 시간

        private double firstLeft;
        private double firstTop;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="canvas"></param>
        public UnitNote(Canvas canvas, double left, double top) : base(canvas)
        {
            // 이미지 지정
            SetImage();
            SetUIElementSize(30,30);
            // 트랜스폼 그룹 추가
            SetTransformGroup();
            // 각도 랜덤
            SetAngle();
            // 초기 위치 설정
            firstLeft = left;
            firstTop = top;
            SetPosition(left, top);

            // 타이머 세팅
            SetTimer();
            SetMove();
        }
        
        /// <summary>
        /// 타이머 설정
        /// </summary>
        private void SetTimer()
        {
            lifeTime = WindowControlMethod.GetRandomInt(3, 5);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(lifeTime); // 타이머 간격
            timer.Tick += LifeHandler;
            timer.Start();
        }

        /// <summary>
        /// 타이머 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LifeHandler(object? sender, EventArgs e) 
        {
            if(timer != null)
            {
                timer.Stop();
                timer = null;

                RemoveSelf();
            }
        }

        /// <summary>
        /// 이미지 설정
        /// </summary>
        private void SetImage()
        {
            int imgType = WindowControlMethod.GetRandomInt(0, 3);
            if (imgType == 0)
            {
                SetImageSource(image, uriQuarterNote);
            }
            else if(imgType == 1)
            {
                SetImageSource(image, uriEighthNote);
            }
            else
            {
                SetImageSource(image, uriSixteenthNote);
            }
        }
        
        /// <summary>
        /// 트랜스폼 그룹 추가
        /// </summary>
        private void SetTransformGroup()
        {
            TransformGroup transformGroup = new TransformGroup();
            rotateTransform = new RotateTransform();
            rotateTransform.CenterX = 15; // 회전 중심 X 좌표
            rotateTransform.CenterY = 15; // 회전 중심 Y 좌표

            translateTransform = new TranslateTransform();
            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(translateTransform);

            // 트랜스폼 그룹 추가
            image.RenderTransform = transformGroup;
        }

        /// <summary>
        /// 각도 지정
        /// </summary>
        private void SetAngle()
        {
            if(rotateTransform != null)
            {
                int angle = WindowControlMethod.GetRandomInt(-45, 45);
                rotateTransform.Angle = angle; // 60도 회전
            }
        }

        /// <summary>
        /// 위치 지정
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        private void SetPosition(double left, double top)
        {
            Canvas.SetLeft(image, left);
            Canvas.SetTop(image, top);
        }
        private void SetMove()
        {
            double toTop = WindowControlMethod.GetRandomInt(-350, -150);
            SingleAnimation<TranslateTransform> Animation = new SingleAnimation<TranslateTransform>(translateTransform, TranslateTransform.YProperty, 0, toTop, lifeTime * 1000, false);
            Animation.Play();
        }
    }
}
