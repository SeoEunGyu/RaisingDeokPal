using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace RasingDeokPal.Common
{
    /// <summary>
    /// 애니메이션 클래스
    /// </summary>
    internal class Animations
    {
        /// <summary>
        /// 애니메이션 스토리보드 클래스
        /// </summary>
        public class AnimationBoard
        {
            Storyboard board;
            public IEnumerable<BoardAnimation> animations { get; set;}
            Stopwatch stopWatch;
            public double playTime { get; set; } = 0;
            public bool isRunning { get; set; }
            public bool isCompleted { get; set; }

            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="animations"></param>
            public AnimationBoard(IEnumerable<BoardAnimation> animations) 
            {
                board = new Storyboard();
                this.animations = animations;
                stopWatch = new Stopwatch();

                // 애니메이션 초기 설정
                if (this.animations.Count() > 0)
                {
                    foreach (var animation in this.animations)
                    {
                        Storyboard.SetTarget(animation.animation, animation.target);
                        Storyboard.SetTargetProperty(animation.animation, animation.targetProperty);
                        board.Children.Add(animation.animation);
                    }
                }
            }

            /// <summary>
            /// 스토리보드 애니메이션 시작
            /// </summary>
            public void Play()
            {
                if(board != null)
                {
                    Window window = Application.Current.MainWindow;
                    board.Begin(window, true);
                    isRunning = true;
                    isCompleted = false;

                    // 애니메이션 재생시간 체크
                    if(stopWatch == null)
                    {
                        stopWatch = new Stopwatch();
                        stopWatch.Start();
                        playTime = 0;
                    }
                    else
                    {
                        stopWatch.Stop();
                        playTime = 0;
                        stopWatch.Start();
                    }
                }

            }

            /// <summary>
            /// 스토리보드 애니메이션 일시 정지
            /// </summary>
            public void Pause()
            {
                if (board != null)
                {
                    Window window = Application.Current.MainWindow;
                    board.Pause(window);
                    isRunning = false;

                    // 애니메이션 재생시간 체크
                    if (stopWatch != null)
                    {
                        stopWatch.Stop();
                        playTime += stopWatch.Elapsed.TotalMilliseconds;
                    }
                }
            }

            /// <summary>
            /// 스토리보드 애니메이션 시작
            /// </summary>
            public void Stop()
            {
                if (board != null)
                {
                    board.Stop();
                    isRunning = false;

                    // 애니메이션 재생시간 체크
                    if (stopWatch != null)
                    {
                        stopWatch.Stop();
                        playTime = stopWatch.Elapsed.TotalMilliseconds;
                    }
                }
            }

            /// <summary>
            /// 스토리보드 애니메이션 다시 재생
            /// </summary>
            public void Resume()
            {
                if (board != null)
                {
                    Window window = Application.Current.MainWindow;
                    board.Resume(window);
                    isRunning = true;

                    // 애니메이션 재생시간 체크
                    if (stopWatch == null)
                    {
                        stopWatch = new Stopwatch();
                        stopWatch.Start();
                    }
                    else
                    {
                        stopWatch.Stop();
                        stopWatch.Start();
                    }
                }
            }

            /// <summary>
            /// 스토리보드 초기화
            /// </summary>
            public void Clear()
            {
                if(board != null)
                {
                    board.Stop();
                    board.Remove();
                    isRunning = false;
                }
            }

            public void SetIsComplete(bool value)
            {
                this.isCompleted = value;
            }

            /// <summary>
            /// 애니메이션 추가
            /// </summary>
            /// <param name="animation"></param>
            public void AddAnimation(BoardAnimation animation)
            {
                animations.Append(animation);
                Storyboard.SetTarget(animation.animation, animation.target);
                Storyboard.SetTargetProperty(animation.animation, animation.targetProperty);
                board.Children.Add(animation.animation);
            }

            /// <summary>
            /// 애니메이션 객체 반환
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public BoardAnimation GetBoardAnimation(int index)
            {
                return this.animations.ElementAt(index);
            }

        }
        /// <summary>
        /// 스토리보드 하위 애니메이션
        /// </summary>
        public class BoardAnimation
        {
            double? preFrom = 0;
            double? preTo = 0;
            double differance;
            protected double firstDuration;

            public DependencyObject? target;        // 타겟
            public PropertyPath? targetProperty;    // 속성
            public DoubleAnimation? animation;

            bool allowUpdate;   // 변수 업데이트 여부

            /// <summary>
            /// 생성자
            /// </summary>
            public BoardAnimation(DoubleAnimation animation, DependencyObject target, DependencyProperty property)
            {
                this.animation = animation;
                this.target = target;
                this.targetProperty = new PropertyPath(property);
                this.preFrom = this.animation.From;
                this.preTo = this.animation.To;
                this.firstDuration = animation.Duration.TimeSpan.TotalMilliseconds;
                if (this.preFrom.HasValue && this.preTo.HasValue)
                {
                    this.differance = this.preTo.Value - this.preFrom.Value;
                }
                
                
            }
            public BoardAnimation(DoubleAnimation animation, DependencyObject target, string property)
            {
                this.animation = animation;
                this.target = target;
                this.targetProperty = new PropertyPath(property);
                this.preFrom = this.animation.From;
                this.preTo = this.animation.To;
                this.firstDuration = animation.Duration.TimeSpan.TotalMilliseconds;
                if (this.preFrom.HasValue && this.preTo.HasValue)
                {
                    this.differance = this.preTo.Value - this.preFrom.Value;
                }
            }

            /// <summary>
            /// From, To 갱신
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            public void UpdateFromTo(double from, double playTime = 0)
            {
                // 재생 시간
                double timeDuration = 1.0-(playTime / firstDuration);
                double nextDuration = firstDuration * timeDuration;
                double nextTo = from + (differance * timeDuration);

                // 지속시간 음수 뜨면 리턴
                if (timeDuration < 0)
                {
                    return;
                }

                // 새로운 목표치 계산                
                if (animation != null)
                {
                    animation.From = from;
                    animation.To = nextTo;
                    animation.Duration = new Duration(TimeSpan.FromMilliseconds(nextDuration));
                }
            }
            /// <summary>
            /// From 갱신
            /// </summary>
            /// <param name="from"></param>
            public void UpdateFrom(double from, double playTime = 0)
            {
                // 재생 시간
                double timeDuration = 1.0 - (playTime / firstDuration);
                double nextDuration = firstDuration * timeDuration;

                // 지속시간 음수 뜨면 리턴
                if(timeDuration < 0)
                {
                    return;
                }

                // 새로운 목표치 계산
                if (animation != null)
                {
                    animation.From = from;
                    animation.Duration = new Duration(TimeSpan.FromMilliseconds(nextDuration));
                }
            }
        }

        /// <summary>
        /// 단일 애니메이션 클래스
        /// </summary>
        /// <remarks>
        /// 대부분의 애니메이션은 애니메이션 보드에 묶어서 객체에 적용한다.
        /// 단일 적용도 가능
        /// </remarks>
        public class SingleAnimation<T> where T : System.Windows.Media.Transform
        {
            double? preFrom;
            double? preTo;
            double differance;

            private T?           targetTransform;    // 트랜스폼
            DependencyProperty?  targetProperty;     // 속성
            DoubleAnimation?     animation;

            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="transform"></param>
            /// <param name="property"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <param name="duration"></param>
            public SingleAnimation(T transform, DependencyProperty property, double from, double to, int duration)
            {
                this.targetTransform = transform;
                this.targetProperty = property;
                this.animation = CreateDoubleAnimation(from, to, duration, null);
            }
            public SingleAnimation(T transform, DependencyProperty property, double from, double to, int duration, bool autoReverse)
            {
                this.targetTransform = transform;
                this.targetProperty = property;
                this.animation = CreateDoubleAnimation(from, to, duration, null, autoReverse);
            }


            /// <summary>
            /// [Pause] 현재 애니메이션의 진행 좌표 계산
            /// </summary>
            /// <param name="preFrom"></param>
            /// <param name="preTo"></param>
            public void SetDifferance(double preFrom, double preTo)
            {
                // 애니메이션 정지한 시점에서
                // from to 저장 및 남은 차이 구하기
                this.preFrom = preFrom;
                this.preTo = preTo;
                differance = this.preTo.Value - this.preFrom.Value;
            }

            /// <summary>
            /// [Resume] 애니메이션의 FromTo 업데이트
            /// </summary>
            /// <param name="from"></param>
            public void UpdateFromTo(double from) 
            {
                // 애니메이션이 존재하는 경우
                // From To 재계산하여 지정
                if(animation != null) 
                {
                    animation.From = from;
                    animation.To = from + differance;
                }
            }

            /// <summary>
            /// 애니메이션 단일 재생
            /// </summary>
            public void Play()
            {
                // 재생에 필요한 요소가 모두 존재하는 경우, 애니메이션 재생
                if(targetTransform != null && targetProperty != null && animation != null) 
                {
                    targetTransform.BeginAnimation(targetProperty, animation);
                }
            }
        }

        /// <summary>
        /// DoubleAnimation 생성
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static DoubleAnimation CreateDoubleAnimation(int from, int to, int duration)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
            };
            return animation;
        }
        public static DoubleAnimation CreateDoubleAnimation(double from, double to, int duration, EventHandler? completedHandler)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
            };
            // 종료 이벤트 핸들러 있는 경우 추가
            if(completedHandler != null)
            {
                animation.Completed += completedHandler;
            }
            return animation;
        }
        public static DoubleAnimation CreateDoubleAnimation(double from, double to, int duration, EventHandler? completedHandler, bool autoReverse = true)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                AutoReverse = autoReverse
            };
            // 종료 이벤트 핸들러 있는 경우 추가
            if (completedHandler != null)
            {
                animation.Completed += completedHandler;
            }
            return animation;
        }

        /// <summary>
        /// 스프라이트 애니메이션 클래스
        /// </summary>
        public class SpriteAnimation
        {
            private DispatcherTimer animationTimer;
            private int width;
            private int height;
            private BitmapImage spriteSheet;
            
            private List<CroppedBitmap> frameBitmaps;
            private int totalFrame = 0;
            private int currentFrame = 0;

            Image target;
            private int frameWidth;
            private int frameHeight;
            private int rowFrames;
            private int columnFrames;

            private bool loop = false;

            public delegate void Operation(Delegate callback);
            // 애니메이션 정지 콜백 함수
            public Func<int> stopCallBack;

            public SpriteAnimation(Image target, string uri, int frameWidth, int frameHeight, int columnFrames, int rowFrames)
            {
                spriteSheet = new BitmapImage(new Uri(uri, UriKind.Absolute));

                this.frameWidth = frameWidth;
                this.frameHeight = frameHeight;
                this.rowFrames = rowFrames;
                this.columnFrames = columnFrames; 
                this.target = target;

                // 프레임 이미지 초기화
                this.frameBitmaps = new List<CroppedBitmap>();
                for (int column= 0; column < this.columnFrames; column++)
                {
                    for(int row= 0;row < this.rowFrames; row++)
                    {
                        // 0,0
                        Int32Rect rect = new Int32Rect(column * frameWidth, row * frameHeight, frameWidth, frameHeight);
                        CroppedBitmap croppedBitmap = new CroppedBitmap(spriteSheet, rect);
                        this.frameBitmaps.Add(croppedBitmap);
                    }
                }
                totalFrame = this.frameBitmaps.Count;
                SetSize(frameWidth, frameHeight);


                // 타이머 초기화
                animationTimer = new DispatcherTimer();
                animationTimer.Interval = TimeSpan.FromMilliseconds(100); // 프레임 간격 (예: 100ms)
                animationTimer.Tick += AnimationTimer_Tick;
            }

            /// <summary>
            /// 애니메이션 종료 핸들러 지정
            /// </summary>
            /// <param name="handler"></param>
            public void SetStopHandler(Func<int> callback)
            {
                stopCallBack = ()=>
                {
                    callback();
                    return 0;
                };
            }


            /// <summary>
            /// 애니메이션 속도 조절
            /// </summary>
            /// <param name="ms"></param>
            public void SetAnimationSpeed(int ms = 100)
            {
                animationTimer.Interval = TimeSpan.FromMilliseconds(ms);
            }

            public void SetSize(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            /// <summary>
            /// 애니메이션 틱
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void AnimationTimer_Tick(object? sender, EventArgs e)
            {
                if (currentFrame >= totalFrame)
                {
                    if (loop)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        Stop();
                        return;
                    }
                }
                target.Source = this.frameBitmaps[currentFrame];
                currentFrame++;
            }

            /// <summary>
            /// 특정 프레임의 리소스 반환
            /// </summary>
            /// <returns></returns>
            public CroppedBitmap GetFrameSnapShot(int frame)
            {
                int column = frame % totalFrame;
                int row = frame / totalFrame;
                Int32Rect rect = new Int32Rect(column * frameWidth, row * frameHeight, frameWidth, frameHeight);
                return new CroppedBitmap(spriteSheet, rect);
            }

            public void SetAnimationLoop(bool value)
            {
                loop = value;
            }

            /// <summary>
            /// 스프라이트 애니메이션 스타트
            /// </summary>
            public void Start()
            {
                animationTimer.Start();
            }

            /// <summary>
            /// 스프라이트 애니메이션 정지
            /// </summary>
            public void Stop()
            {
                animationTimer.Stop();

                // 애니메이션 정지 callback 함수가 있는 경우 발동
                if(stopCallBack != null)
                {
                    this.stopCallBack();
                }
            }
        }
    }
}
