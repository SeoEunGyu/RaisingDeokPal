using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Character.Item
{

    internal class ItemSoil : GameItem
    {
        public ItemSoil(ItemCode code, string name, string decription, string uri) : base(code, name, decription, uri)
        {
            itemType = ItemType.Soil;
        }
    }

    /// <summary>
    /// 기본 화분
    /// </summary>
    internal class ItemDefaultSoil : ItemSoil
    {
        static ItemCode code = ItemCode.NormalSoil;
        static new string ItemName = "흙";
        static new string ItemDescription = "평범한 흙이다.";
        static string IconUri = "pack://application:,,,/asset/Icon/icon_normal_soil.png";

        public ItemDefaultSoil() : base(code, ItemName, ItemDescription, IconUri)
        {

        }
    }
}
