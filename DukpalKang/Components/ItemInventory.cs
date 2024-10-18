using RasingDeokPal.Character.Item;
using RasingDeokPal.Common;
using RasingDeokPal.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Components
{
    internal class ItemInventory
    {
        Image imgListBox;
        Canvas canvas;
        InventoryIcon[] imgList;

        Image imgLeftBtn;
        Image imgRightBtn;

        int left;
        int top;
        int itemImgSize = 48;
        int itemImgMarginLeft = 10;
        
        private int page = 1;
        private int totalItemCount;
        private int pageItem = 5;
        private int totalPage;

        public ItemInventory(Canvas canvas, int left, int top)
        {
            this.canvas = canvas;
            // 이미지 리스트 박스
            imgListBox = UIControl.CreateImage("pack://application:,,,/asset/ui/ui_slot_wood.png");
            imgListBox.Width = 380;
            imgListBox.Height = 70;
            this.canvas.Children.Add(imgListBox);

            this.left = left;
            this.top = top;
            UIControl.SetCanvasMargin(imgListBox, left, top);

            // 왼쪽 오른쪽 화살표
            imgLeftBtn = UIControl.CreateImage("pack://application:,,,/asset/ui/ui_left_wood.png");
            imgRightBtn = UIControl.CreateImage("pack://application:,,,/asset/ui/ui_right_wood.png");
            int leftBtnMarginLeft   = left + 10;
            int leftBtnMarginTop    = top + 20;
            int rightBtnMarginLeft  = left + (int)(imgListBox.Width) - ((int)imgRightBtn.Width + 10);
            int rightBtnMarginTop   = top + 20;
            UIControl.SetCanvasMargin(imgLeftBtn, leftBtnMarginLeft, leftBtnMarginTop);
            UIControl.SetCanvasMargin(imgRightBtn, rightBtnMarginLeft, rightBtnMarginTop);
            this.canvas.Children.Add(imgLeftBtn);
            this.canvas.Children.Add(imgRightBtn);
            imgLeftBtn.MouseDown += SelectMenuLeft;
            imgRightBtn.MouseDown += SelectMenuRight;

            CreateItemImage();
            SetPaging();
        }

       

        public void CreateItemImage()
        {
            // image
            imgList = new InventoryIcon[]
            {
                new InventoryIcon(this,UIControl.CreateImage("pack://application:,,,/asset/Icon/icon_normal_soil.png",itemImgSize,itemImgSize)),
                new InventoryIcon(this,UIControl.CreateImage("pack://application:,,,/asset/Icon/icon_normal_soil.png",itemImgSize,itemImgSize)),
                new InventoryIcon(this,UIControl.CreateImage("pack://application:,,,/asset/Icon/icon_normal_soil.png",itemImgSize,itemImgSize)),
                new InventoryIcon(this,UIControl.CreateImage("pack://application:,,,/asset/Icon/icon_normal_soil.png",itemImgSize,itemImgSize)),
                new InventoryIcon(this,UIControl.CreateImage("pack://application:,,,/asset/Icon/icon_normal_soil.png",itemImgSize,itemImgSize))
            };
            for (int i = 0; i<imgList.Length; i++)
            {
                UIControl.SetCanvasMargin(imgList[i].img, left + (itemImgSize * (i+1)) + (itemImgMarginLeft*i), top + 10);
                canvas.Children.Add(imgList[i].img);
                UIControl.SetVisibility(imgList[i].img, false); 
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

        public void SetPaging()
        {
            List<int> items = GameManager.Instance.playerData.GetItems();
            // 아이템 수
            totalItemCount = items.Count;
            totalPage = CalTotalPage(totalItemCount, pageItem);

            // 현재 시작 지점
            int nowStart = (page - 1) * pageItem;
            // 현재 페이지에서 몇개를 출력해야하는지
            int existItem = totalItemCount - ((page-1) * pageItem); // 남은 아이템
            int nowPageItemCount = existItem > pageItem ? pageItem : existItem;

            //int nowPageItemCount = (totalItemCount / pageItem >= page - 1) ? pageItem : totalItemCount % ((page - 1) * pageItem);
            int nowPageEnd = nowStart + nowPageItemCount - 1;
            for (int i= 0; i <= totalItemCount-1; i++)
            {
                int imgIndex = i >= imgList.Length ? i % imgList.Length : i;
                if (i >= nowStart && i < (nowStart + nowPageItemCount))
                {
                    // 카운트 보다 작으면 출력
                    // 아이템 출력
                    GameItem? item = GameItem.GetItem(items.ElementAt(i));
                    if (item != null)
                    {
                        imgList[imgIndex].SetItem(item);
                    }
                    UIControl.SetVisibility(imgList[imgIndex].img, true);
                }
                else
                {
                    if(i <= nowPageEnd)
                    {
                        UIControl.SetVisibility(imgList[imgIndex].img, false);
                    }
                }
            }

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
                UIControl.SetVisibility(imgLeftBtn, true);
            }
            else
            {
                // 이전 페이지 아이콘 제거
                UIControl.SetVisibility(imgLeftBtn, false);
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
                UIControl.SetVisibility(imgRightBtn, true);
            }
            else
            {
                // 다음 페이지 아이콘 제거
                UIControl.SetVisibility(imgRightBtn, false);
            }
        }

        private void SelectMenuLeft(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageUP();
            e.Handled = true;
        }

        private void SelectMenuRight(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageDown();
            e.Handled = true;
        }

        /// <summary>
        /// 페이지 올리기
        /// </summary>
        internal void PageUP()
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
        internal void PageDown()
        {
            page += 1;
            if (page > totalPage)
            {
                page = totalPage;
            }
            SetPaging();
        }

        public void Show()
        {
            SetVisibility(true);
        }
        public void Hide()
        {
            SetVisibility(false);
        }
        public void SetVisibility(bool isShow)
        {
            imgListBox.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
            UIControl.SetVisibility(imgLeftBtn, isShow);
            UIControl.SetVisibility(imgRightBtn, isShow);

            // 출력의 경우 아이템 설정
            if (isShow)
            {
                SetPaging();
            }
            else
            {
                foreach(InventoryIcon inventoryItem in  imgList)
                {
                    UIControl.SetVisibility(inventoryItem.img, false);
                }
            }
        }
    }

    /// <summary>
    /// 인벤토리 아이콘
    /// </summary>
    internal class InventoryIcon
    {
        public Image img { get; set; }
        public GameItem? item { get; set; }
        private ItemInventory inventory;
        public InventoryIcon(ItemInventory inventory, Image img)
        {
            this.inventory = inventory;
            this.img = img;
        }
        public void SetItem(GameItem item)
        {
            this.item = item;
            this.img.Source = item.imgIcon;
            this.img.MouseEnter += new MouseEventHandler(Img_MouseEnter);
            this.img.MouseLeave += new MouseEventHandler(Img_MouseLeave);
            this.img.MouseWheel += new MouseWheelEventHandler(Img_MouseWheel);
            this.img.MouseRightButtonDown += new MouseButtonEventHandler(Img_MouseRightButtonDown);
        }

        private void Img_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            GameManager.Instance.ChangeItem(item);
            e.Handled = true;
        }

        private void Img_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is Image || sender is Canvas)
            {
                if (e.Delta > 0) // 휠을 위로 스크롤
                {
                    this.inventory.PageUP();
                }
                else if (e.Delta < 0) // 휠을 아래로 스크롤
                {
                    this.inventory.PageDown();
                }
            }
            e.Handled = true;
        }

        private void Img_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(this.item != null)
            {
                SoilChangeView view = GameManager.Instance.GetEquipView();
                if (view != null)
                {
                    view.ShowItemDescriptionBox(this.item);
                }
                
            }
        }

        private void Img_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.item != null)
            {
                SoilChangeView view = GameManager.Instance.GetEquipView();
                if (view != null)
                {
                    view.HideDescriptionBox();
                }
            }
        }
    }
}
