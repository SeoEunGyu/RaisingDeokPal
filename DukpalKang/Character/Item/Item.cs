
using RasingDeokPal.Common;
using RasingDeokPal.Common.Enums;
using System.Windows.Media.Imaging;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Character.Item
{
    internal class GameItem
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public BitmapImage imgIcon { get; set; }
        public DeokPalRarity rarity { get; set; } = DeokPalRarity.normal;
        public ItemType itemType { get; set; }
        public ItemCode itemCode { get; set; }
        public string? resourceUri { get; set; }

        private int iconWidth = 32;
        private int iconHeight = 32;

        public GameItem(ItemCode code, string name, string desc, string iconUri)
        {
            this.itemCode = code;
            this.ItemName = name;
            this.ItemDescription = desc;
            this.imgIcon = UIControl.CreateBitmap(iconUri, iconWidth, iconHeight);
        }

        /// <summary>
        /// 아이템 반환
        /// </summary>
        /// <param name="codeValue"></param>
        /// <returns></returns>
        public static GameItem GetItem(int codeValue)
        {
            ItemCode code = (ItemCode)codeValue;
            switch (code)
            {
                case ItemCode.NormalPot:
                    return new ItemDefaultPot();
                case ItemCode.GoldenPot:
                    return new ItemGoldenPot();
                case ItemCode.RedPot:
                    return new ItemRedPot();
                case ItemCode.NormalSoil:
                    return new ItemDefaultSoil();
                case ItemCode.DeokPal:
                    return new ItemDeokPal();
                case ItemCode.GoldenDeokPal:
                    return new ItemGoldenDeokPal();
                default:
                    return new ItemDefaultPot();
            }
        }
        public static GameItem GetItem(ItemCode codeValue)
        {
            return GetItem((int)codeValue);
        }
    }
}
