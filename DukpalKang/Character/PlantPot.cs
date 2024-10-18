using RasingDeokPal.Character.Item;
using RasingDeokPal.Character.Unit;
using RasingDeokPal.Common;
using RasingDeokPal.effect;
using RasingDeokPal.View;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Character
{
    internal class PlantPot : SubUnit
    {
        private DeokPal mainUnit;

        private int potMarginLeftValue = 40;
        private int potMarginTopValue = 10;

        DispatcherTimer? twinkleTimer;
        private const int twinkeCoolTime = 3;

        // 스텟 관련
        public double maxWater         = 100.0;  // 최대 물 저장량
        public double evaporationWater = 10.0;   // 물 증발 수치

        private int potMarginLeft;
        private int potMarginTop;

        ItemPot item;

        public PlantPot(DeokPal mainUnit, Canvas canvas, ItemPot item, int marginLeft, int marginTop, int zIndex) : base(canvas, item.resourceUri, zIndex)
        {
            this.mainUnit = mainUnit;
            potMarginLeft = marginLeft - (bitmapWidth / 2) + potMarginLeftValue;
            potMarginTop = marginTop + potMarginTopValue;

            SetUIMargin(potMarginLeft, potMarginTop);

            // 이벤트 지정
            AddHandler(Image.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftDown));
            AddHandler(Image.MouseMoveEvent, new System.Windows.Input.MouseEventHandler(MouseMove));
            AddHandler(Image.MouseLeftButtonUpEvent, new MouseButtonEventHandler(MouseLeftUp));
            AddHandler(Image.MouseRightButtonDownEvent, new MouseButtonEventHandler(MouseRightDown));

            SetItem(item);
        }

        /// <summary>
        /// 아이템 설정
        /// </summary>
        public void SetItem(ItemPot item)
        {
            ClearItemOption();
            SetImageSource(image, item.resourceUri);
            this.item = item;
            item.SetItemOption(this);
        }

        /// <summary>
        /// 화분 아이템 옵션 초기화
        /// </summary>
        private void ClearItemOption()
        {
            if (twinkleTimer != null)
            {
                twinkleTimer.Stop();
                twinkleTimer = null;
            }
        }

        /// <summary>
        /// 화분 색상 변경
        /// </summary>
        public void SetPotColor()
        {
            UIControl.SetImageColor(image, this.item.Color);
        }

        /// <summary>
        /// 유니크 화분인 경우 일정 시간마다 반짝이 효과 생성
        /// </summary>
        public void SetTwinkleTimer()
        {
            Debug.WriteLine($"""[유니크 효과] 황금 덕팔이의 화분은 언제나 반짝입니다.""");
            twinkleTimer = new DispatcherTimer();
            twinkleTimer.Interval = TimeSpan.FromSeconds(twinkeCoolTime);
            twinkleTimer.Tick += CreateTwinkleEffect;
            twinkleTimer.Start();
        }

        /// <summary>
        /// 반짝이 이펙트 생성
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateTwinkleEffect(object? sender, EventArgs e) 
        {
            EffectLayer effectLayer = EffectLayer.Instance;
            Random rand = new Random();

            // 왼쪽 효과
            int effectLeft = (int)(potMarginLeft) + rand.Next(-100, 100);
            int effectTop = potMarginTop + rand.Next(-(bitmapHeight / 2), bitmapHeight / 2);
            effectLayer.EffectTwinkle(effectLeft, effectTop);
        }

        /// <summary>
        /// 화분 좌클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            InteractionMenu selectedMenu = GameManager.Instance.GetGameView().selectedMenu;
            if (selectedMenu.Equals(InteractionMenu.None))
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //// 현재 마우스 좌표를 WPF 좌표계로 변환합니다.
                    //var mousePos = WindowControlMethod.GetMouseCursorCenterPoint();
                    ////Debug.WriteLine($"""현재 마우스 좌표 {mousePos}""");
                    //Point winSize = WindowControlMethod.GetWindowSize();
                    //mousePos.X -= winSize.X / 2;
                    //mousePos.Y -= winSize.Y / 2;
                    //WindowControlMethod.SetWindowPosition(mousePos);

                    // 덕팔이 애니메이션 정지
                    mainUnit.DragStart();
                }
            }
        }
        public void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mainUnit.MouseMove(sender, e);
        }
        /// <summary>
        /// 화분 좌클릭 업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            //InteractionMenu selectedMenu = GameManager.Instance.GetGameView().selectedMenu;
            //if (selectedMenu.Equals(InteractionMenu.None))
            //{
            //    var mousePos = WindowControlMethod.GetMouseCursorCenterPoint();

            //    Point winSize = WindowControlMethod.GetWindowSize();
            //    mousePos.X -= (winSize.X / 2);
            //    mousePos.Y -= (winSize.Y / 2) + (potMarginTop/2);
            //    WindowControlMethod.SetWindowPosition(mousePos);

            //    mainUnit.DragStoryBoardUpdate();
            //}
            mainUnit.MouseLeftUp(sender, e);
        
        }
        /// <summary>
        /// 마우스 우클릭 다운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            // 장비 UI 종료
            GameManager.Instance.CloseEquipUI();
            // 화분 메뉴 토글, 플레이어 메뉴 닫기
            GameManager.Instance.GetGameView().TogglePodMenu();
        }
    }
}
