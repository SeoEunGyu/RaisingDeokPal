
using RasingDeokPal.Character.Item;
using RasingDeokPal.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RasingDeokPal.Components
{
    internal class ItemInfoAlert
    {
        Image imgDescriptionBox;
        Label textItemTitle;
        TextBlock textItemDesc;
        Canvas canvas;

        int titleMarginLeft = 15;
        int titleMarginTop = 8;

        int descMarginLeft = 18;
        int descMarginTop = 35;

        public ItemInfoAlert(Canvas canvas, int left, int top)
        {
            this.canvas = canvas;
            
            // 이미지 영역
            imgDescriptionBox = UIControl.CreateImage("pack://application:,,,/asset/ui/ui_description_wood.png");
            imgDescriptionBox.Width = 185;
            imgDescriptionBox.Height = 80;
            imgDescriptionBox.IsHitTestVisible = false;
            imgDescriptionBox.Visibility = Visibility.Hidden;
            this.canvas.Children.Add(imgDescriptionBox);
            UIControl.SetCanvasMargin(imgDescriptionBox, left, top);

            // 아이템 타이틀
            textItemTitle = new Label();
            textItemTitle.Content = "아이템 명";
            textItemTitle.Visibility = Visibility.Hidden;
            textItemTitle.FontWeight = FontWeights.Bold;
            textItemTitle.FontSize = 12;
            this.canvas.Children.Add(textItemTitle);
            UIControl.SetCanvasMargin(textItemTitle, left + titleMarginLeft, top + titleMarginTop);

            // 아이템 설명
            textItemDesc = new TextBlock();
            textItemDesc.TextWrapping = TextWrapping.Wrap;
            textItemDesc.Text = "아이템 텍스트";
            textItemDesc.Width = 165;
            textItemDesc.Height = 30;
            textItemDesc.FontSize = 11;
            //textItemDesc.Background = new SolidColorBrush(Colors.Aqua);
            textItemDesc.Visibility = Visibility.Hidden;
            this.canvas.Children.Add(textItemDesc);
            UIControl.SetCanvasMargin(textItemDesc, left + descMarginLeft, top + descMarginTop);

        }

        /// <summary>
        /// 아이템 정보 반영
        /// </summary>
        /// <param name="item"></param>
        public void SetItemInfo(GameItem item)
        {
            textItemTitle.Content = item.ItemName;
            textItemDesc.Text = item.ItemDescription;
        }
        

        public void Show()
        {
            SetVisibility(true);
        }

        public void Hide()
        {
            SetVisibility(false);
        }
        public void SetVisibility(bool value)
        {
            imgDescriptionBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            textItemTitle.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            textItemDesc.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
