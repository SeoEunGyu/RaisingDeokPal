namespace RasingDeokPal.Common.Enums
{
    internal class CommonEnum
    {
        /// <summary>
        /// 메뉴 선택
        /// </summary>
        public enum InteractionMenu
        {
            None,
            Touch,
            Breath,
            ListenMusic,
            Twinkle,
            Water,
            SunShine,
            Nutrient,

            SoilChange,
            PotChange,
            AccessoryChange
        }

        /// <summary>
        /// 장비 교체 메뉴 선택
        /// </summary>
        public enum SelectEquipMenu
        {
            Soil,
            Pot
        }

        public enum DeokPalRarity
        {
            normal,    // 기본
            Rare,       // 레어
            Unique      // 유니크
        }

        /// <summary>
        /// 생존 상태
        /// </summary>
        public enum DeokPalStatus
        {
            Live,
            Hurt,
            Die
        }
        
        /// <summary>
        /// 플레이어 성장 페이즈
        /// </summary>
        public enum PlayerGrowPhase
        {
            Phase1 = 20,
            Phase2 = 30,
            Phase3 = 40,
        }

        /// <summary>
        /// 아이템 타입
        /// </summary>
        public enum ItemType
        {
            Soil,
            Pot,
            Character,
            Accessory
        }

        /// <summary>
        /// 아이템 코드
        /// </summary>
        public enum ItemCode
        {
            None = -1,
            // 화분
            NormalPot = 0,
            GoldenPot = 1,
            RedPot = 2,

            // 흙
            NormalSoil = 100,

            // 식물
            DeokPal = 200,          // 덕팔이
            GoldenDeokPal = 201     // 황금 덕팔이
        }
    }
}
