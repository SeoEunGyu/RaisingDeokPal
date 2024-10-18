using DukpalKang;
using System.Windows;
using System.Windows.Media;

namespace RasingDeokPal.Common
{
    internal class SoundManager
    {
        private MediaPlayer media;
        private static SoundManager instance = null;
        private static readonly object instanceLock = new object();

        public SoundManager(MainWindow mainWindow)
        {
            this.media = new MediaPlayer();
        }

        public static SoundManager Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SoundManager((DukpalKang.MainWindow)Application.Current.MainWindow);
                    }
                    return instance;
                }
            }
        }
        /// <summary>
        /// 비프음 출력
        /// </summary>
        public void PlayBeep()
        {
            if (this.media != null) 
            {
                string mp3FilePath = "./asset/Sound/sound_8bit_select.mp3";
                this.media.Volume = 0.02;
                this.media.Open(new Uri(mp3FilePath, UriKind.Relative));
                this.media.Play();
            }
        }

        /// <summary>
        /// 코인 획득
        /// </summary>
        public void PlayCoin()
        {
            if (this.media != null)
            {
                string mp3FilePath = "./asset/Sound/sound_coin_collection.mp3";
                this.media.Volume = 0.03;
                this.media.Open(new Uri(mp3FilePath, UriKind.Relative));
                this.media.Play();
            }
        }
    }
}
