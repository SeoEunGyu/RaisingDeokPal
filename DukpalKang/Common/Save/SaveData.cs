using RasingDeokPal.Character.Item;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Common.Save
{
    internal class SaveData
    {
        // 1. 기존 버전 데이터 저장
        private static string dirName = "RagingDeokPal";
        private static string dataFileName = "data.ini";
        private static string playerDataFileName = "player.ini";

        public int Lv { get; set; } = 1;
        public int Hp { get; set; } = 100;
        public int Exp { get; set; } = 0;
        public double Scale { get; set; } = 0.5;
        public double PlayTime { get; set; } = 0;
        public DeokPalStatus Life { get; set; } = DeokPalStatus.Live;

        // 컬러 값
        public int[] color { get; set; } = GameConfig.GetConfig().StartDefaultColor;
        public double StartScale { get; set; }
        public DeokPalRarity rarity { get; set; } = DeokPalRarity.normal;
        public DateTime saveDate { get; set; }
        public ItemCode code { get; set; } = ItemCode.DeokPal;

        // 환경 정보
        public int Water { get; set; } = GameConfig.GetConfig().WaterDefaultValue;
        public int SunShine { get; set; } = GameConfig.GetConfig().SunShineDefaultValue;
        public int Nutrition { get; set; } = GameConfig.GetConfig().NutritionDefaultValue;

        override public string ToString()
        {
            return $"""
                #{saveDate}
                [덕팔이 정보]
                Lv:{Lv}
                HP:{Hp}
                Exp:{Exp}
                Life:{Life}
                Scale:{Scale}
                PlayTime:{PlayTime}
                [덕팔이 개체 값]
                Scale:{StartScale}
                Color:{color[0]},{color[1]},{color[2]}
                Rarity:{rarity}
                Code:{(int)code}
                [환경]
                Water:{Water}
                SunShine:{SunShine}
                Nutrition:{Nutrition}
            """;
        }

        /// <summary>
        /// 데이터 저장
        /// </summary>
        /// <param name="data"></param>
        public static void Save(SaveData data)
        {
            try
            {
                // AppData 폴더 경로 가져오기
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // INI 파일 경로 설정
                string iniFilePath = Path.Combine(appDataPath, dirName, dataFileName);

                // 디렉토리가 존재하지 않으면 생성
                string directoryPath = Path.GetDirectoryName(iniFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }


                data.ToString();
                // 파일 내용 정의
                string iniContent = data.ToString();
                // 파일 쓰기
                File.WriteAllText(iniFilePath, iniContent);

                //----------------------------------------------------------------------------------------------
                SaveDataFormat.GameData saveData = new SaveDataFormat.GameData();
                saveData.Equipment.ItemPlant = new SaveDataFormat.PlantData();
                GameSave.Save(saveData);
                //----------------------------------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"""저장 실패""");
            }
        }

        /// <summary>
        /// 캐릭터 설정
        /// </summary>
        /// <param name="charater"></param>
        internal void SetCharater(ItemCharacter charater)
        {
            color = charater.Color;
            rarity = charater.rarity;
        }

        /// <summary>
        /// 덕팔이 사망 저장
        /// </summary>
        public static void DieSave()
        {
            // AppData 폴더 경로 가져오기
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fileName = $"""data_Die_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.ini""";

            // INI 파일 경로 설정
            string filePath = Path.Combine(appDataPath, dirName, dataFileName);
            string newPath = Path.Combine(appDataPath, dirName, fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Move(filePath, newPath);
                    Debug.WriteLine("[파일 저장 성공] 덕팔이 사망 데이터 저장 완료");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[파일 저장 실패] 덕팔이 사망 데이터 저장 실패");
            }
        }

        /// <summary>
        /// 데이터 로드
        /// </summary>
        /// <returns></returns>
        public static SaveData Load()
        {
            try
            {
                var iniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dirName, dataFileName);
                var parser = new InitFileParser(iniFilePath);

                // [덕팔이 정보]
                string section = "덕팔이 정보";
                int valuelv = int.Parse(parser.GetValue(section, "Lv"));
                int valueHp = int.Parse(parser.GetValue(section, "HP"));
                int valueExp = int.Parse(parser.GetValue(section, "Exp"));
                double valueScale = double.Parse(parser.GetValue(section, "Scale"));
                DeokPalStatus valueLife = (DeokPalStatus)Enum.Parse(typeof(DeokPalStatus), parser.GetValue(section, "Life"));
                double valuePlayTime = double.Parse(parser.GetValue(section, "PlayTime"));

                //[덕팔이 이미지]
                section = "덕팔이 개체 값";
                string colorRow = parser.GetValue(section, "Color");
                double valueStartScale = double.Parse(parser.GetValue(section, "Scale"));
                int[] valueColor = colorRow.Split(',').Select(int.Parse).ToArray();
                DeokPalRarity rarity = (DeokPalRarity)Enum.Parse(typeof(DeokPalRarity), parser.GetValue(section, "Rarity"));
                int valueCode = int.Parse(parser.GetValue(section, "Code"));


                //[환경]
                section = "환경";
                int valueWater = int.Parse(parser.GetValue(section, "Water"));
                int valueSunShine = int.Parse(parser.GetValue(section, "SunShine"));
                int valueNutrition = int.Parse(parser.GetValue(section, "Nutrition"));

                //-----------------------------------------------------------------------------------------------------------
                SaveDataFormat.GameData saveData = GameSave.Load();
                //-----------------------------------------------------------------------------------------------------------

                return new SaveData
                {
                    // 정보
                    Lv = valuelv,
                    Hp = valueHp,
                    Exp = valueExp,
                    Scale = valueScale,
                    Life = valueLife,
                    PlayTime = valuePlayTime,
                    // 개체 값
                    color = valueColor,
                    StartScale = valueStartScale,
                    rarity = rarity,
                    code = (ItemCode)valueCode,
                    // 환경
                    Water = valueWater,
                    SunShine = valueSunShine,
                    Nutrition = valueNutrition
                };

            }
            catch (Exception ex)
            {
                Random rand = new Random();
                int[] defaultColor = GameConfig.GetConfig().StartDefaultColor;
                int[] dif = GameConfig.GetConfig().StartColorDif;

                // 데이터가 없거나 읽을 수 없는 경우
                double startScale = GameConfig.GetConfig().StartScaleMin + rand.NextDouble() * (GameConfig.GetConfig().StartScaleMax - GameConfig.GetConfig().StartScaleMin);
                int[] randColor =
                    [
                        rand.Next(defaultColor[0] - dif[0], defaultColor[0] + dif[0]),
                        rand.Next(defaultColor[1] - dif[1], defaultColor[1] + dif[1]),
                        rand.Next(defaultColor[2] - dif[2], defaultColor[2] + dif[2])
                    ];

                return new SaveData
                {
                    // 신규 데이터 생성
                    Scale = startScale,
                    StartScale = startScale,
                    color = randColor
                };

            }
        }

        /// <summary>
        /// 플레이어 데이터 확인
        /// </summary>
        /// <returns></returns>
        public static bool IsExistPlayData()
        {
            try
            {
                // AppData 폴더 경로 가져오기
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // INI 파일 경로 설정
                string iniFilePath = Path.Combine(appDataPath, dirName, playerDataFileName);

                // 디렉토리가 존재하지 않으면 생성
                string directoryPath = Path.GetDirectoryName(iniFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                return File.Exists(iniFilePath);
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static bool IsExistSaveData()
        {
            try
            {
                // AppData 폴더 경로 가져오기
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // INI 파일 경로 설정
                string iniFilePath = Path.Combine(appDataPath, dirName, dataFileName);

                // 디렉토리가 존재하지 않으면 생성
                string directoryPath = Path.GetDirectoryName(iniFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                return File.Exists(iniFilePath);
            }
            catch (Exception e)
            {
                return false;
            }
        }


        /// <summary>
        /// 플레이어 데이터
        /// </summary>
        /// <param name="data"></param>
        public static void SavePlayerData(PlayerData data)
        {
            try
            {
                // AppData 폴더 경로 가져오기
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                // INI 파일 경로 설정
                string iniFilePath = Path.Combine(appDataPath, dirName, playerDataFileName);

                // 디렉토리가 존재하지 않으면 생성
                string directoryPath = Path.GetDirectoryName(iniFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }


                data.ToString();
                // 파일 내용 정의
                string iniContent = data.ToString();
                // 파일 쓰기
                File.WriteAllText(iniFilePath, iniContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"""저장 실패""");
            }
        }
    }

    /// <summary>
    /// 플레이어 데이터
    /// </summary>
    internal class PlayerData
    {
        private static string dirName = "RagingDeokPal";
        private static string playerDataFileName = "player.ini";


        // 데이터
        DateTime saveDate { get; set; }
        int stardust = 0;
        int[] currentEquip { get; set; } = [
                (int)ItemCode.None,
            (int)ItemCode.None,
            (int)ItemCode.None
            ];
        List<int> items { get; set; } = new List<int>
        {
            (int)ItemCode.NormalPot
        };

        public List<int> GetItems()
        {
            return items;
        }
        /// <summary>
        /// 아이템 추가 
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(GameItem item)
        {
            items.Add((int)item.itemCode);
        }

        public int[] GetEquipData()
        {
            return currentEquip;
        }

        public override string ToString()
        {
            return $"""
                #{saveDate}
                [유저 정보]
                Stardust:{stardust}
                CurrentEquip:{string.Join(",", currentEquip)}
                Items:{string.Join(",", items)}
                """;
        }

        public void SetEquip(GameItem item)
        {
            switch (item.itemType)
            {
                case ItemType.Character:
                    currentEquip[0] = (int)item.itemCode;
                    break;
                case ItemType.Soil:
                    currentEquip[1] = (int)item.itemCode;
                    break;
                case ItemType.Pot:
                    currentEquip[2] = (int)item.itemCode;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 별가루 증가
        /// </summary>
        /// <param name="value"></param>
        public void AddStardust(int value = 1)
        {
            stardust += value;
        }
        public int GetStardust()
        {
            return stardust;
        }
        /// <summary>
        /// 플레이어 정보 로드
        /// </summary>
        /// <returns></returns>
        public static PlayerData Load()
        {
            try
            {

                // 

                var iniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dirName, playerDataFileName);
                var parser = new InitFileParser(iniFilePath);

                //[덕팔이 이미지]
                string section = "유저 정보";
                int valueStarDust = int.Parse(parser.GetValue(section, "Stardust"));
                // 장비
                string currentEquipRow = parser.GetValue(section, "CurrentEquip");
                int[] valueCurrentEquip = currentEquipRow.Split(',').Select(int.Parse).ToArray();

                // 인벤토리
                string itemRow = parser.GetValue(section, "Items");
                List<int> valueItem = new List<int>();
                if (!string.IsNullOrWhiteSpace(itemRow))
                {
                    valueItem = itemRow.Split(',').Select(int.Parse).ToList();
                }


                return new PlayerData
                {
                    stardust = valueStarDust,
                    currentEquip = valueCurrentEquip,
                    items = valueItem
                };

            }
            catch (Exception e)
            {
                return new PlayerData();
            }
        }
    }

}
