using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using RasingDeokPal.Common;
using RasingDeokPal.effect;
using System.Windows.Media;

namespace DukpalKang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            this.MouseLeftButtonDown += new MouseButtonEventHandler(MainWindow_MouseLeftButtonDown);
#endif
            // 창 크기 제한
            WindowControlMethod.SetWindowSize(this, GameConfig.GetConfig().WindowWidth, GameConfig.GetConfig().WindowHeight);
            // 타이틀 설정
            WindowControlMethod.SetTitle(this);
            //WindowControlMethod.SetWindowPosition(0, 0);

            // 이펙트 레이어 싱글턴 지정
            EffectLayer effectLayer = EffectLayer.Instance;
            effectLayer.SetEffectCanvas(EffectCanvas);

            // 게임 매니저
            GameManager manager = new GameManager(this);
            manager.ProgramStart();

            manager.SetBackGround();
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //포인터 위치 가져오기
            Point position = e.GetPosition(this);

            // HitTest 메소드를 사용하여 위치에 있는 오브젝트 찾기
            HitTestResult result = VisualTreeHelper.HitTest(this, position);
            if (result != null)
            {
                //MessageBox.Show($"Hit object: {result.VisualHit.GetType().Name}");
                Debug.WriteLine($"Hit object: {result.VisualHit.GetType().Name}");
            }
        }

        /// <summary>
        /// 윈도우 로드 완료 시점
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 시작 프로그램 등록
            WindowControlMethod.SetRegistry();
        }

        /// <summary>
        /// 윈도우 키 다운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // 애플리케이션 종료
                this.Close();
            }
            if(e.Key == Key.B) 
            {
                GameManager.Instance.ToggleBackGround();
            }
        }
    }
}