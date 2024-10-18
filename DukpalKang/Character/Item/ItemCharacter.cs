using RasingDeokPal.Common;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Character.Item
{
    /// <summary>
    /// 캐릭터 아이템 클래스
    /// </summary>
    internal class ItemCharacter : GameItem
    {
        public int[] Color { get; set; } = [0, 0, 0];

        public ItemCharacter(ItemCode code, string name, string decription, string uri) : base(code, name, decription, uri)
        {
            itemType = ItemType.Character;
        }

        /// <summary>
        /// 랜덤 컬러 값
        /// </summary>
        /// <returns></returns>
        protected int[] GetRandomCharaterColor()
        {
            Random rand = new Random();
            int[] defaultColor = GameConfig.GetConfig().StartDefaultColor;
            int[] dif = GameConfig.GetConfig().StartColorDif;

            // 데이터가 없거나 읽을 수 없는 경우
            //double startScale = GameConfig.GetConfig().StartScaleMin + (rand.NextDouble() * (GameConfig.GetConfig().StartScaleMax - GameConfig.GetConfig().StartScaleMin));
            int[] randColor =
                [
                    rand.Next(defaultColor[0]-dif[0], defaultColor[0] + dif[0]),
                        rand.Next(defaultColor[1]-dif[1], defaultColor[1] + dif[1]),
                        rand.Next(defaultColor[2]-dif[2], defaultColor[2] + dif[2])
                ];

            return randColor;
        }
    }

    internal class ItemDeokPal : ItemCharacter
    {
        static ItemCode code = ItemCode.DeokPal;
        static new string ItemName = "덕팔이";
        static new string ItemDescription = $"""
            칼큘러스 코노피듐 종이다.
            동그랗다.
            """;
        static string IconUri = "pack://application:,,,/asset/Icon/icon_deokpal.png";

        public ItemDeokPal() : base(code, ItemName, ItemDescription, IconUri)
        {
            Color = GetRandomCharaterColor();
        }
    }

    internal class ItemGoldenDeokPal : ItemCharacter
    {
        static ItemCode code = ItemCode.GoldenDeokPal;
        static new string ItemName = "황금 덕팔이";
        static new string ItemDescription = $"""
            칼큘러스 코노피듐 종이다.
            금빛으로 물들었다.
            """;
        static string IconUri = "pack://application:,,,/asset/Icon/icon_golden_deokpal.png";

        public ItemGoldenDeokPal() : base(code, ItemName, ItemDescription, IconUri)
        {
            Color = [255, 205, 50];
            rarity = DeokPalRarity.Unique;
        }
    }
}
