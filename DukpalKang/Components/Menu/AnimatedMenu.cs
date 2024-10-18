using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace RasingDeokPal.Components.Menu
{
    internal class AnimatedMenuPanel
    {
        Canvas canvas;
        List<AnimatedMenuButton> buttons;
        protected bool isShow = false;

        // 버튼 경로
        BezierSegment[] paths;
        BezierSegment firstPath = new BezierSegment();
        BezierSegment secondPath = new BezierSegment();
        BezierSegment thirdPath = new BezierSegment();

        BezierSegment leftPath = new BezierSegment();
        BezierSegment rightPath = new BezierSegment();

        private AnimatedMenuButton leftBtn;
        private AnimatedMenuButton rightBtn;

        public int page = 1;            // 현재 페이지
        private int totalItemCount;     // 전체 아이템 수
        private int pageItem = 3; // 페이지 출력 아이템
        private int totalPage;          // 전체 페이지

        public AnimatedMenuPanel(Canvas canvas, List<AnimatedMenuButton> buttons, Point startPoint, BezierSegment[] paths, BezierSegment leftPath, BezierSegment rightPath, bool useMouseWheel, int pageItem = 3, int zIndex = 14)
        {
            this.pageItem = pageItem;
            this.buttons = buttons;
            this.canvas = canvas;

            // 버튼 추가
            foreach (var button in buttons)
            {
                button.SetFromPoint(startPoint);
                System.Windows.Controls.Panel.SetZIndex(button.image, zIndex);
                if (useMouseWheel)
                {
                    button.image.MouseWheel += MenuMouseWheel;
                }
                canvas.Children.Add(button.image);

            }

            // Left 버튼
            leftBtn = new AnimatedMenuButton("<", this.canvas, "pack://application:,,,/asset/button/btn_arrow_left1.png", new RoutedEventHandler(SelectMenuLeft), null);
            // Right 버튼
            rightBtn = new AnimatedMenuButton(">", this.canvas, "pack://application:,,,/asset/button/btn_arrow_right1.png", new RoutedEventHandler(SelectMenuRight), null);
            if (useMouseWheel)
            {
                leftBtn.image.MouseWheel += MenuMouseWheel;
                rightBtn.image.MouseWheel += MenuMouseWheel;
            }
            canvas.Children.Add(leftBtn.image);
            canvas.Children.Add(rightBtn.image);

            // 경로 지정
            SetPathPoints(paths, leftPath, rightPath);

            // 페이징 상수 계산
            totalItemCount = buttons.Count; // 전체 아이템 개수
            totalPage = CalTotalPage(totalItemCount, pageItem);

            // 페이징 작업
            SetPaging();
        }

        /// <summary>
        /// 패스 지정
        /// </summary>
        /// <param name="points"></param>
        public void SetPathPoints(BezierSegment[] paths, BezierSegment leftPath, BezierSegment rightPath)
        {
            if (paths.Length <= pageItem)
            {
                // 버튼 경로 지정
                firstPath = paths[0];
                secondPath = paths[1];
                thirdPath = paths[2];
                this.paths = paths;

                // 컨트롤 버튼 경로 지정
                this.leftPath = leftPath;
                this.rightPath = rightPath;

                leftBtn.SetFromPoint(leftPath.Point1);
                leftBtn.SetPath(leftPath);

                rightBtn.SetFromPoint(rightPath.Point1);
                rightBtn.SetPath(rightPath);
            }
        }

        /// <summary>
        /// 전체 페이지 계산
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageItemCount"></param>
        /// <returns></returns>
        private int CalTotalPage(int totalCount, int pageItemCount)
        {
            double total = totalCount / (double)pageItemCount; //  5 / 3 = 1
            return (int)Math.Ceiling(total);
        }

        /// <summary>
        /// 페이징 작업
        /// </summary>
        private void SetPaging()
        {
            int nowStart = (page - 1) * pageItem;  // 시작 지점
            // 현재 시작 아이템 지점
            // 현재 페이지에서 출력할 아이템 개수
            // 3개 3개 씩 딱 나누어 떨어져야함.
            //int nowPageItemCount = (totalItemCount -(page * pageItem)) % pageItem == 0 ? pageItem : totalItemCount % (page);
            int nowPageItemCount = totalItemCount / pageItem > page - 1 ? pageItem : totalItemCount % ((page-1) * pageItem);



            for (int i = 0; i < totalItemCount; i++)
            {
                AnimatedMenuButton btn = buttons.ElementAt(i);
                // 해당 하는 아이템은 출력
                if (i >= nowStart && i < (nowStart + nowPageItemCount))
                {
                    // 0,1,2,3 ,4,5
                    // 0,1,2, 3,4,5
                    int iperPage = (i % pageItem);

                    btn.SetPath(this.paths[iperPage]);

                    //int pathValue = (pathNum % pageItem)-1;
                    //if (pathValue == 0)
                    //{
                    //    // 1번째
                    //    btn.SetPath(firstPath);
                    //}
                    //else if (pathValue == 1)
                    //{
                    //    // 2번째
                    //    btn.SetPath(secondPath);
                    //}
                    //else
                    //{
                    //    // 마지막
                    //    btn.SetPath(thirdPath);
                    //}
                    btn.ToggleVisible(true);
                }
                else
                {
                    btn.ToggleVisible(false);
                }
            }

            // 컨트롤 버튼 출력 결정
            SetVisibleLeftControlButton();
            SetVisibleRightControlButton();
        }

        /// <summary>
        /// 왼쪽 컨트롤 버튼 출력 결정
        /// </summary>
        private void SetVisibleLeftControlButton()
        {
            if (page > 1)
            {
                // 이전 페이지 아이콘 표시
                UIControl.SetVisibility(leftBtn.image, true);
                leftBtn.MenuAnimationStart();
            }
            else
            {
                // 이전 페이지 아이콘 제거
                UIControl.SetVisibility(leftBtn.image, false);
                leftBtn.SetFromPoint();
            }
        }

        /// <summary>
        /// 오른쪽 컨트롤 버튼 출력 결정
        /// </summary>
        private void SetVisibleRightControlButton()
        {
            if (page < totalPage)
            {
                // 다음 페이지 아이콘 표시
                UIControl.SetVisibility(rightBtn.image, true);
                rightBtn.MenuAnimationStart();
            }
            else
            {
                // 다음 페이지 아이콘 제거
                UIControl.SetVisibility(rightBtn.image, false);
                rightBtn.SetFromPoint();
            }
        }

        /// <summary>
        /// 마우스 휠 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is Image || sender is Canvas)
            {
                if (e.Delta > 0) // 휠을 위로 스크롤
                {
                    PageUP();
                }
                else if (e.Delta < 0) // 휠을 아래로 스크롤
                {
                    PageDown();
                }
            }
        }

        // 좌우 버튼 이벤트
        private void SelectMenuLeft(object sender, RoutedEventArgs e)
        {
            PageUP();
        }
        private void SelectMenuRight(object sender, RoutedEventArgs e)
        {
            PageDown();
        }

        /// <summary>
        /// 페이지 올리기
        /// </summary>
        private void PageUP()
        {
            page -= 1;
            if (page <= 0)
            {
                page = 1;
            }
            else
            {
                SetPaging();
            }

        }
        /// <summary>
        /// 페이지 내리기
        /// </summary>
        private void PageDown()
        {
            page += 1;
            if (page > totalPage)
            {
                page = totalPage;
            }
            else
            {
                SetPaging();
            }
        }


        /// <summary>
        /// 메뉴 보이기
        /// </summary>
        public void Show()
        {
            isShow = true;
            SetUIVisbility();
        }
        public void Hide()
        {
            isShow = false;
            SetUIVisbility();
        }
        public void Toggle()
        {
            isShow = !isShow;
            SetUIVisbility();
        }

        /// <summary>
        /// 내부 버튼 출력 여부
        /// </summary>
        private void SetUIVisbility()
        {
            if (isShow)
            {
                SetPaging();
            }
            else
            {
                foreach (var button in buttons)
                {
                    button.Hide();
                }
                leftBtn.Hide();
                rightBtn.Hide();
            }
        }

        /// <summary>
        /// 패스 생성
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <returns></returns>
        public static BezierSegment CreatePath(Point first, Point second, Point third)
        {
            BezierSegment path = new BezierSegment();
            path.Point1 = first;
            path.Point2 = second;
            path.Point3 = third;
            return path;
        }
    }

    internal class AnimatedMenuButton
    {
        public Image image;
        public Canvas canvas;
        private Point fromPoint;
        private BezierSegment? path;
        private const double duration = 0.2;
        bool isShow = true;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="imgUri"></param>
        /// <param name="leftClickHandler"></param>
        /// <param name="rightClickHandler"></param>
        public AnimatedMenuButton(string title, Canvas canvas, string imgUri, Delegate? leftClickHandler, Delegate? rightClickHandler)
        {
            image = new Image();
            image.Width = 50;
            image.Height = 50;
            this.canvas = canvas;
            // 이미지 설정
            SetImage(imgUri);

            if (leftClickHandler != null)
            {
                image.AddHandler(UIElement.MouseLeftButtonDownEvent, leftClickHandler);
            }
            if (rightClickHandler != null)
            {
                image.AddHandler(UIElement.MouseRightButtonDownEvent, rightClickHandler);
            }
        }

        /// <summary>
        /// 시작점 지정
        /// </summary>
        /// <param name="fromPoint"></param>
        public void SetFromPoint(Point fromPoint)
        {
            this.fromPoint = fromPoint;
            SetImagePosition(this.fromPoint);
        }
        public void SetFromPoint()
        {
            SetImagePosition(fromPoint);
        }

        /// <summary>
        /// 경로 지정
        /// </summary>
        /// <param name="paht"></param>
        public void SetPath(BezierSegment path)
        {
            this.path = path;
        }

        // 이미지 설정
        protected void SetImage(string imgUri)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imgUri, UriKind.Absolute);
            bitmap.EndInit();
            image.Source = bitmap;
        }

        /// <summary>
        /// 버튼 위치 지정
        /// </summary>
        /// <param name="pos"></param>
        private void SetImagePosition(Point pos)
        {
            Canvas.SetLeft(image, pos.X);
            Canvas.SetTop(image, pos.Y);
        }

        // 애니메이션 시작
        public void MenuAnimationStart()
        {
            if (path == null)
            {
                return;
            }

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = fromPoint;  // 시작 지점

            //경유지
            pathFigure.Segments.Add(path);
            pathGeometry.Figures.Add(pathFigure);

            // 애니메이션 생성
            DoubleAnimationUsingPath xAnimation = new DoubleAnimationUsingPath
            {
                PathGeometry = pathGeometry,
                Source = PathAnimationSource.X,
                Duration = TimeSpan.FromSeconds(duration)
            };
            DoubleAnimationUsingPath yAnimation = new DoubleAnimationUsingPath
            {
                PathGeometry = pathGeometry,
                Source = PathAnimationSource.Y,
                Duration = TimeSpan.FromSeconds(duration)
            };

            // 스토리 보드 지정
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            Storyboard.SetTarget(xAnimation, image);
            Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));

            Storyboard.SetTarget(yAnimation, image);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(Canvas.Top)"));

            // 애니메이션을 시작합니다.
            storyboard.Begin();
        }

        /// <summary>
        /// UI 숨기기
        /// </summary>
        public void Hide()
        {

            // 초기 위치 이동
            SetImagePosition(fromPoint);
            ToggleVisible(false);
        }

        /// <summary>
        /// UI 출력하기
        /// </summary>
        public void Show()
        {
            ToggleVisible(true);
            MenuAnimationStart();
        }

        /// <summary>
        /// 버튼 출력 여부 설정
        /// </summary>
        public void ToggleVisible()
        {
            isShow = !isShow;
            UIControl.SetVisibility(image, isShow);
            if (isShow)
            {
                // 출력해야하는 경우
                MenuAnimationStart();
            }
            else
            {
                // 닫아야하는 경우
                SetImagePosition(fromPoint);
            }

        }
        public void ToggleVisible(bool value)
        {
            isShow = value;
            UIControl.SetVisibility(image, isShow);
            if (isShow)
            {
                // 출력해야하는 경우
                MenuAnimationStart();
            }
            else
            {
                // 닫아야하는 경우
                SetImagePosition(fromPoint);
            }
        }
    }

}
