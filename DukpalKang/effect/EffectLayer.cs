using RasingDeokPal.Character.Unit;
using RasingDeokPal.Common;
using RasingDeokPal.effect.Unit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RasingDeokPal.effect
{
    internal class EffectLayer
    {
        private static EffectLayer instance = null;
        private static readonly object instanceLock = new object();
        private Canvas effectCanvas;

        private int starDustLeft = (GameConfig.Instance.WindowWidth / 2) - 25;
        private int starDustTop = (GameConfig.Instance.WindowHeight / 2) - 150;

        // 노트 이펙트
        private DispatcherTimer? effectNoteTimer;
        private Point noteCreatePoint;

        private EffectLayer()
        {

        }

        public static EffectLayer Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new EffectLayer();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// 캔버스 설정
        /// </summary>
        /// <param name="canvas"></param>
        public void SetEffectCanvas(Canvas canvas)
        {
            this.effectCanvas = canvas;
        }

        /// <summary>
        /// 반짝이 레이어 효과
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="frameMs"></param>
        public void EffectTwinkle(int left, int top, int frameMs=75)
        {
            SpriteUnit star = new SpriteUnit(effectCanvas, "pack://application:,,,/asset/effect/sprite_twinkle.png", 300, 300, 13);
            star.SetAnimationSpeed(frameMs);
            star.SetUIPosition(left, top);
            star.SetHitBox(false);
            star.SetAnimationStopHandler(star.RemoveSelf);
            star.AnimationStart();
        }

        /// <summary>
        /// 별가루 획득
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="frameMs"></param>
        public void EffectStarDust(int left, int top, int frameMs = 90)
        {
            SpriteUnit starDust = new SpriteUnit(effectCanvas, "pack://application:,,,/asset/effect/effct_stardust.png", 300, 300, 7,1, 13);
            starDust.SetAnimationSpeed(frameMs);
            starDust.SetUIPosition(left, top);
            starDust.SetUIElementSize(50, 50);
            starDust.SetHitBox(false);
            starDust.SetAnimationStopHandler(starDust.RemoveSelf);
            starDust.AnimationStart();
        }

        public void EffectStarDust(int frameMs = 90)
        {
            EffectStarDust(starDustLeft, starDustTop, frameMs);
        }

        /// <summary>
        /// 음표 이펙트
        /// </summary>
        public void EffectNoteOn(double left, double top)
        {   
            if(effectNoteTimer == null)
            {
                noteCreatePoint = new Point(left, top);
                effectNoteTimer = new DispatcherTimer();
                effectNoteTimer.Interval = TimeSpan.FromSeconds(3); // 타이머 간격
                effectNoteTimer.Tick += EffectNoteHandler;
                effectNoteTimer.Start();

                // 노트 2개 생성하고 시작
                CreateNote();
            }
        }
        private void EffectNoteHandler(object? sender, EventArgs e) 
        {
            // 노트 2~3개씩 생성
            for(int i = 0; i< WindowControlMethod.GetRandomInt(1,4); i++)
            {
                CreateNote();
            }
        }

        private void CreateNote()
        {
            int createLeft = WindowControlMethod.GetRandomInt((int)(noteCreatePoint.X - 80), (int)(noteCreatePoint.X + 70));
            int createTop = WindowControlMethod.GetRandomInt((int)(noteCreatePoint.Y), (int)(noteCreatePoint.Y + 25));
            UnitNote note = new UnitNote(effectCanvas, createLeft, createTop);
        }

        public void EffectNoteOff()
        {
            if(effectNoteTimer != null)
            {
                // 이펙트 끄기
                effectNoteTimer.Stop();
                effectNoteTimer = null;
            }
        }
    }
}
