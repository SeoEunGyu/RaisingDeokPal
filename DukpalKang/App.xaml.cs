using System.CodeDom;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DukpalKang
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex;
        private string mutexName = "DeokPalKang";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bool createNew;

            mutex = new Mutex(true, mutexName, out createNew);
            if (!createNew) 
            {
                // 이미 실행된 창이 있음
                Shutdown();
            }
        }

    }

}
