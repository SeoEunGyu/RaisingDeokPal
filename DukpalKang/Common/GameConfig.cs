using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Common
{
    internal class GameConfig
    {
        private static readonly Config instance = new Config();
        public static Config Instance => instance;

        /// <summary>
        /// 게임 Config 클래스
        /// </summary>
        public class Config
        {
            public int WindowWidth = 700;
            public int WindowHeight = 700;

            
            // 덕팔이 크기 관련 
            public double ScaleDownMin = 0.2;
            public double ScaleUpMax = 2.0;
            // 초기 리세마라 관련
            public double StartScaleMin = 0.4;
            public double StartScaleMax = 1.2;
            public int[] StartDefaultColor  = [132, 191, 154];
            public int[] StartColorDif      = [60, 10, 40];
            
            // 덕팔이 레어리티 확률
            public int RareDeokPalpercentage = 10;
            public int UniqueDeokPalpercentage = 5;

            public int[] ColorHongDeokPal = [255, 70, 70];
            public int[] ColorBaekDeokPal = [255, 255, 255];
            // 유니크 덕팔
            public int[] ColorHwangDeokPal = [255, 205, 50];
            public int[] ColorHwangDeokPalPot = [255, 180, 40];
            // 라이프 사이클
            public int LifeIntervalSecond;

            // 물주기 관련
            public int WaterDefaultValue = 20;   // 초기 물 수치, 물 수치 0~100
            public int WaterDownValue = 1;      // 1틱당 물 빠지는 수치
            public int ThirstyPivot;        // 목마름 상태 변경 기준

            // 햇빛 관련
            public int SunShineDefaultValue = 80;    // 햇살 수치
            public int SunShineDownValue = 5;       // 1틱당 줄어드는 햇살 수치
            public int SunShineUpValue = 1;         // 틱당 올라가는 햇살 수치
            public double SunShineInsufficientDownScale = 0.01; // 햇빛 부족인 경우 줄어드는 크기

            public int NutritionDefaultValue = 50;  // 영양제 기본 값

            // 데미지 관련
            public int DamagePivotWaterMin; // 물 부족 상태 데미지 기준
            public int DamagePivotWaterMax; // 물 과다 상태 데미지 기준
            public int DamageWaterValue;    // 물 관련 데미지 수치
            public int DamageSunShineLessValue = 1; // 햇빛 관련 데미지 수치

            // HP 관련
            public int DyingPivotHp;        // 죽어가는 상태 돌입 Hp
            public int[] dieGrayColor = [51, 51, 51];    // 죽었을때 컬러

            // 경험치 관련
            public int ExpMax;              // 경험치 최대값
            public int ExpUpValue;          // 경험치 오르는 수치

            // 별가루 획득
            public int StarDustMusicMax = 50;
            public int StarDustTouchMax = 10;
            public double StarDustMusicProbability = 0.3;   // 음악듣기로 획득할 확률
            public double StarDustTouchProbability = 0.8;   // 터치로 획득할 확률
            public int StarDustAddValue = 10;   // 별가루 획득량

            // 영양 관련
            public int NutrientValue = 10;  // 영양 회복량
            public int NutrientDamageValue = 10;    // 영양제 데미지

            // 레벨 관련
            public int GrowMaxLv = 20;           // 성장하는 구간
            public double GrowScale = 0.01;        // 성장치

            // 픽업 확률
            public Dictionary<ItemCode, double> pickUpData = new Dictionary<ItemCode, double>
                {
                    // 화분
                    {ItemCode.GoldenPot , 0.1},
                    {ItemCode.RedPot    , 0.3},
                    {ItemCode.NormalPot , 0.6},
                };
            public Dictionary<ItemCode, double> pickUpCharacter = new Dictionary<ItemCode, double>
            {
                {ItemCode.GoldenDeokPal , 0.5},
                {ItemCode.DeokPal , 0.5},
            };
        }


        /// <summary>
        /// 디버그 버전 Config
        /// </summary>
        public class ConfigDebug : Config
        {
            public ConfigDebug()
            {
                RareDeokPalpercentage = 15;         // 100퍼 확률로 홍덕팔 씨 나옴
                UniqueDeokPalpercentage = 100;      // 100퍼 확률 황덕팔

                StartScaleMin = 0.4;
                StartScaleMax = 1.2;

                LifeIntervalSecond = 10;
                
                WaterDefaultValue = 20;
                WaterDownValue = 1;
                ThirstyPivot = 20;

                SunShineDefaultValue = 80;
                SunShineDownValue = 5;
                SunShineUpValue = 1;

                DamagePivotWaterMax = 80;
                DamagePivotWaterMin = 10;
                DamageWaterValue = 1;

                DyingPivotHp = 20;
                
                ExpMax = 100;
                ExpUpValue = 20;
            }
        }
        /// <summary>
        /// 릴리즈 버전 Config
        /// </summary>
        public class ConfigRelease : Config
        {
            public ConfigRelease()
            {
                RareDeokPalpercentage = 10;
                UniqueDeokPalpercentage = 5;      // 100퍼 확률 황덕팔

                StartScaleMin = 0.4;
                StartScaleMax = 1.4;

                LifeIntervalSecond = 60;

                WaterDefaultValue = 20;
                WaterDownValue = 1;
                ThirstyPivot = 20;

                SunShineDefaultValue = 0;
                SunShineDownValue = 5;

                DamagePivotWaterMax = 80;
                DamagePivotWaterMin = 10;
                DamageWaterValue = 1;
                ExpMax = 100;
                ExpUpValue = 1; // 경험치 1

                DyingPivotHp = 20;  // 죽어가는 모드 기준

                GrowMaxLv = 20;
                GrowScale = 0.02;
            }
        }
        
        /// <summary>
        /// Config 가져오기
        /// </summary>
        /// <returns></returns>
        public static Config GetConfig()
        {
#if DEBUG
            return new ConfigDebug();
#elif RELEASE
            return new ConfigRelease();
#endif
        }
    }
}
