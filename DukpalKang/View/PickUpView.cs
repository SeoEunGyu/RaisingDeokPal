using RasingDeokPal.Character.Item;
using RasingDeokPal.Common;
using RasingDeokPal.Components;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static RasingDeokPal.Common.Enums.CommonEnum;


namespace RasingDeokPal.View
{
    internal class PickUpView : UIView
    {
        Image imgPickUpBox;
        BitmapImage bitmapPickUpStanby;
        BitmapImage bitmapPickUpClick;
        BitmapImage bitmapPickUpDrag;
        BitmapImage bitmapPickUpDone;

        MediaElement media;

        private ItemAlert? uiAlert;

        Point dragStartPoint;
        private bool dragCompleted = false;
        private bool pickUpCompleted = false;

        private int dragMax = 250;

        private const int width = 700;
        private const int height = 700;

        private GameItem pickUpItem;

        public PickUpView(Canvas canvas) : base(canvas)
        {
            // 가챠 영상 
            media = UIControl.CreateMediaRelative("./asset/video/pickup_normal.mp4");
            media.Width = width;
            media.Height = height;
            media.Volume = 1.0;
            media.MediaEnded += PickUpVideoEnded;

            canvas.Background = new SolidColorBrush(Color.FromRgb(39, 42, 61));

            // 픽업 박스 이미지
            bitmapPickUpStanby  = UIControl.CreateBitmap("pack://application:,,,/asset/ui/pickup_base.png");
            bitmapPickUpClick   = UIControl.CreateBitmap("pack://application:,,,/asset/ui/pickup_2.png");
            bitmapPickUpDrag    = UIControl.CreateBitmap("pack://application:,,,/asset/ui/pickup_3.png");
            bitmapPickUpDone    = UIControl.CreateBitmap("pack://application:,,,/asset/ui/pickup_4.png");

            imgPickUpBox = UIControl.CreateImage(bitmapPickUpStanby);
            imgPickUpBox.Width = width;
            imgPickUpBox.Height = height;

            // 가로 마진 구하기
            int marginWidth = (GameConfig.GetConfig().WindowWidth - width)/2;
            UIControl.SetCanvasMargin(imgPickUpBox, marginWidth, 0);
            UIControl.SetCanvasMargin(media, marginWidth, 0);

            this.canvas.Children.Add(imgPickUpBox);
            this.canvas.Children.Add(media);

            // 픽업 박스 좌클릭
            imgPickUpBox.MouseLeftButtonDown += PickUpMouseLeftDown;
        }

        /// <summary>
        /// 마우스 좌클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickUpMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            PickUpStart();
        }

        /// <summary>
        /// 픽업 실행
        /// </summary>
        private void PickUpStart()
        {
            if (pickUpCompleted)
            {
                return;
            }

            // 처음 시작이면 덕팔이 뽑기 진행
            if (GameManager.Instance.isStartPack)
            {
                GameManager.Instance.isStartPack = false;
                CharacterPickUp();
            }
            else
            {
                ItemPickUp();
            }
        }

        /// <summary>
        /// 캐릭터 뽑기
        /// </summary>
        private void CharacterPickUp()
        {
            // 캐릭터 뽑기 및 아이템 장착
            // 아이템 결정
            pickUpItem = PickUpManager.GetRandomCharacter();
            PlayPickAnimation(pickUpItem);

            // 캐릭터 장착
            GameManager.Instance.GetStartPickUp(pickUpItem);
        }

        /// <summary>
        /// 아이템 뽑기 
        /// </summary>
        private void ItemPickUp()
        {
            // 아이템 결정
            pickUpItem = PickUpManager.GetRandomItem();
            PlayPickAnimation(pickUpItem);

            // 아이템 저장
            GameManager.Instance.GetPickUP(pickUpItem);
        }

        /// <summary>
        /// 뽑기 연출 재생
        /// </summary>
        /// <param name="item"></param>
        private void PlayPickAnimation(GameItem item)
        {
            if (item.rarity == DeokPalRarity.Unique)
            {
                // 유니크 연출 영상 재생
                UIControl.SetMediaSource(media, "./asset/video/pickup_unique.mp4");
            }
            else
            {
                // 일반 연출
                UIControl.SetMediaSource(media, "./asset/video/pickup_normal.mp4");
            }
            // 가챠 연출 재생
            imgPickUpBox.Visibility = Visibility.Hidden;
            media.SpeedRatio = 1;
            media.Play();
        }

        /// <summary>
        /// 픽업 종료
        /// </summary>
        private void PickUpEnd()
        {
            imgPickUpBox.Source = bitmapPickUpDone;
            pickUpCompleted = true;
            imgPickUpBox.IsHitTestVisible = false;

            Debug.WriteLine($"""가챠 진행""");
            uiAlert = new ItemAlert(canvas, 200, 100, pickUpItem);
            SoundManager.Instance.PlayBeep();
        }

        /// <summary>
        /// UI 요소 삭제
        /// </summary>
        public override void RemoveSelf()
        {
            canvas.Children.Remove(imgPickUpBox);
            canvas.Children.Remove(media);

            if(uiAlert != null)
            {
                uiAlert.RemoveSelf();
                uiAlert = null;
            }
        }

        private void PickUpVideoEnded(object sender, RoutedEventArgs e)
        {
            // 동영상 재생이 끝나면 메시지 표시
            //MessageBox.Show("영상 재생이 끝났습니다.");
            PickUpEnd();
        }
    }
}
