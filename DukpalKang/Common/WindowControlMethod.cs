using DukpalKang;
using Microsoft.Win32;
using RasingDeokPal.Components;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RasingDeokPal.Common
{
    /// <summary>
    /// Window 컨트롤 및 시스템 함수
    /// </summary>
    internal class WindowControlMethod
    {
        public static RegistryKey? runRegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static string titleName = "덕팔이 키우기";
        private static Random random = new Random();

        private static MouseCursor waterCursor = new MouseCursor("pack://application:,,,/asset/cursor/icon_water.cur");

        /// <summary>
        /// 타이틀 명 지정
        /// </summary>
        /// <param name="window"></param>
        public static void SetTitle(MainWindow window)
        {
            window.Title = titleName;
        }
        public static void SetTitle(MainWindow window, string title)
        {
            window.Title = title;
        }

        /// <summary>
        /// 윈도우 창 크기 설정
        /// </summary>
        /// <param name="window"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <remarks>
        /// 사이즈 조절 불가 항목 포함
        /// </remarks>
        public static void SetWindowSize(MainWindow window, int width, int height)
        {
            window.MaxWidth = width;
            window.MaxHeight = height;
            window.ResizeMode = ResizeMode.NoResize;
        }

        public static void SetWindowBackground(SolidColorBrush? background = null)
        {
            Window window = Application.Current.MainWindow;
            if(background != null)
            {
                window.Background = background;
            }
            else
            {
                window.Background = Brushes.Transparent;
            }
            
        }

        /// <summary>
        /// 윈도우 창 크기 반환
        /// </summary>
        /// <returns></returns>
        public static Point GetWindowSize()
        {
            Window window = Application.Current.MainWindow;
            return new Point(window.Width, window.Height);
        }

        /// <summary>
        /// 시작 프로그램 등록
        /// </summary>
        public static void SetRegistry()
        {
            // 레지스트 키가 없은 경우 시작 프로그램으로 등록
            if (runRegKey != null)
            {
                if (runRegKey.GetValue(titleName) == null)
                {
                    runRegKey.SetValue(titleName, Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName);
                }
                //else
                //{
                //    runRegKey.DeleteValue(titleName, false);
                //}
            }
        }

        /// <summary>
        /// 윈도우 창 위치 값 반환
        /// </summary>
        /// <returns></returns>
        public static Point GetWindowPosition()
        {
            Window window = Application.Current.MainWindow;
            return new Point(window.Left, window.Top);
        }

        /// <summary>
        /// 마우스 좌표 더하기 용
        /// </summary>
        /// <returns></returns>
        public static Point GetCurrentMousePosition()
        {
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            return Application.Current.MainWindow.PointToScreen(mousePosition);
        }

        /// <summary>
        /// WPF 좌표계로 변환
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point GetWpfPoint(System.Drawing.Point point)
        {
            var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (presentationSource != null)
            {
                // WinForms 좌표를 WPF 좌표로 변환합니다.
                var transform = presentationSource.CompositionTarget.TransformFromDevice;
                var mousePoint = transform.Transform(new Point(point.X, point.Y));
                return mousePoint;
            }
            return new Point(0, 0);
        }

        // 마우스 커서 포인트 반환
        public static Point GetMouseCursorCenterPoint()
        {
            var mousePosition = System.Windows.Forms.Cursor.Position;
            return GetWpfPoint(mousePosition);
        }

        /// <summary>
        /// 윈도우 창 위치 값 설정
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public static void SetWindowPosition(double left, double top)
        {
            Window window = Application.Current.MainWindow;
            window.Left = left;
            window.Top = top;   
        }
        public static void SetWindowPosition(Point position)
        {
            //Debug.WriteLine($"""윈도우 변환 {position.X},{position.Y}""");
            Window window = Application.Current.MainWindow;
            window.Left = position.X;
            window.Top = position.Y;
        }
        public static void AddWindowPosition(Point position)
        {
            Window window = Application.Current.MainWindow;
            window.Left += position.X;
            window.Top += position.Y;
        }


        /// <summary>
        /// 마우스 커서 설정
        /// </summary>
        /// <param name="cursorPath"></param>
        public static void SetCursor(Cursor customCursor)
        {
            Application.Current.MainWindow.Cursor = customCursor;
        }
        /// <summary>
        /// 마우스 커서 초기화
        /// </summary>
        public static void SetCursor()
        {
            if(Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Cursor = Cursors.Arrow;
            }
        }
        /// <summary>
        /// 물 주기 커서로 변경
        /// </summary>
        public static void SetWaterCursor()
        {
            SetCursor(waterCursor.cursor);
        }

        /// <summary>
        /// 랜덤 Int 반환
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// 확률에 대한 랜덤 bool
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        public static bool GetRandomBool(double probability)
        {
            return random.NextDouble() < probability;
        }
    }
}
