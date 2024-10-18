using RasingDeokPal.Character.Item;
using RasingDeokPal.Common;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RasingDeokPal.Components
{
    internal class ItemAlert
    {
        Image img;
        Label textTitle;
        TextBlock textItemDesc;
        Image imgIcon;

        int iconMarginLeft  = 15;
        int iconMarginTop   = 35;
        int itemDescLeft = 80;
        int itemDescTop = 35;

        private int margin = 10;
        private Canvas canvas;


        public ItemAlert(Canvas canvas, double width, double height, GameItem item)
        {
            this.canvas = canvas;
            img = new Image();
            img.Width = width;
            img.Height = height;

            BitmapImage bitmap = UIControl.CreateBitmap("pack://application:,,,/asset/ui/ui_alert.png");
            img.Source = bitmap;

            // 텍스트 영역
            string titleContent = "아이템 획득";
            textTitle = new Label();
            textTitle.Width = width - margin;
            textTitle.Height = height - margin;
            textTitle.Content = titleContent;
            textTitle.IsHitTestVisible = false;
            textTitle.FontWeight = FontWeights.Bold;
            textTitle.FontSize = 12;
            textTitle.IsHitTestVisible = false;

            // 아이템 아이콘
            imgIcon = UIControl.CreateImage(item.imgIcon);
            imgIcon.IsHitTestVisible = false;

            // 아이템 설명
            textItemDesc = new TextBlock();
            textItemDesc.TextWrapping = TextWrapping.Wrap;
            textItemDesc.Text = item.ItemName;
            textItemDesc.Width = 165;
            textItemDesc.Height = 30;
            textItemDesc.FontSize = 14;
            textItemDesc.FontWeight = FontWeights.Bold;
            textItemDesc.Padding = new Thickness(10);
            textItemDesc.IsHitTestVisible = false;
            AddUIElement();


            // 창 위치 조정
            int centerX = (int)(canvas.Width/2 - (width / 2));
            int centerY = (int)(canvas.Height/2 - (height / 2));

            UIControl.SetCanvasMargin(img, centerX, centerY);
            UIControl.SetCanvasMargin(imgIcon, centerX+ iconMarginLeft, centerY + iconMarginTop);
            UIControl.SetCanvasMargin(textTitle, centerX + margin, centerY + margin);
            UIControl.SetCanvasMargin(textItemDesc, centerX + itemDescLeft, centerY + itemDescTop);

            // 마우스 버튼 클릭
            img.MouseDown += ImgMouseLeftButtonDown;
            
        }

        private void ImgMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("창 클릭");
            GameManager.Instance.GameStart();
        }

        private void AddUIElement()
        {
            canvas.Children.Add(img);
            canvas.Children.Add(textTitle);
            canvas.Children.Add(imgIcon);
            canvas.Children.Add(textItemDesc);
        }
        public void RemoveSelf()
        {
            canvas.Children.Remove(img);
            canvas.Children.Remove(textTitle);
            canvas.Children.Remove(imgIcon);
            canvas.Children.Remove(textItemDesc);
        }
    }
}
