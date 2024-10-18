using RasingDeokPal.Common;
using static RasingDeokPal.Common.Animations;
using System.Windows.Media;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using RasingDeokPal.Character.Unit;
using System.Windows.Threading;
using static RasingDeokPal.Common.Enums.CommonEnum;
using System.Windows.Input;
using RasingDeokPal.Character.Item;
using RasingDeokPal.Common.Save;

namespace RasingDeokPal.Character
{
    /// <summary>
    /// 덕팔이 클래스
    /// </summary>
    internal class DeokPal : MainUnit
    {
        private const string bitmapPhase1 = "pack://application:,,,/asset/deokpal/deokpal_form_1.png";
        private const string bitmapPhase2 = "pack://application:,,,/asset/deokpal/deokpal_form_2.png";
        private const string bitmapPhase3 = "pack://application:,,,/asset/deokpal/deokpal_form_3.png";

        bool isDrag;
        bool isFirstMouseMove;

        // 덕팔이 물 가지고 받는 데미지 기준
        private int LifeIntervalSecond  = GameConfig.GetConfig().LifeIntervalSecond;        // 타임 체크
        private int pivotDamageWaterMin = GameConfig.GetConfig().DamagePivotWaterMin;
        private int pivotDamageWaterMax = GameConfig.GetConfig().DamagePivotWaterMax;


        //public SubUnit unitShadow;        // 그림자 유닛
        public Soil unitSoil;               // 흙 유닛
        public PlantPot unitPot;            // 화분 유닛
        public SubUnit? unitSunShine;       // 햇살 유닛
        public NoPowerSun? unitNoPowerSun;  // 힘이없썬 유닛
        public Nutrient? unitNutrient;      // 영양제 유닛
        public SubUnit unitDieFrame;        // 영정사진

        // 덕팔이 인생 타이머
        DispatcherTimer lifeTimer;
        Stopwatch playTimeWatch;
        
        // 저장 데이터
        SaveData saveData;
        PlayerData playerData;

        int marginLeft;
        int marginTop;

        /// <summary>
        /// 덕팔이 생성자
        /// </summary>
        /// <param name="target"></param>
        /// <param name="imgUri"></param>
        public DeokPal(Canvas canvas, string imgUri) : base(canvas, imgUri)
        {

            // 덕팔이 데이터 로드
            saveData = GameManager.Instance.saveData;
            playerData = GameManager.Instance.playerData;

            // 이미지 UI 크기 지정
            SetTargetUIElementSize((int)width, (int)height);
            Point winSize = WindowControlMethod.GetWindowSize();
            int scaleMargin = (int)((1.0 - saveData.Scale) * 30);

            marginLeft = (int)((winSize.X / 2)-(width / 2));
            marginTop = (int)((winSize.Y / 2) - (height / 2));
            
            // 이미지 위치 중심 지정
            UIControl.SetCanvasMargin(this.targetUI, marginLeft, marginTop + scaleMargin);

            // 그림자 유닛
            //unitShadow = new SubUnit(targetCanvas, "pack://application:,,,/asset/deokpal/shadow.png");
            //int shadowMarginTop = marginTop + unitShadow.bitmapHeight + shadowMarginTopValue;
            //unitShadow.SetUIMargin(marginLeft, shadowMarginTop);

            // 흙 유닛
            unitSoil = new Soil(this, targetCanvas, "pack://application:,,,/asset/deokpal/soil.png", saveData.Water, marginLeft, marginTop, 9);

            // 화분 유닛
            RenderPotUnit();
            
            // 영정 사진 유닛
            unitDieFrame = new SubUnit(targetCanvas, "pack://application:,,,/asset/effect/die_frame.png", 12);
            int frameWidth = (GameConfig.GetConfig().WindowWidth / 2);
            int frameHeight = (GameConfig.GetConfig().WindowHeight / 2);
            int framLeft = frameWidth - (frameWidth / 2);
            int framTop = frameHeight - (frameHeight / 2);

            unitDieFrame.SetUIElementSize(frameWidth, frameHeight);
            unitDieFrame.SetUIMargin(framLeft, framTop);
            unitDieFrame.SetVisible(false);

            // 본체 이벤트 지정
            AddHandler(Image.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftDown));
            AddHandler(Image.MouseLeftButtonUpEvent, new MouseButtonEventHandler(MouseLeftUp));
            AddHandler(Image.MouseRightButtonDownEvent, new MouseButtonEventHandler(MouseRightDown));
            AddHandler(Image.MouseMoveEvent, new MouseEventHandler(MouseMove));
            AddHandler(Image.MouseEnterEvent, new MouseEventHandler(MouseEnter));
            AddHandler(Image.MouseLeaveEvent, new MouseEventHandler(MouseLeave));



            // 덕팔이 상태에 따른 이미지 설정
            SetDeokPalImageOnStatus();
            SetDeokPalImageScale();
            SetDeokPalImageColor();

            // 덕팔이 라이프 타이머 실행
            SetLifeTimer();
            // 플레이 타임 체크
            playTimeWatch = new Stopwatch();
            playTimeWatch.Start();


            // 저장된 플레이 타임 출력
            TimeSpan savedPlayTimeSpan = TimeSpan.FromMilliseconds(saveData.PlayTime);
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                              savedPlayTimeSpan.Hours,
                                              savedPlayTimeSpan.Minutes,
                                              savedPlayTimeSpan.Seconds);
            Debug.WriteLine(
               $"""
                   [덕팔이 데이터 로드]
                   Lv       : {saveData.Lv}
                   HP       : {saveData.Hp}
                   Exp      : {saveData.Exp}
                   Scale    : {saveData.Scale}
                   PlayTime : {formattedTime}
                   Color    : {saveData.color[0]},{saveData.color[1]},{saveData.color[2]}
                 """
           );
           Save();
        }

        /// <summary>
        /// 화분 렌더링
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        internal void RenderPotUnit()
        {
            ItemPot item = (ItemPot)GameItem.GetItem(playerData.GetEquipData()[2]);
            
            if (unitPot != null)
            {
                unitPot.SetItem(item);
            }
            else
            {    
                unitPot = new PlantPot(this, targetCanvas, item, marginLeft, marginTop, 8);
            }            
        }

        /// <summary>
        /// 덕팔이 상태에 따른 이미지 설정
        /// </summary>
        private void SetDeokPalImageOnStatus()
        {
            if(saveData.Life.Equals(DeokPalStatus.Hurt) || saveData.Life.Equals(DeokPalStatus.Die))
            {
                // 죽음 이미지
                SetImageSource(targetUI, "pack://application:,,,/asset/deokpal/deokpal_die.png");
            }
            else
            {
                if(saveData.Water <= GameConfig.GetConfig().ThirstyPivot || saveData.SunShine <= 0)
                {
                    // 목마름 이미지
                    SetImageSource(targetUI, "pack://application:,,,/asset/deokpal/deokpal_thirsty.png");
                }
                else
                {
                    // 기본 값
                    if(saveData.Lv < (int)PlayerGrowPhase.Phase1)
                    {
                        SetImageSource(targetUI, "pack://application:,,,/asset/deokpal/deokpal_default.png");
                    }
                    else if(saveData.Lv < (int)PlayerGrowPhase.Phase2)
                    {
                        SetImageSource(targetUI, "pack://application:,,,/asset/deokpal/deokpal_form_1.png");
                    }
                    else if (saveData.Lv < (int)PlayerGrowPhase.Phase3)
                    {
                        SetImageSource(targetUI, "pack://application:,,,/asset/deokpal/deokpal_form_2.png");
                    }
                    else
                    {
                        SetImageSource(targetUI, "pack://application:,,,/asset/deokpal/deokpal_form_3.png");
                    }
                }
            }
        }
        /// <summary>
        /// 덕팔이 이미지 크기 갱신
        /// </summary>
        private void SetDeokPalImageScale()
        {
            // 크기 조정
            this.scaleTransform.ScaleX = saveData.Scale;
            this.scaleTransform.ScaleY = saveData.Scale;
        }
        /// <summary>
        /// 덕팔이 이미지 색상 갱신
        /// </summary>
        private void SetDeokPalImageColor()
        {
            // 저장된 색상으로 덕팔이 변경
            UIControl.SetImageColor(targetUI, GameConfig.GetConfig().StartDefaultColor, saveData.color);
        }

        /// <summary>
        /// 트랜스폼 설정
        /// </summary>
        /// <remarks>
        /// 덕팔이 이미지 조정을 위해서 오버라이딩
        /// </remarks>
        protected void SetTransfromGroup()
        {
            TransformGroup transformGroup = new TransformGroup();
            this.rotateTransform = new RotateTransform { CenterX = 0, CenterY = 10 };
            this.translateTransform = new TranslateTransform();
            this.scaleTransform = new ScaleTransform(saveData.Scale, saveData.Scale);

            transformGroup.Children.Add(rotateTransform);
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(scaleTransform);

            this.targetUI.RenderTransform = transformGroup;
            // 중심 좌표 지정
            this.centerPoint = new Point(rotateTransform.CenterX, rotateTransform.CenterY);
        }

        /// <summary>
        /// 덕팔이 상태 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckStatus(object? sender, EventArgs e) 
        {
            // 물이 너무 많으면 데미지를 입음
            if(saveData.Water > pivotDamageWaterMax)
            {
                Debug.WriteLine($"""덕팔이가 물을 너무 많이 먹고 있습니다. {saveData.Water}""");
                GetDamage(GameConfig.GetConfig().DamageWaterValue);
            }
            else if(saveData.Water < pivotDamageWaterMin)
            {
                Debug.WriteLine($"""덕팔이가 너무 건조합니다.{saveData.Water}""");
                GetDamage(GameConfig.GetConfig().DamageWaterValue);
            }
            else if(saveData.Nutrition <= 0)
            {
                Debug.WriteLine($"""덕팔이 영양소가 부족합니다.{saveData.Nutrition}""");
            }
            else
            {
                // 기쁨의 상호작용
                DoSomething();
                // 경험치 업
                ExpUp();
                Debug.WriteLine($"""덕팔이 쾌적합니다. {saveData.Water}""");
            }

            // 물 수치 갱신
            UpdateWaterDrainage();
            // 햇살 수치 갱신
            UpdateSunShine();
            // 영양 수치 갱신
            UpdateNutrient();

            // 햇살 부족인 경우 덕팔이 크기 조정
            if(saveData.SunShine <= 0)
            {
                // 죽는 기준
                double pivotDieBySunshine = saveData.StartScale - GameConfig.GetConfig().ScaleDownMin;
                saveData.Scale -= GameConfig.GetConfig().SunShineInsufficientDownScale;
                // 데미지 받음
                GetDamage(GameConfig.GetConfig().DamageSunShineLessValue);

                Debug.WriteLine($"""광합성 부족으로 쪼그라듭니다. [현재 크기] {saveData.Scale.ToString("F3")} [HP] {saveData.Hp}""");
                if(saveData.Scale <= pivotDieBySunshine)
                {
                    saveData.Scale = pivotDieBySunshine;
                    // 죽음
                    DeokPalDie();
                }
            }

            Debug.WriteLine($"""[HP] {saveData.Hp}""");
            Debug.WriteLine($"""[Lv] {saveData.Lv} [경험치] {saveData.Exp} {Environment.NewLine}""");
            
            // 일반 상태에서 체력이 깎여 아픈 상태로 돌입
            if (saveData.Life.Equals(DeokPalStatus.Live) && saveData.Hp <= GameConfig.GetConfig().DyingPivotHp)
            {
                saveData.Life = DeokPalStatus.Hurt;
            }
            // 아픈 상태에서 체력이 찬 경우 원래대로
            if(saveData.Life.Equals(DeokPalStatus.Hurt) && saveData.Hp > GameConfig.GetConfig().DyingPivotHp)
            {
                saveData.Life = DeokPalStatus.Live;
            }


            // 게임 저장
            Save();

            // 덕팔이 UI 갱신
            SetDeokPalImageOnStatus();
            SetDeokPalImageScale();
            SetDeokPalImageColor();
        }

        /// <summary>
        /// 덕팔이 라이프 타이머 실행
        /// </summary>
        private void SetLifeTimer()
        {
            lifeTimer = new DispatcherTimer();
            lifeTimer.Interval = TimeSpan.FromSeconds(LifeIntervalSecond);
            lifeTimer.Tick += CheckStatus;
            lifeTimer.Start();
        }

        internal void Pause()
        {
            if(lifeTimer != null)
            {
                lifeTimer.Stop();
            }
        }

        internal void Resume()
        {
            if (lifeTimer != null)
            {
                lifeTimer.Stop();
            }
            else
            {
                SetLifeTimer();
            }
        }


        /// <summary>
        /// [상태 갱신] 물 수치 갱신
        /// </summary>
        /// <param name="value"></param>
        private void UpdateWaterDrainage()
        {
            int waterDounValue = GameConfig.Instance.WaterDownValue;

            // 햇빛 켜져 있는 경우 물 수치 추가 감소
            if (unitNoPowerSun != null)
            {
                Debug.WriteLine($"""햇빛으로 인해 추가 감소합니다.""");
                waterDounValue *= 2;
            }


            saveData.Water -= waterDounValue;
            if(saveData.Water < 0)
            {
                saveData.Water = 0;
            }
            //흙 유닛에서 색상 변경
            // 물 상태 체크
            // statusWater:100 = soilColor:255
            
            unitSoil.DoWaterDrainage(saveData.Water);
            Debug.WriteLine($"""[물] {saveData.Water}/100""");
        }

        /// <summary>
        /// [상태 갱신] 햇빛 수치 갱신
        /// </summary>
        private void UpdateSunShine()
        {
            if (unitNoPowerSun != null)
            {
                // 햇빛이 켜져 있으면
                int sunShineUpValue = GameConfig.GetConfig().SunShineUpValue;
                saveData.SunShine += sunShineUpValue;
                if (saveData.SunShine > 100)
                {
                    saveData.SunShine = 100;
                }
            }
            else
            {
                int sunShineDounValue = GameConfig.GetConfig().SunShineDownValue;
                saveData.SunShine -= sunShineDounValue;
                if (saveData.SunShine < 0)
                {
                    saveData.SunShine = 0;
                }
            }

            // 햇빛 효과
            Debug.WriteLine($"""[햇빛]  {saveData.SunShine}/100""");
        }

        /// <summary>
        /// [상태 갱신] 영상 수치 갱신
        /// </summary>
        private void UpdateNutrient()
        {
            // 영양제 오브젝트 존재시 
            if(unitNutrient != null)
            {
                saveData.Nutrition += GameConfig.GetConfig().NutrientValue; 
            }

            if (saveData.Nutrition > 100)
            {
                saveData.Nutrition = 100;
                GetDamage(GameConfig.GetConfig().NutrientDamageValue);
            }
            else
            {
                if(!saveData.Life.Equals(DeokPalStatus.Die) && saveData.Hp <= 100 && saveData.Nutrition > 0)
                {
                    // 죽지 않은 경우
                    Heal(5);
                    DownNutrition();
                }
            }
            Debug.WriteLine($"""[영양소]  {saveData.Nutrition}/100""");
        }

        /// <summary>
        /// [상태 갱신] 영양 감소
        /// </summary>
        private void DownNutrition(int value = 1)
        {
            // 영양 감소
            saveData.Nutrition -= value;
            if(saveData.Nutrition < 0)
            {
                saveData.Nutrition = 0;
            }
        }

        /// <summary>
        /// [상태 갱신] 체력 회복
        /// </summary>
        private void Heal(int value)
        {
            saveData.Hp += value;
            if( saveData.Hp > 100)
            {
                saveData.Hp = 100;
            }
        }

        /// <summary>
        /// [상태 갱신] 물주기 
        /// </summary>
        public void DoWater()
        {
            // 물 가득 차있는데 더 부으면 데미지
            if(saveData.Water >= 100) 
            {
                Debug.WriteLine($"""덕팔이 화분에 물이 흘러 넘칩니다. HP:{saveData.Hp}""");
                GetDamage(GameConfig.GetConfig().DamageWaterValue);
            }

            saveData.Water += 20;
            if(saveData.Water > 100) 
            {
                saveData.Water = 100;
            }

            unitSoil.DoWater(saveData.Water);
        }

        /// <summary>
        /// 햇빛 토글
        /// </summary>
        public void ToggleSunshine()
        {
            if(unitNoPowerSun != null)
            {
                unitNoPowerSun.RemoveSelf();
                unitNoPowerSun = null;
            }
            else
            {
                // 힘이없썬
                unitNoPowerSun = new NoPowerSun(targetCanvas, "pack://application:,,,/asset/effect/noPowerSun.png", 11);
            }   
        }

        /// <summary>
        /// 영양제 토글
        /// </summary>
        public void ToggleNutrient()
        {
            if(unitNutrient != null)
            {
                unitNutrient.RemoveSelf();
                unitNutrient = null;
            }
            else
            {
                unitNutrient = new Nutrient(targetCanvas, 11);
            }
        }


        /// <summary>
        /// 경험치 상승
        /// </summary>
        private void ExpUp()
        {
            saveData.Exp += GameConfig.GetConfig().ExpUpValue;
            if(saveData.Exp >= GameConfig.GetConfig().ExpMax)
            {
                LvUp();
            }
        }

        /// <summary>
        /// 레벨 업
        /// </summary>
        private void LvUp()
        {
            // 성장 레벨 구간인 경우
            if(saveData.Lv < (int)PlayerGrowPhase.Phase1)
            {
                saveData.Scale += GameConfig.GetConfig().GrowScale;
            }

            // 레벨 업 경험치 초기화
            saveData.Lv += 1;
            saveData.Exp = 0;
            // 이미지 스케일 조정
            scaleTransform.ScaleX = saveData.Scale;
            scaleTransform.ScaleY = saveData.Scale;

            Debug.WriteLine($"""[레벨 업!] {saveData.Lv}""");


            DoSomething();
            SetDeokPalImageOnStatus();
        }

        /// <summary>
        /// 물, 영양 등 다양한 요소로 피해를 받는 경우 발동
        /// </summary>
        private void GetDamage(int damage)
        {
            saveData.Hp -= damage;
            DownNutrition();

            if(saveData.Hp < 0)
            {
                // 죽음
                DeokPalDie();
            }
        }

        /// <summary>
        /// 죽음 함수
        /// </summary>
        private void DeokPalDie()
        {
            // 사망선고
            saveData.Life = DeokPalStatus.Die;

            // 그레이 스케일 적용
            UIControl.SetGrayScaleShader();        
            
            // 타이머 정지
            lifeTimer.Stop();
            playTimeWatch.Stop();

            // 비활성화
            targetUI.IsEnabled = false;
            unitSoil.GetImage().IsEnabled = false;
            unitPot.GetImage().IsEnabled = false;

            // 영정 사진 출력
            unitDieFrame.SetVisible(true);
            
            // 장비 메뉴 모두 닫기
            GameManager.Instance.CloseEquipUI();
            // 플레이어 메뉴, 화분 메뉴 닫기
            GameManager.Instance.GetGameView().HideAllMenu();

            // 사망 데이터 저장
            SaveData.DieSave();
        }

        /// <summary>
        /// 데이터 세이브
        /// </summary>
        public void Save()
        {
            if (saveData.Life.Equals(DeokPalStatus.Live) || saveData.Life.Equals(DeokPalStatus.Hurt))
            {
                // 게임 저장
                saveData.saveDate = DateTime.Now;
                saveData.PlayTime += playTimeWatch.Elapsed.TotalMilliseconds;
                // 저장
                SaveData.Save(saveData);
                playTimeWatch.Restart();
            }
        }

  


        /// <summary>
        /// 뽈롱 모드 실행
        /// </summary>
        public void DoBreathAction()
        {
            int actionNumber = GetRandomNumber(0, 2);
            if(actionNumber > 0)
            {
                // 가로 숨쉬기
                DoBreathHorizon(100);
            }
            else
            {
                // 세로 숨쉬기
                DoBreathVertical(100);
            }
        }

        /// <summary>
        /// 방치 모드 실행
        /// </summary>
        public void DoLivingYourLife()
        {
            // 타이머 초기화
            ClearTimer();

            // 애니메이션 쿨타임 설정
            int coolTime = this.GetRandomNumber(3, 7);

            // 타이머 설정
            SetTimer(coolTime, handler: DeokPalLifeHandler);
        }

        /// <summary>
        /// 방치 모드 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeokPalLifeHandler(object? sender, EventArgs e)
        {
            // 애니메이션 재생 시간
            int animationDuration = this.GetRandomNumber(800, 1200);
            // 애니메이션 번호 뽑기
            int animationNumber = this.GetRandomNumber(0, 3);
#if DEBUG
            Debug.WriteLine($"""[{animationNumber}] 덕팔이의 숨쉬기 [{animationDuration}] 초 """);
            //DoRolling(animationDuration);
            //return;
#endif

            // 애니메이션 선택
            switch (animationNumber)
            {
                case 0:
                    //DoRolling(animationDuration);
                    DoShake(animationDuration);
                    break;
                case 1:
                    DoBreathVertical(animationDuration, 1.1, 0.95);
                    break;
                case 2:
                    DoBreathHorizon(animationDuration, 0.95);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 랜덤 애니메이션 재생
        /// </summary>
        public void DoSomething()
        {
            // 애니메이션 재생 시간
            int animationDuration = this.GetRandomNumber(800, 1200);
            // 애니메이션 번호 뽑기
            int animationNumber = this.GetRandomNumber(0, 3);

            // 애니메이션 선택
            switch (animationNumber)
            {
                case 0:
                    //DoRolling(animationDuration);
                    DoShake(animationDuration);
                    break;
                case 1:
                    DoBreathVertical(animationDuration, 1.1, 0.95);
                    break;
                case 2:
                    DoBreathHorizon(animationDuration, 0.95);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 애니메이션 가로 숨쉬기
        /// </summary>
        /// <param name="time"></param>
        /// <param name="power"></param>
        public void DoBreathHorizon(int time, double power = 0.5)
        {
            // 단일 애니메이션 2개 실행
            if (this.scaleTransform != null)
            {
                SingleAnimation<ScaleTransform> ScaleXAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleXProperty, saveData.Scale, saveData.Scale * power, time, true);
                SingleAnimation<ScaleTransform> ScaleYAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleYProperty, saveData.Scale, saveData.Scale * 1.1, time, true);
                ScaleXAnimation.Play();
                ScaleYAnimation.Play();
            }
        }

        /// <summary>
        /// 애니메이션 세로 숨쉬기
        /// </summary>
        /// <param name="time"></param>
        /// <param name="powerStart"></param>
        /// <param name="powerEnd"></param>
        public void DoBreathVertical(int time, double powerStart = 1.3, double powerEnd = 1.3)
        {
            if (this.scaleTransform != null)
            {
                SingleAnimation<ScaleTransform> ScaleXAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleXProperty, saveData.Scale, saveData.Scale * powerStart, time, true);
                SingleAnimation<ScaleTransform> ScaleYAnimation = new SingleAnimation<ScaleTransform>(scaleTransform, ScaleTransform.ScaleYProperty, saveData.Scale, saveData.Scale * powerEnd, time, true);
                ScaleXAnimation.Play();
                ScaleYAnimation.Play();
            }
        }
    
        /// <summary>
        /// 애니메이션 구르기
        /// </summary>
        /// <param name="time"></param>
        public void DoRolling(int time)
        {
            PauseTimer();
            // 구르기 방향 지정
            bool toRight = GetRandomNumber(0, 2) == 0;
            // 애니메이션 지속 시간
            int duration = (time * 10);

            //01. 회전 애니메이션 설정 값
            int degree = toRight ? 360 : -360;
            
            //02. 이동 애니메이션 설정 값
            int distance = GetRandomNumber(time / 6, time / 4);

            // 현재 위치
            Point currentPos = GetCurrentScreenPos();
            // 이동 목표 지점 : 현재 위치 + 랜덤 거리
            double moveX = toRight ? currentPos.X + distance : currentPos.X - distance;
            //--------------------------------------------------------------------------------
            // 애니메이션 생성
            DoubleAnimation rotateAnimation = CreateDoubleAnimation(0, degree, duration, RotateAnimeationCompletedHandler);
            DoubleAnimation rollingAnimation = CreateDoubleAnimation(currentPos.X, moveX, duration, TranslatateAnimationCompletedHandler);

            // 윈도우 접근
            Window window = Application.Current.MainWindow;

            // 스토리 보드 생성
            CreateAnimationBoard(
                new List<BoardAnimation>
                {
                    new BoardAnimation(rotateAnimation, targetUI,"(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"),
                    new BoardAnimation(rollingAnimation, window, Window.LeftProperty)
                });

            // 스토리 보드 재생
            AnimationPlay();
        }

        /// <summary>
        /// 애니메이션 흔들기
        /// </summary>
        public void DoShake(int duration)
        {
            PauseTimer();
            // 애니메이션 지속 시간
            int degree = GetRandomNumber(-30, 45);
            // 오른쪽 기울기면
            if(degree > 0)
            {
                //무게 중심 오른쪽으로 옮기기
                SetCenterXY(centerPoint.X + 10, centerPoint.Y+10);
            }
            else
            {
                // 왼쪽 기울기면 왼쪽으로 옮기기
                SetCenterXY(centerPoint.X - 10, centerPoint.Y + 10);
            }
            DoubleAnimation rotateAnimation = CreateDoubleAnimation(0, degree, duration, RotateAnimeationCompletedHandler, true);
            CreateAnimationBoard(
                new List<BoardAnimation>
                {
                    new BoardAnimation(rotateAnimation, targetUI,"(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"),
                    
                });

            // 스토리 보드 재생
            AnimationPlay();
        }
        

        /// <summary>
        /// 스토리보드 애니메이션 속성 업데이트
        /// </summary>
        public void UpdateStoryBoardValue()
        {
            if(animationBoard!= null)
            {
                double posX = WindowControlMethod.GetCurrentMousePosition().X;
                animationBoard.GetBoardAnimation(0).UpdateFrom(GetImageAngle(), animationBoard.playTime);
                animationBoard.GetBoardAnimation(1).UpdateFromTo(posX, animationBoard.playTime);
            }
        }

        /// <summary>
        /// 이동 애니메이션 종료 확인 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TranslatateAnimationCompletedHandler(object? sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 회전 애니메이션 종료 확인
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateAnimeationCompletedHandler(object? sender, EventArgs e)
        {
            // 중심축 원상복귀
            SetCenterXY(centerPoint);
            // 애니메이션 공통으로 실행 중, 완료 상태 확인 필요
            this.animationBoard.isRunning = false;
            this.animationBoard.SetIsComplete(true);
            ResumeTimer();
        }
        
        

        /// <summary>
        /// 덕팔이 현재 윈도우 포지션 리턴
        /// </summary>
        /// <returns></returns>
        public Point GetCurrentScreenPos()
        {
            return WindowControlMethod.GetWindowPosition();
        }
        /// <summary>
        /// 덕팔이 현재 이미지 각도 반환
        /// </summary>
        /// <returns></returns>
        public double GetImageAngle()
        {
            return base.GetCurrentAngle();
        }
        
        /// <summary>
        /// 덕팔이 현재 Margin 반환
        /// </summary>
        /// <returns></returns>
        public Point GetCurrentMargin()
        {
            return new Point(Canvas.GetLeft(targetUI),Canvas.GetTop(targetUI));
        }

        /// <summary>
        /// 덕팔이 현재 Actual Width,Height 반환
        /// </summary>
        /// <returns></returns>
        public Point GetCurrentActualSize()
        {
            return new Point(targetUI.Width, targetUI.Height);
        }

        /// <summary>
        /// 애니메이션 정지 및 마우스 무브 상태 변경
        /// </summary>
        /// <remarks>
        /// 화분 드래그 목적 함수
        /// </remarks>
        public void DragStart()
        {
            AnimationPause();
            isFirstMouseMove = true;
        }
        

        //=============== 마우스 조작 ===========================================
        /// <summary>
        /// 마우스 진입
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseEnter(object sender, MouseEventArgs e)
        {
            InteractionMenu selectedMenu = GameManager.Instance.GetGameView().selectedMenu;
            // 물 주기 모드인 경우 커서 변경
            if (selectedMenu.Equals(InteractionMenu.Water))
            {
                WindowControlMethod.SetWaterCursor();
            }
        }
        /// <summary>
        /// 마우스 진입 해제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeave(object sender, MouseEventArgs e)
        {
            WindowControlMethod.SetCursor();
        }
        /// <summary>
        /// 마우스 좌클릭 다운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            InteractionMenu selectedMenu = GameManager.Instance.GetGameView().selectedMenu;
            // 선택한 메뉴에 맞는 동작 실행
            switch (selectedMenu)
            {
                // 1. 이동 모드
                case InteractionMenu.None:
                    // 윈도우 창 드래그
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {

                        Debug.WriteLine("마우스 Down");

                        // 마우스 위치를 기준으로 윈도우 창 지정
                        var mousePos = WindowControlMethod.GetMouseCursorCenterPoint();
                        Point winSize = WindowControlMethod.GetWindowSize();
                        mousePos.X -= winSize.X / 2;
                        mousePos.Y -= winSize.Y / 2;
                        WindowControlMethod.SetWindowPosition(mousePos);

                        // 덕팔이 애니메이션 정지
                        DragStart();
                    }
                    break;
                // 2. 뽈롱 모드
                case InteractionMenu.Touch:
                    DoBreathAction();
                    break;
                // 3. 물주기 모드
                case InteractionMenu.Water:
                    DoWater();
                    break;
                default: break;
            }
            // 메뉴 닫기
            GameManager.Instance.GetGameView().HideAllMenu();
        }
        /// <summary>
        /// 마우스 좌클릭 업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            InteractionMenu selectedMenu = GameManager.Instance.GetGameView().selectedMenu;
            if (selectedMenu.Equals(InteractionMenu.None))
            {
                if (isDrag)
                {
                    var mousePos = WindowControlMethod.GetMouseCursorCenterPoint();

                    Point winSize = WindowControlMethod.GetWindowSize();
                    mousePos.X -= winSize.X / 2;
                    mousePos.Y -= winSize.Y / 2;
                    WindowControlMethod.SetWindowPosition(mousePos);

                    DragStoryBoardUpdate();
                }
            }
        }
        public void DragStoryBoardUpdate()
        {
            if (isDrag)
            {
                // 스토리보드 From To 업데이트
                if (
                animationBoard != null &&
                !animationBoard.isRunning &&
                !animationBoard.isCompleted)
                {
                    // 스토리보드 재생 
                    AnimationPlay();
                }
                isDrag = false;
            }
        }
        /// <summary>
        /// 마우스 우클릭 다운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            GameManager.Instance.CloseEquipUI();
            GameManager.Instance.GetGameView().TogglePlayerMenu();
        }
        /// <summary>
        /// 마우스 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (isFirstMouseMove)
            {
                isFirstMouseMove = false;
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                SafeDragMoveCall(e);
            }
        }
        /// <summary>
        /// 윈도우 창 드래그 Task
        /// </summary>
        /// <param name="e"></param>
        private void SafeDragMoveCall(MouseEventArgs e)
        {
            Task.Delay(10).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke((Action)
                    delegate
                    {
                        if (Mouse.LeftButton == MouseButtonState.Pressed)
                        {
                            Application.Current.MainWindow.DragMove();
                            isDrag = true;
                            MouseButtonEventArgs newEventArgs = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
                            {
                                RoutedEvent = Image.MouseLeftButtonUpEvent
                            };
                            // 마우스 좌클릭 UP 이벤트
                            RaiseEvent(newEventArgs);
                        }
                    });
            });
        }

        // ======================================================================
        public SaveData GetSaveData()
        {
            return this.saveData;
        }
    }
}
