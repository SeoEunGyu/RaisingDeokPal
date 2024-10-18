using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Character.Item
{
    /// <summary>
    /// 화분 아이템 클래스
    /// </summary>
    internal class ItemPot : GameItem
    {
        // 화분의 역할
        // 최대 물 저장 공간 확보
        public double WaterMax { get; set; } = 100.0;   // 물 저장량
        public double WaterDown { get; set; } = 5.0;    // 물 빠짐 수치
        public int[] Color { get; set; } = [0, 0, 0];

        public ItemPot(ItemCode code, string name, string decription, string uri) : base(code, name, decription, uri)
        {
            itemType = ItemType.Pot;
        }
        public virtual void SetItemOption(PlantPot pot)
        {

        }
    }

    /// <summary>
    /// 기본 화분
    /// </summary>
    internal class ItemDefaultPot : ItemPot
    {
        static ItemCode code = ItemCode.NormalPot;
        static new string ItemName = "흙 화분";
        static new string ItemDescription = "평범한 화분이다.";
        static string IconUri = "pack://application:,,,/asset/Icon/icon_normal_pot.png";

        public ItemDefaultPot() : base(code, ItemName, ItemDescription, IconUri)
        {
            resourceUri = "pack://application:,,,/asset/deokpal/pot.png";
        }
    }

    /// <summary>
    /// 빨간 화분
    /// </summary>
    internal class ItemRedPot : ItemPot
    {
        static ItemCode code = ItemCode.RedPot;
        static new string ItemName = "빨간 화분";
        static new string ItemDescription = $"""
            붉은 색의 평범한 화분이다.
            물 저장량이 약간 증가한다.
            """;
        static string IconUri = "pack://application:,,,/asset/Icon/icon_red_pot.png";

        public ItemRedPot() : base(code, ItemName, ItemDescription, IconUri)
        {
            resourceUri = "pack://application:,,,/asset/deokpal/pot.png";
            Color = [230, 80, 60];
        }

        /// <summary>
        /// 아이템 반영
        /// </summary>
        public override void SetItemOption(PlantPot pot)
        {
            // 컬러 설정
            pot.SetPotColor();
        }
    }

    /// <summary>
    /// 황금 화분
    /// </summary>
    internal class ItemGoldenPot : ItemPot
    {
        static ItemCode code = ItemCode.GoldenPot;
        static new string ItemName = "황금 화분";
        static new string ItemDescription = $"""
            황금으로 만든 화분이다.
            물 저장량이 크게 증가한다.
            """;
        static string IconUri = "pack://application:,,,/asset/Icon/icon_golden_pot.png";

        public ItemGoldenPot() : base(code, ItemName, ItemDescription, IconUri)
        {
            resourceUri = "pack://application:,,,/asset/deokpal/pot.png";
            // 황금 컬러
            Color = [255, 180, 40];
            rarity = DeokPalRarity.Unique;
        }

        /// <summary>
        /// 아이템 반영
        /// </summary>
        public override void SetItemOption(PlantPot pot)
        {
            // 컬러 설정
            pot.SetPotColor();

            // 반짝이 타이머
            pot.SetTwinkleTimer();
        }
    }
}
