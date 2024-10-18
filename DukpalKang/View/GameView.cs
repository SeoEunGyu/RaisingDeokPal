using NAudio.Wave;
using RasingDeokPal.Character;
using RasingDeokPal.Character.Unit;
using RasingDeokPal.Common;
using RasingDeokPal.Common.API;
using RasingDeokPal.Common.Save;
using RasingDeokPal.Components.Chat;
using RasingDeokPal.Components.Menu;
using RasingDeokPal.effect;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.View
{
    internal class GameView : UIView
    {
        public DeokPalMenuPanel playerMenu;
        public PotMenu potMenu;
        internal DeokPal player;

        public InteractionMenu selectedMenu;   // 선택한 메뉴
        WasapiLoopbackCapture? soundCapture;

        Canvas bubbleCanvas;
        
        // 채팅 컴포넌트
        ChatBubble uiChat;
        List<GPTRole> chatLog;
        ChatInput uiChatInput;


        /// <summary>
        /// 게임 뷰
        /// </summary>
        /// <param name="canvas"></param>
        public GameView(Canvas gameCanvas, Canvas bubbleCanvas) : base(gameCanvas)
        {
            this.bubbleCanvas = bubbleCanvas;

            // 캐릭터 생성
            CreatePlayer();

            // 메뉴 생성
            CreateGameMenu();
        }

        private void CreatePlayer()
        {
            player = new DeokPal(canvas, "pack://application:,,,/asset/deokpal/deokpal_default.png");

            //SpriteCharacterUnit player2 = new SpriteCharacterUnit(canvas, "pack://application:,,,/asset/deokpal/deokpal_sheet.png", 150, 150, 150, 150, 9, 1, 80, 20, true);
            //SpriteCharacterUnit player3 = new SpriteCharacterUnit(canvas, "pack://application:,,,/asset/deokpal/deokchun_sheet.png", 150, 150, 150, 150, 4, 1, -230, 20, false);
        }

        /// <summary>
        /// 메뉴 생성
        /// </summary>
        private void CreateGameMenu()
        {
            this.playerMenu = new DeokPalMenuPanel(canvas, new List<AnimatedMenuButton>
            {
                new AnimatedMenuButton("이동", canvas,            "pack://application:,,,/asset/button/btn_move1.png", new RoutedEventHandler(SelectMenuMove),null),
                new AnimatedMenuButton("뽈롱", canvas,           "pack://application:,,,/asset/button/btn_touch1.png", new RoutedEventHandler(SelectMenuTouch),null),
                new AnimatedMenuButton("방치", canvas,          "pack://application:,,,/asset/button/btn_breath1.png", new RoutedEventHandler(SelectMenuBreath),null),
                new AnimatedMenuButton("음악듣기", canvas,       "pack://application:,,,/asset/button/btn_music1.png", new RoutedEventHandler(SelectMenuListenMusic),null),

                new AnimatedMenuButton("물주기", canvas,         "pack://application:,,,/asset/button/btn_water1.png", new RoutedEventHandler(SelectMenuWater),null),
                new AnimatedMenuButton("광합성", canvas,      "pack://application:,,,/asset/button/btn_sunshine1.png", new RoutedEventHandler(SelectMenuSunShine),null),
                new AnimatedMenuButton("영양제", canvas,      "pack://application:,,,/asset/button/nutirication.png", new RoutedEventHandler(SelectMenuNutrient),null)
            });

            this.potMenu = new PotMenu(canvas, new List<AnimatedMenuButton>
            {
                new AnimatedMenuButton("흙 변경", canvas,            "pack://application:,,,/asset/button/btn_change_soil.png", new RoutedEventHandler(SelectMenuSoilChange),null),
                new AnimatedMenuButton("화분 변경", canvas,         "pack://application:,,,/asset/button/btn_change_pot.png", new RoutedEventHandler(SelectMenuPotChange),null),
                new AnimatedMenuButton("장식품 변경", canvas,      "pack://application:,,,/asset/button/btn_change_acc.png", new RoutedEventHandler(SelectMenuAccessoryChange),null),
            });

            playerMenu.Hide();
            potMenu.Hide();
        }

        /// <summary>
        /// 메뉴 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="menuCode"></param>
        private void SelectMenu(InteractionMenu menu)
        {
            // 음악 듣기나 이동이 아닌 경우, 듣던거 취소
            if (!menu.Equals(InteractionMenu.ListenMusic) && !menu.Equals(InteractionMenu.None))
            {
                StopListenMusic();
            }

            switch (menu)
            {
                case InteractionMenu.Breath:
                    player.AnimationClear();
                    player.DoLivingYourLife();

                    // 채팅 만들기
                    CreateChatBubble();
                    break;
                case InteractionMenu.Touch:
                    // 타이머 초기화
                    player.ClearTimer();
                    break;
                case InteractionMenu.ListenMusic:
                    ListenMusic();
                    break;
                case InteractionMenu.Water:
                    player.ClearTimer();
                    player.AnimationClear();
                    break;
                //case InteractionMenu.Twinkle:
                //    SpriteUnit star = new SpriteUnit(MyCanvas, "pack://application:,,,/asset/effect/sprite_twinkle.png", 640, 633, 12);
                //    star.SetAnimationSpeed(50);
                //    star.SetAnimationStopHandler(star.RemoveSelf);
                //    star.AnimationStart();
                //    break;
                case InteractionMenu.SunShine:
                    player.ToggleSunshine();
                    break;
                case InteractionMenu.Nutrient:
                    player.ToggleNutrient();
                    break;
                

                // 장비 메뉴
                case InteractionMenu.SoilChange:
                    // 흙 변경
                    GameManager.Instance.GoEquipChange();
                    break;
                case InteractionMenu.PotChange:
                    GameManager.Instance.GoPickUp();
                    break;
                case InteractionMenu.AccessoryChange:
                    break;
            }

            potMenu.Hide();
            playerMenu.Hide();
            selectedMenu = menu;
#if DEBUG
            switch (selectedMenu)
            {
                case InteractionMenu.None:
                    Debug.WriteLine("이동 모드");
                    break;
                case InteractionMenu.Touch:
                    Debug.WriteLine("뽈롱 모드");
                    break;
                case InteractionMenu.Breath:
                    Debug.WriteLine("니 알아서 살아라 모드");
                    break;
                case InteractionMenu.Water:
                    Debug.WriteLine("물 주기 모드");
                    break;
                case InteractionMenu.Twinkle:
                    Debug.WriteLine("반짝이 생성 모드");
                    break;
                case InteractionMenu.SunShine:
                    Debug.WriteLine("햇살 모드");
                    break;
            }
#endif
        }

        /// <summary>
        /// 음악 드럼 비트에 맞춰서 둠칫
        /// </summary>
        private void ListenMusic()
        {
            // 음악 듣기 처리
            if (soundCapture == null)
            {
                //soundCapture = new WasapiLoopbackCapture(); // 스피커의 오디오를 캡처합니다
                //soundCapture.DataAvailable += CaptureSoundAvailableHandler2;
                //soundCapture.StartRecording();

                soundCapture = new WasapiLoopbackCapture();
                int bufferSize = 4096; // 버퍼 크기 설정
                var buffer = new byte[bufferSize];
                soundCapture.DataAvailable += (s, e) =>
                {
                    // 바이트 배열을 float 배열로 변환
                    int bytesRecorded = e.BytesRecorded;
                    if (bytesRecorded > buffer.Length)
                        bytesRecorded = buffer.Length;

                    Array.Copy(e.Buffer, buffer, bytesRecorded);
                    double bassEnergy = SoundCapture.ProcessAudioData(buffer, bytesRecorded, 40, 200);
                    if (bassEnergy >= 0.025)
                    {
                        //Debug.WriteLine($"큰 베이스: {bassEnergy}");
                        // 덕팔이 컨트롤
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            //player.DoBreathAction();
                            // 별가루 획득 시도
                            GameManager.Instance.GetStarDustFromListenMusic();
                            player.DoBreathVertical(100, 1.5);
                        }));
                    }
                    else if (bassEnergy >= 0.019)
                    {
                        //Debug.WriteLine($"베이스: {bassEnergy}");
                        // 덕팔이 컨트롤
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            //player.DoBreathAction();
                            // 별가루 획득 시도
                            GameManager.Instance.GetStarDustFromListenMusic();
                            player.DoBreathAction();
                            //player.DoBreathHorizon(100);
                        }));
                    }
                };

                soundCapture.StartRecording();
                // 이펙트 주기적으로 생성
                Point playerMargin = player.GetCurrentMargin();
                Point playerSize = player.GetCurrentActualSize();
                EffectLayer.Instance.EffectNoteOn(playerMargin.X + (playerSize.X / 2), playerMargin.Y);
            }
        }
        private void StopListenMusic()
        {
            if (soundCapture != null)
            {
                soundCapture.StopRecording();
                soundCapture.Dispose();
                soundCapture = null;
            }
            EffectLayer.Instance.EffectNoteOff();
        }

        //=============== 말풍선 생성 ===========================================
        public void CreateChatBubble()
        {
            if (uiChat != null)
            {
                uiChat.SetText($"""
                    반가워!.
                    또 궁금한 사항이 있니?
                    """);
                uiChat.Show();
                uiChatInput.Show();
            }
            else
            {
                string text = $"""안녕? 나는 덕팔이야""";
                // 덕팔이 현재 위치 반화
                uiChat = new ChatBubble(bubbleCanvas, text);
                uiChatInput = new ChatInput(bubbleCanvas);

                // 채팅 메세지 로그 생성
                CreateChatLog();
                
            }
            
        }
        public void HideChatBubble()
        {
            if(uiChat != null)
            {
                uiChat.Hide();
            }
            if(uiChatInput != null)
            {
                uiChatInput.Hide();
            }
        }

        /// <summary>
        /// 채팅 응답 하기
        /// </summary>
        /// <param name="text"></param>
        public void AnswerChatUI(string text)
        {
            uiChat.SetText(text);
            uiChat.Show();
        }

        /// <summary>
        /// GPT 채팅 초기 변수
        /// </summary>
        /// <returns></returns>
        private void CreateChatLog()
        {
            // 캐릭터 지정
            GPTRole optCharacter = new GPTRole { role = "system", content = 
                $"""
                    너는 귀여운 말투를 사용해.
                    니 이름은 덕팔이야.
                    """
            };
            // 상태 묻는 질문
            GPTRole optStatus = new GPTRole { role = "system", content = 
                $"""
                    현재 너의 상태를 묻는 질문, 지금 너의 상태는 어떠냐는 질문에는
                    질문 앞에 붙는 '[물]'태그의 값과 '[햇빛]' '[HP]' 태그의 값이 0~100의 범위 중 어느쪽에 가까운지에 따라 물 또는 햇빛이 부족한지 많은지 판단하고
                    현재 상태를 평가해줘. 

                    물은 {GameConfig.GetConfig().DamagePivotWaterMax} 수치보다 높거나 같으면 너무 많은 거야.
                    HP 수치는 낮으면 아프다고 이야기해.
                    너의 상태를 이야기할 때, 구체적인 수치는 말하지마.
                    """ };
            GPTRole optNonStatus = new GPTRole
            {
                role = "system",
                content =
                $"""
                    그외에 일반적인 질문이나, 인삿말에는 '[물]', '[햇빛]', '[HP]' 정보는 무시하고 관련한 언급도 하지말고 '[질문]'에 대한 답만 해.
                    물과 햇빛, HP에 관한 내용은 상태를 묻는 질문에만 참고해.
                    """
            };
            GPTRole optOrder = new GPTRole
            {
                role = "system",
                content =
                $"""
                    답변을 할때에는 항상 반말로 하도록 하고 이모티콘은 사용하지 마.
                    """
            };
            this.chatLog = new List<GPTRole>
                {
                    optCharacter,
                    //optStatus,
                    //optNonStatus,
                    //optOrder
                };
        }

        /// <summary>
        /// GPT API 호출
        /// </summary>
        public async Task ChatWithGPT(string text)
        {
            SaveData saveData = player.GetSaveData();

            string orderText = $"""
                [물]:{saveData.Water}, 
                [햇빛]:{saveData.SunShine}, 
                [HP]:{saveData.Hp} 
                [질문]:{text}
                """;

            Debug.WriteLine(orderText);

            // 01. 로그에 입력 내용 추가
            GPTRole order = new GPTRole()
            {
                role = "user",
                content = orderText
            };
            chatLog.Add(order);


            // API 호출
            GPTResponseDTO response = await OpenAPI.CallAPI<GPTResponseDTO>(text, chatLog);
            int count = response.Choices.Count;
            if(count > 0)
            {
                string responseText = "";
                foreach(var choice in response.Choices)
                {
                    string replacedText =
                        choice.Message.Content
                        .Replace(". ", ".\n")
                        .Replace("! ", "!\n")
                        .Replace("? ", "?\n");
                    responseText = String.Concat(responseText, replacedText, "\n");
                }

                // 02. 로그에 답변 내용 추가
                GPTRole answer = new GPTRole()
                {
                    role = "assistant",
                    content = responseText
                };
                chatLog.Add(answer);

                //답변 출력
                AnswerChatUI(responseText);
            }
            else
            {
                AnswerChatUI("말씀하신 내용을 이해하지 못 했어용!");       
            }
        }



        // 상호작용 메뉴
        private void SelectMenuMove(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.None);
        }
        private void SelectMenuTouch(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.Touch);
        }
        private void SelectMenuBreath(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.Breath);
        }
        private void SelectMenuListenMusic(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.ListenMusic);
        }
        private void SelectMenuTwinkle(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.Twinkle);
        }
        private void SelectMenuSunShine(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.SunShine);
        }
        private void SelectMenuWater(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.Water);
        }
        private void SelectMenuNutrient(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.Nutrient);
        }

        // 장비 메뉴
        private void SelectMenuSoilChange(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.SoilChange);
        }
        private void SelectMenuPotChange(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.PotChange);
        }
        private void SelectMenuAccessoryChange(object sender, RoutedEventArgs e)
        {
            SelectMenu(InteractionMenu.AccessoryChange);
        }
        
        /// <summary>
        /// 플레이어 메뉴 열기
        /// </summary>
        public void TogglePlayerMenu()
        {
            HideChatBubble();
            potMenu.Hide();
            playerMenu.Toggle();
        }
        
        /// <summary>
        /// 화분 메뉴 열기
        /// </summary>
        public void TogglePodMenu()
        {
            HideChatBubble();
            playerMenu.Hide();
            potMenu.Toggle();
        }
        /// <summary>
        /// 모든 메뉴 닫기
        /// </summary>
        public void HideAllMenu()
        {
            playerMenu.Hide();
            potMenu.Hide();
            HideChatBubble();
        }

        /// <summary>
        /// 정지
        /// </summary>
        public void Pause()
        {
            player.Pause();
        }
        /// <summary>
        /// 다시 재생
        /// </summary>
        public void Resume()
        {
            player.Resume();
        }
    }
}
