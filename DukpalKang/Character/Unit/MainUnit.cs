using static RasingDeokPal.Common.Animations;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;

namespace RasingDeokPal.Character.Unit
{
    /// <summary>
    /// 메인 유닛
    /// </summary>
    internal class MainUnit
    {
        BitmapImage img;
        protected double width;
        protected double height;
        protected Canvas targetCanvas;
        protected Image targetUI;

        // 이미지 트랜스폼
        public RotateTransform rotateTransform;
        public ScaleTransform scaleTransform;
        public TranslateTransform translateTransform;
        // 애니메이션 보드
        public AnimationBoard? animationBoard;
        // 랜덤 클래스
        protected Random random;
        // 타이머
        protected DispatcherTimer timer;
        private int zIndex = 11;    // 캐릭터 zIndex
        protected Point centerPoint;    

        /// <summary>
        /// 본체 생성자
        /// </summary>
        /// <param name="target"></param>
        /// <param name="imgUri"></param>
        public MainUnit(Canvas canvas, string imgUri)
        {
            // 이미지 클래스 설정
            Image image = new Image();
            
            // 랜덤 클래스 초기화
            random = new Random();
            targetCanvas = canvas;
            targetUI = image;

            SetTargetUIElementSize();
            // 이미지 소스 입력
            SetImageSource(image, imgUri);
            // zindex 지정
            Panel.SetZIndex(image, zIndex);

            // 렌더 트랜스폼 추가
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            // 트랜스 폼 그룹 추가
            SetTransformGroup();

            // 캔버스에 이미지 추가
            canvas.Children.Add(image);
        }

        /// <summary>
        /// 타겟 사이즈 조절
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetTargetUIElementSize(int width = 100, int height = 100)
        {
            this.targetUI.Width = width;
            this.targetUI.Height = height;  
        }

        /// <summary>
        /// 이미지 설정
        /// </summary>
        /// <param name="imgUri"></param>
        protected void SetImageSource(Image target, string imgUri)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imgUri, UriKind.Absolute);
            bitmap.EndInit();
            target.Source = bitmap;
            width = bitmap.Width;
            height = bitmap.Height;
        }

        /// <summary>
        /// 중심 좌표 이동
        /// </summary>
        /// <param name="point"></param>
        public void SetCenterXY(Point point)
        {
            rotateTransform.CenterX = point.X;
            rotateTransform.CenterY = point.Y;
        }
        public void SetCenterXY(double x, double y)
        {
            rotateTransform.CenterX = x;
            rotateTransform.CenterY = y;
        }

        /// <summary>
        /// z-index 설정
        /// </summary>
        /// <param name="zIndex"></param>
        public void SetZIndex(int zIndex)
        {
            this.zIndex = zIndex;
            Panel.SetZIndex(targetUI, zIndex);
        }

        /// <summary>
        /// 객체에 핸들러 추가
        /// </summary>
        /// <param name="routeEvent"></param>
        /// <param name="handler"></param>
        public void AddHandler(RoutedEvent routeEvent, Delegate handler)
        {
            targetUI.AddHandler(routeEvent, handler, true);
        }
        public void RaiseEvent(RoutedEventArgs e)
        {
            targetUI.RaiseEvent(e);
        }

        /// <summary>
        /// 트랜스폼 설정
        /// </summary>
        protected void SetTransformGroup()
        {
            
            TransformGroup transformGroup = new TransformGroup();
            rotateTransform = new RotateTransform();
            translateTransform = new TranslateTransform();
            scaleTransform = new ScaleTransform();

            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(scaleTransform);

            targetUI.RenderTransform = transformGroup;
            // 중심 좌표 지정
            this.centerPoint = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
        }

        /// <summary>
        /// 현재 이미지 각도 반환
        /// </summary>
        /// <returns></returns>
        protected double GetCurrentAngle()
        {
            return rotateTransform.Angle;
        }

        /// <summary>
        /// 이미지 가로 값 반환
        /// </summary>
        /// <returns></returns>
        public int GetImageWidth()
        {
            return (int)width;
        }
        /// <summary>
        /// 이미지 세로 값 반환
        /// </summary>
        /// <returns></returns>
        public int GetImageHeight()
        {
            return (int)height;
        }

        /// <summary>
        /// 캐릭터 애니메이션 보드 생성
        /// </summary>
        /// <param name="board"></param>
        public void CreateAnimationBoard(List<BoardAnimation> animations)
        {
            // 초기화 한번 하고 새로 생성
            AnimationClear();
            animationBoard = new AnimationBoard(animations);
        }
        /// <summary>
        /// 캐릭터 애니메이션 보드 재생
        /// </summary>
        public void AnimationPlay()
        {
            if (animationBoard != null)
            {
                animationBoard.Play();
            }
        }
        /// <summary>
        /// 캐릭터 애니메이션 보드 일시 정지
        /// </summary>
        public void AnimationPause()
        {
            if (animationBoard != null)
            {
                animationBoard.Pause();
            }
        }
        /// <summary>
        /// 캐릭터 애니메이션 보드 정지
        /// </summary>
        public void AnimationStop()
        {
            if (animationBoard != null)
            {
                animationBoard.Stop();
            }
        }

        /// <summary>
        /// 캐릭터 애니메이션 보드 다시 재생
        /// </summary>
        public void AnimationResume()
        {
            if (animationBoard != null)
            {
                animationBoard.Resume();
            }
        }
        /// <summary>
        /// 캐릭터 애니메이션 보드 초기화
        /// </summary>
        public void AnimationClear()
        {
            if (animationBoard != null)
            {
                animationBoard.Clear();
            }
        }

        /// <summary>
        /// 난수 생성
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        protected int GetRandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// 타이머 설정
        /// </summary>
        /// <param name="timeSecond"></param>
        /// <param name="handler"></param>
        public void SetTimer(int timeSecond, EventHandler handler)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(timeSecond); // 타이머 간격
            timer.Tick += handler;
            timer.Start();
        }

        /// <summary>
        /// 타이머 초기화
        /// </summary>
        public void ClearTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }
        /// <summary>
        /// 타이머 정지
        /// </summary>
        public void PauseTimer()
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }
        /// <summary>
        /// 타이머 재실행
        /// </summary>
        public void ResumeTimer()
        {
            if (timer != null)
            {
                timer.Start();
            }
        }
    }

}
