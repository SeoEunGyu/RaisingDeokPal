using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;


namespace RasingDeokPal.Components.Menu
{
    /// <summary>
    /// 메뉴 패널
    /// </summary>
    internal class MenuPanel
    {
        Canvas canvas;
        StackPanel panel;
        protected bool isShow = false;
        public int page = 1;
        List<MenuButton> buttons;
        MenuButton btnLeft;
        MenuButton btnRight;

        // 페이징 상수
        private int totalItemCount;     // 전체 아이템 수
        private const int pageItem = 3; // 페이지 출력 아이템
        private int totalPage;          // 전체 페이지
        private const int menuBtnMargin = 10;   // 메뉴 버튼 마진 값
        private const int menuBtnSize = 50;     // 메뉴 버튼 크기

        // 패널의 zIndex 값
        private const int zIndex = 20;


        public MenuPanel(Canvas canvas, List<MenuButton> buttons)
        {
            // 패널 기본 설정
            this.canvas = canvas;
            panel = new StackPanel();
            this.buttons = buttons;
            panel.Orientation = Orientation.Horizontal;
            // 패널 크기 지정
            int panelWidth = pageItem * (menuBtnSize + menuBtnMargin * 2);
            int panelHeight = menuBtnSize + menuBtnMargin * 2;
            panel.Width = panelWidth;
            panel.Height = panelHeight;
            UIControl.SetZindex(panel, zIndex);

#if DEBUG
            panel.Background = new SolidColorBrush(Colors.BurlyWood);
#endif

            // 패널 가로 가운데 정렬
            SetPanelLeftCenter();

            //패널에 버튼 추가
            foreach (MenuButton button in buttons)
            {
                panel.Children.Add(button.buttonElement);
                button.ToggleVisible(false);
            }
            // 패널에 마우스 휠 이벤트 추가
            panel.MouseWheel += PanelMouseWheel;

            // 왼쪽 오른쪽 이동 버튼
            double leftMarginLeft = panel.ActualWidth - menuBtnSize / 2 - menuBtnMargin;
            double RightMarginRight = panel.ActualWidth + panel.Width + menuBtnMargin;
            btnLeft = new MenuButton("<", menuBtnSize / 2, menuBtnSize, leftMarginLeft, 10, 0, 0, new RoutedEventHandler(SelectMenuLeft), null);
            btnRight = new MenuButton(">", menuBtnSize / 2, menuBtnSize, RightMarginRight, 10, 0, 0, new RoutedEventHandler(SelectMenuRight), null);

            // 캔버스에 패널 추가
            this.canvas.Children.Add(btnLeft.buttonElement);
            this.canvas.Children.Add(panel);
            this.canvas.Children.Add(btnRight.buttonElement);


            // 페이징 상수 계산
            totalItemCount = buttons.Count; // 전체 아이템 개수
            totalPage = CalTotalPage(totalItemCount, pageItem);

            // 페이징 작업
            SetPaging();
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
        /// 패널 가운데 정렬
        /// </summary>
        private void SetPanelLeftCenter()
        {
            // 패널 가운데 정렬
            double canvasWidth = canvas.Width;
            double panelWidth = panel.Width;
            double left = (canvasWidth - panelWidth) / 2;
            Canvas.SetLeft(panel, left);
        }

        /// <summary>
        /// 페이징 작업
        /// </summary>
        private void SetPaging()
        {
            int nowStart = (page - 1) * pageItem;  // 시작 지점
            // 현재 시작 아이템 지점
            // 현재 페이지에서 출력할 아이템 개수
            int nowPageItemCount = totalItemCount % page == 0 ? pageItem : totalItemCount % page;

            for (int i = 0; i < totalItemCount; i++)
            {
                MenuButton btn = buttons.ElementAt(i);
                // 해당 하는 아이템은 출력
                if (i >= nowStart && i <= nowStart + nowPageItemCount)
                {
                    btn.ToggleVisible(true);
                }
                else
                {
                    btn.ToggleVisible(false);
                }
            }
            // 컨트롤 버튼 출력 결정
            ShowLeftRightButton();
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
            SetPaging();
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
            SetPaging();
        }

        /// <summary>
        /// 마우스 휠 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is StackPanel || sender is Button)
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

        /// <summary>
        /// 컨트롤 버튼 출력 결정
        /// </summary>
        private void ShowLeftRightButton()
        {
            if (page > 1)
            {
                // 이전 페이지 아이콘 표시
                UIControl.SetVisibility(btnLeft.buttonElement, true);
            }
            else
            {
                // 이전 페이지 아이콘 제거
                UIControl.SetVisibility(btnLeft.buttonElement, false);
            }

            if (page < totalPage)
            {
                // 다음 페이지 아이콘 표시
                UIControl.SetVisibility(btnRight.buttonElement, true);
            }
            else
            {
                // 다음 페이지 아이콘 제거
                UIControl.SetVisibility(btnRight.buttonElement, false);
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
        private void SetUIVisbility()
        {
            // 패널 출력 여부 결정
            UIControl.SetVisibility(panel, isShow);

            // 출력하는 상황인 경우, Left, Right 버튼은 페이징 확인하고 출력
            if (isShow)
            {
                ShowLeftRightButton();
            }
            else
            {
                UIControl.SetVisibility(btnLeft.buttonElement, isShow);
                UIControl.SetVisibility(btnRight.buttonElement, isShow);
            }
        }
    }

    /// <summary>
    /// 메뉴 버튼
    /// </summary>
    internal class MenuButton
    {
        public Button buttonElement;
        bool isShow = true;
        private const int zIndex = 21;

        public MenuButton(string title, Delegate? leftClickHandler, Delegate? rightClickHandler)
        {
            buttonElement = new Button();
            buttonElement.Content = title;
            buttonElement.Width = 50;
            buttonElement.Height = 50;
            buttonElement.Margin = new Thickness(10);
            UIControl.SetZindex(buttonElement, zIndex);

            if (leftClickHandler != null)
            {
                buttonElement.AddHandler(ButtonBase.ClickEvent, leftClickHandler);
            }
            if (rightClickHandler != null)
            {
                buttonElement.AddHandler(UIElement.MouseRightButtonDownEvent, rightClickHandler);
            }
        }
        public MenuButton(string title, int width, int height, double marginLeft, double marginTop, double marginRight, double marginDown, Delegate? leftClickHandler, Delegate? rightClickHandler)
        {
            buttonElement = new Button();
            buttonElement.Content = title;
            buttonElement.Width = width;
            buttonElement.Height = height;
            buttonElement.Margin = new Thickness(marginLeft, marginTop, marginRight, marginDown);
            UIControl.SetZindex(buttonElement, zIndex);

            if (leftClickHandler != null)
            {
                buttonElement.AddHandler(ButtonBase.ClickEvent, leftClickHandler);
            }
            if (rightClickHandler != null)
            {
                buttonElement.AddHandler(UIElement.MouseRightButtonDownEvent, rightClickHandler);
            }
        }


        /// <summary>
        /// 핸들러 추가
        /// </summary>
        /// <param name="e"></param>
        /// <param name="handler"></param>
        public void AddHandler(RoutedEvent e, Delegate handler)
        {
            buttonElement.AddHandler(e, handler, true);
        }

        /// <summary>
        /// 버튼 출력 여부 설정
        /// </summary>
        public void ToggleVisible()
        {
            isShow = !isShow;
            UIControl.SetVisibility(buttonElement, isShow);
        }
        public void ToggleVisible(bool value)
        {
            isShow = value;
            UIControl.SetVisibility(buttonElement, isShow);
        }
    }
}
