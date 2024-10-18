using DukpalKang;
using RasingDeokPal.Character.Item;
using RasingDeokPal.Common.Save;
using RasingDeokPal.Components;
using RasingDeokPal.effect;
using RasingDeokPal.View;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Common
{
    internal class GameManager
    {
        // 싱글턴
        private static GameManager instance = null;
        private static readonly object instanceLock = new object();
#if DEBUG
        private bool background = true;
#else
        private bool background = false;
#endif

        private int currentCountStarDustMusic = 0;
        private int currentCountStarDustTouch = 0;

        private readonly int starDustMusicMax = GameConfig.GetConfig().StarDustMusicMax;
        private readonly int starDustTouchMax = GameConfig.GetConfig().StarDustTouchMax;
        private readonly double starDustMusicProbability = GameConfig.GetConfig().StarDustMusicProbability;
        private readonly double starDustTouchProbability = GameConfig.GetConfig().StarDustTouchProbability;
        private readonly int starDustAddValue = GameConfig.GetConfig().StarDustAddValue;


        // 메인 윈도우
        MainWindow mainWindow;
        UIView? mainUIView;     // 메인 UI 영역 / 뽑기 View 포함
        GameView gameView;      // 게임 UI 영역
        SoilChangeView soilChangeView;  // 장비 변경 View

        public bool isStartPack = false;

        // 메인 Cavas
        Canvas gameCanvas;
        Canvas uiCanvas;
        Canvas btnCanvas;       // 플레이어 메뉴, 장비 창 캔버스
        Canvas bubbleCanvas;    // 채팅 캔버스
        
        public PlayerData playerData { get; set; }
        public SaveData saveData { get; set; }

        public static GameManager Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new GameManager((DukpalKang.MainWindow)Application.Current.MainWindow);
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// 게임 진행 관리 매니저 클래스
        /// </summary>
        public GameManager(MainWindow mainWindow) 
        {
            this.mainWindow = mainWindow;

            // 캔버스 설정
            this.gameCanvas = (Canvas)mainWindow.FindName("GameViewCanvas");
            this.bubbleCanvas = (Canvas)mainWindow.FindName("BubbleCanvas");
            this.btnCanvas  = (Canvas)mainWindow.FindName("ContainerBtn");
            this.uiCanvas = (Canvas)mainWindow.FindName("MainUICanvas");
            

            instance = this;
        }

        /// <summary>
        /// 게임 프로그램 시작
        /// </summary>
        public void ProgramStart()
        {
            // 플레이어 데이터가 없는 경우
            if (!SaveData.IsExistPlayData() || !SaveData.IsExistSaveData())
            {                
                GoMain();
            }
            else
            {
                LoadSaveData();
                GameStart();
            }
        }

        /// <summary>
        /// 메인 화면 이동
        /// </summary>
        public void GoMain()
        {
            isStartPack = true;
            this.mainUIView = new TitleView(uiCanvas);
        }

        /// <summary>
        /// 플레이어 저장 데이터 로드
        /// </summary>
        public void LoadSaveData()
        {
            playerData = PlayerData.Load();
            saveData = SaveData.Load();
            SaveData.SavePlayerData(playerData);
        }

        /// <summary>
        /// 픽업 화면 이동
        /// </summary>
        public void GoPickUp()
        {
            CloseEquipUI();
            CloseGameView();

            this.mainUIView = new PickUpView(uiCanvas);
            uiCanvas.IsHitTestVisible = true;
            this.mainUIView.SetVisible(true);
        }

        /// <summary>
        /// 기본 스타터 팩 픽업 진행
        /// </summary>
        /// <param name="character"></param>
        public void GetStartPickUp(GameItem character)
        {
            // 아이템 장비
            playerData.SetEquip(character);
            playerData.SetEquip(GameItem.GetItem(ItemCode.NormalSoil));
            playerData.SetEquip(GameItem.GetItem(ItemCode.NormalPot));

            // 세이브 데이터 저장
            saveData.SetCharater((ItemCharacter)character);
            SaveData.SavePlayerData(playerData);
        }

        /// <summary>
        /// 픽업 진행
        /// </summary>
        public void GetPickUP(GameItem pickUpItem)
        {
            // 아이템 인벤토리에 추가
            playerData.AddItem(pickUpItem);
           // 세이브 데이터 저장
           SaveData.SavePlayerData(playerData);
        }

        /// <summary>
        /// 별가루 획득
        /// </summary>
        public void AddStarDust()
        {
            EffectLayer.Instance.EffectStarDust();
            SoundManager.Instance.PlayCoin();

            // 별가루 10 획득
            this.playerData.AddStardust(starDustAddValue);
            Debug.WriteLine($"""[별가루 {starDustAddValue} 획득] {this.playerData.GetStardust()}""");

            if(this.gameView != null)
            {
                this.gameView.potMenu.SetStarDustText();
            }

            // 저장
            SaveData.SavePlayerData(this.playerData);
        }
        /// <summary>
        /// 음악듣기를 통한 별가루 획득
        /// </summary>
        public void GetStarDustFromListenMusic()
        {
            currentCountStarDustMusic++;
            if(currentCountStarDustMusic >= starDustMusicMax)
            {
                currentCountStarDustMusic = 0;
                // 일정 확률을 통해서 별가루 획득
                if(WindowControlMethod.GetRandomBool(starDustMusicProbability))
                {
                    AddStarDust();
                }
            }
        }

        /// <summary>
        /// 게임 오브젝트 생성
        /// </summary>
        public void GameStart()
        {
            // 픽업 뷰라면 게임 화면으로 이동
            if(mainUIView != null && mainUIView is PickUpView)
            {
                mainUIView.RemoveSelf();   
                mainUIView.SetVisible(false);
                mainUIView = null;
            }

            // 게임 화면
            if(gameView == null)
            {
                gameView = new GameView(gameCanvas, bubbleCanvas);
            }
            else
            {
                // 존재하는 경우 
                gameView.SetVisible(true);
                
            }
        }

        /// <summary>
        /// 장비 UI 모두 닫기
        /// </summary>
        public void CloseEquipUI()
        {
            if(soilChangeView != null)
            {
                soilChangeView.Hide();
            }
        }

        /// <summary>
        /// 게임뷰 출력 제한
        /// </summary>
        public void CloseGameView()
        {
            if(gameView != null)
            {
                gameView.RemoveSelf();
                gameView.SetVisible(false);
            }
        }

        /// <summary>
        /// 흙 변경 화면 이동
        /// </summary>
        public void GoEquipChange()
        {
            if (soilChangeView == null)
            {
                soilChangeView = new SoilChangeView(btnCanvas);
            }
            else
            {
                soilChangeView.SetVisible(true);
                soilChangeView.Toggle();
            }
        }

        /// <summary>
        /// 메인 뷰 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetMainView<T>() where T : UIView
        {
            if(mainUIView != null)
            {
                return (T)mainUIView;
            }
            return null;
        }
        public GameView GetGameView()
        {
            return this.gameView;
        }
        public SoilChangeView GetEquipView()
        {
            return this.soilChangeView;
        }

        /// <summary>
        /// 아이템 변경
        /// </summary>
        /// <param name="item"></param>
        public void ChangeItem(GameItem item)
        {
            Debug.WriteLine($"""{item.ItemName} 장착""");
            playerData.SetEquip(item);
            GetEquipView().Update();


            // 플레이어 반영
            gameView.player.RenderPotUnit();
            // 데이터 저장
            SaveData.SavePlayerData(playerData);
        }

        /// <summary>
        /// 배경 토글
        /// </summary>
        internal void SetBackGround()
        {
            if (this.background)
            {
                WindowControlMethod.SetWindowBackground(new SolidColorBrush(Colors.LightBlue));
            }
            else
            {
                WindowControlMethod.SetWindowBackground();
            }
        }
        public void ToggleBackGround()
        {
            this.background = !this.background;
            SetBackGround();
        }
    }
}
