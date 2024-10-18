using Microsoft.VisualBasic.ApplicationServices;
using RasingDeokPal.Character.Item;
using RasingDeokPal.Common;
using RasingDeokPal.Common.Save;
using RasingDeokPal.Components;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static RasingDeokPal.Common.Enums.CommonEnum;


namespace RasingDeokPal.View
{
    internal class SoilChangeView : UIView
    {
        ItemInventory imgListBox;
        Image imgEquipBox;
        ItemInfoAlert imgDescriptionBox;

        

        // 장비
        Image imgSlotPlant;
        Image imgSlotSoil;
        Image imgSlotPot;

        // 장비 아이템
        GameItem? itemSoil;
        GameItem? itemPot;
        GameItem? itemPlant;

        bool isShow = false;

        private PlayerData playerData;
        private const int inventoryWidth = 380;
        private const int inventoryHeight = 70;
        private const int marginTop = -300;

        /// <summary>
        /// 장비 변경 뷰
        /// </summary>
        /// <param name="canvas"></param>
        public SoilChangeView(Canvas canvas) : base(canvas)
        {
            // 이미지 장비 버튼
            imgEquipBox = UIControl.CreateImage("pack://application:,,,/asset/ui/ui_equip_wood.png");
            imgEquipBox.Width = 185;
            imgEquipBox.Height = 80;
            imgEquipBox.IsHitTestVisible = false;

            this.canvas.Children.Add(imgEquipBox);

            // 기준이 되는 지점
            int pivotWidth  = (GameConfig.GetConfig().WindowWidth / 2) - (int)(inventoryWidth / 2);
            int pivotHeight = (GameConfig.GetConfig().WindowWidth / 2) + marginTop;


            UIControl.SetCanvasMargin(imgEquipBox, pivotWidth, pivotHeight + 80);

            // 인벤토리
            imgListBox = new ItemInventory(canvas, pivotWidth, pivotHeight);
            // 이미지 설명 박스
            imgDescriptionBox = new ItemInfoAlert(canvas, pivotWidth + 190, pivotHeight + 80);


            // 이미지 아이콘 객체 초기화
            if (imgSlotPlant == null)
            {
                int Left = pivotWidth + 16;
                int Top = pivotHeight + 98;
                imgSlotPlant = new Image();
                imgSlotPlant.MouseEnter += new MouseEventHandler(PlantSlotMouseEnter);
                imgSlotPlant.MouseLeave += new MouseEventHandler(SlotMouseLeave);
                this.canvas.Children.Add(imgSlotPlant);
                imgSlotPlant.IsHitTestVisible = true;
                UIControl.SetCanvasMargin(imgSlotPlant, Left, Top);
                imgSlotPlant.Visibility = System.Windows.Visibility.Visible;
            }
            if (imgSlotSoil == null)
            {
                // 흙 아이콘 
                int Left = pivotWidth + 71;
                int Top = pivotHeight + 98;

                imgSlotSoil = new Image();
                imgSlotSoil.MouseEnter += new MouseEventHandler(SoilSlotMouseEnter);
                imgSlotSoil.MouseLeave += new MouseEventHandler(SlotMouseLeave);
                this.canvas.Children.Add(imgSlotSoil);
                imgSlotSoil.IsHitTestVisible = true;
                UIControl.SetCanvasMargin(imgSlotSoil, Left, Top);
                imgSlotSoil.Visibility = System.Windows.Visibility.Visible;
            }
            if (imgSlotPot == null)
            {
                // 화분 아이콘 
                int Left = pivotWidth + 126;
                int Top = pivotHeight + 98;

                imgSlotPot = new Image();
                imgSlotPot.MouseEnter += new MouseEventHandler(PotSlotMouseEnter);
                imgSlotPot.MouseLeave += new MouseEventHandler(SlotMouseLeave);
                this.canvas.Children.Add(imgSlotPot);
                imgSlotPot.IsHitTestVisible = true;
                UIControl.SetCanvasMargin(imgSlotPot, Left, Top);
                imgSlotPot.Visibility = System.Windows.Visibility.Visible;
            }

            LoadPlayerItem();
        }

        private void PlantSlotMouseEnter(object sender, MouseEventArgs e)
        {
            if (itemPlant != null)
            {
                ShowItemDescriptionBox(itemPlant);
            }
        }

        private void SoilSlotMouseEnter(object sender, MouseEventArgs e)
        {
            if(itemSoil != null)
            {
                ShowItemDescriptionBox(itemSoil);
            }
        }
        private void PotSlotMouseEnter(object sender, MouseEventArgs e)
        {
            if (itemPot != null) 
            {
                ShowItemDescriptionBox(itemPot);
            }
        }
        private void SlotMouseLeave(object sender, MouseEventArgs e)
        {
            HideDescriptionBox();
        }

        /// <summary>
        /// 아이템 설명
        /// </summary>
        /// <param name="item"></param>
        internal void ShowItemDescriptionBox(GameItem item)
        {
            if (item != null)
            {
                imgDescriptionBox.SetItemInfo(item);
                imgDescriptionBox.Show();
            }
        }
        /// <summary>
        /// 아이템 설명 닫기
        /// </summary>
        internal void HideDescriptionBox()
        {
            imgDescriptionBox.Hide();
        }

        private void SetVisiblity()
        {
            imgListBox.SetVisibility(isShow);
            UIControl.SetVisibility(imgEquipBox, isShow);
            //imgDescriptionBox.SetVisibility(isShow);

            // 아이콘
            UIControl.SetVisibility(imgSlotPlant, isShow);
            UIControl.SetVisibility(imgSlotSoil, isShow);
            UIControl.SetVisibility(imgSlotPot, isShow);
        }

        /// <summary>
        /// UI 출력하는 상황일때 호출
        /// </summary>
        internal override void Update()
        {
            //Debug.WriteLine("아이템 업데이트");
            LoadPlayerItem();
            imgListBox.SetPaging();
        }

        private void LoadPlayerItem()
        {
            // 플레이어 데이터 로드
            playerData = GameManager.Instance.playerData;
            int[] curEquip = playerData.GetEquipData();

            // 본체 장비 이미지 아이콘
            if (curEquip[0] != (int)ItemCode.None)
            {
                // 장착된 경우, 이미지 아이콘 생성
                itemPlant = (ItemCharacter)GameItem.GetItem(curEquip[0]);
                imgSlotPlant.Source = itemPlant.imgIcon;
            }
            // 흙 장비 이미지 아이콘
            if (curEquip[1] != (int)ItemCode.None)
            {
                // 장착된 경우, 이미지 아이콘 생성
                itemSoil = (ItemSoil)GameItem.GetItem(curEquip[1]);
                imgSlotSoil.Source = itemSoil.imgIcon;
            }
            // 화분 장비 이미지 아이콘
            if (curEquip[2] != (int)ItemCode.None)
            {
                // 장착된 경우, 이미지 아이콘 생성
                itemPot = (ItemPot)GameItem.GetItem(curEquip[2]);
                imgSlotPot.Source = itemPot.imgIcon;
            }

        }


        public void Show()
        {
            isShow = true;
            SetVisiblity();
        }

        public void Hide()
        {
            isShow = false;
            SetVisiblity();
        }

        public void Toggle()
        {
            isShow = !isShow;
            SetVisiblity();
        }

        
    }
}
