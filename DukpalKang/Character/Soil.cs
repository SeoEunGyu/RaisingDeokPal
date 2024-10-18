using RasingDeokPal.Character.Unit;
using RasingDeokPal.Common;
using RasingDeokPal.View;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Character
{
    /// <summary>
    /// 덕팔이 흙 유닛
    /// </summary>
    internal class Soil : SubUnit
    {
        private int imageMarginLeft = 40;
        private int imageMarginTop = -20;
        private int[] color;
        DeokPal mainUnit;


        public Soil(DeokPal mainUnit, Canvas canvas, string imgUri, int statusWater, double marginLeft, double marginTop, int zIndex) : base(canvas, imgUri)
        {
            this.mainUnit = mainUnit;
            double width = this.bitmapOrigin.Width;
            double height = this.bitmapOrigin.Height;

            int soilMarginLeft = (int)((marginLeft - width / 2) + imageMarginLeft);
            int soilMarginTop = (int)(marginTop + height/2) + imageMarginTop;
            SetUIMargin(soilMarginLeft, soilMarginTop);
            SetZIndex(zIndex);

            color = GetImageColor();

            // 물 상태 확인
            DoWater(statusWater);


            // 이벤트 지정
            AddHandler(Image.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftDown));
            AddHandler(Image.MouseRightButtonDownEvent, new MouseButtonEventHandler(MouseRightDown));
            AddHandler(Image.MouseEnterEvent, new MouseEventHandler(MouseEnter));
            AddHandler(Image.MouseLeaveEvent, new MouseEventHandler(MouseLeave));
        }
        
        /// <summary>
        /// 물주기
        /// </summary>
        public void DoWater(int statusWater)
        {
            double blue = (int)(((double)statusWater / 100) * 110); // 최대 컬러 값을 200으로 제한
            int soilColor = GetColor()[2]; // 0~255
            int dif = ((int)blue - soilColor);
            Debug.WriteLine($"""[물주기] 물 상태 {statusWater},색상 {soilColor} 변경할 값 {blue} 차이값{dif}""");
            if (dif > 0) 
            {
                CalmageColor(0, 0, dif, true);
            }
            else
            {
                CalmageColor(0, 0, dif, false);
            }
            
            color = GetImageColor();
        }

        /// <summary>
        /// 물빠짐
        /// </summary>
        public void DoWaterDrainage(int statusWater)
        {
            double blue = (int)(((double)statusWater / 100) * 110);
            int soilColor = GetColor()[2]; // 0~255
            int dif = (soilColor - (int)blue);
            if(dif > 0)
            {
                CalmageColor(0, 0, dif, false);
                color = GetImageColor();
            }
        }

        /// <summary>
        /// 흙 색상 리턴
        /// </summary>
        /// <returns></returns>
        public int [] GetColor()
        {
            return this.color;
        }


        /// <summary>
        /// 흙 마우스 진입
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseEnter(object sender, MouseEventArgs e)
        {
            mainUnit.MouseEnter(sender, e);
        }
        /// <summary>
        /// 흙 마우스 진입 해제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeave(object sender, MouseEventArgs e)
        {
            mainUnit.MouseLeave(sender, e);
        }
        /// <summary>
        /// 흙 마우스 우클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            GameManager.Instance.GetGameView().HideAllMenu();
        }
        /// <summary>
        /// 흙 마우스 좌클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            InteractionMenu selectedMenu = GameManager.Instance.GetGameView().selectedMenu;
            if (selectedMenu.Equals(InteractionMenu.Water))
            {
                // 덕팔이 물주기
                mainUnit.DoWater();
            }
        }
    }
}
