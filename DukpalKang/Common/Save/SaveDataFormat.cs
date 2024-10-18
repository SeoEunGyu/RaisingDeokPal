using RasingDeokPal.Components;
using System.Security.RightsManagement;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Common.Save
{
    internal class SaveDataFormat
    {
    
        /// <summary>
        /// 플레이어 데이터
        /// </summary>
        public class GameData
        {
            /// <summary>
            /// 인벤토리
            /// </summary>
            /// <remarks>
            /// 장착한 아이템은 제외
            /// </remarks>
            public List<ItemData> Inventory { get; set; } = new List<ItemData> { };

            /// <summary>
            /// 장비창
            /// </summary>
            /// 장착 아이템 슬롯
            public Equip Equipment { get; set; } = new Equip { };

            /// <summary>
            /// 인게임 재화
            /// </summary>
            public int Stardust { get; set; }
        }

        /// <summary>
        /// 장비창
        /// </summary>
        public class Equip
        {
            /// <summary>
            /// 꽃
            /// </summary>
            public PlantData? ItemPlant { get; set; }
            /// <summary>
            /// 화분
            /// </summary>
            public ItemData? ItemPot { get; set; }
            /// <summary>
            /// 흙
            /// </summary>
            public ItemData? ItemSoil { get; set; }
            /// <summary>
            /// 악세사리
            /// </summary>
            public ItemData[] ItemAcc { get; set; } = new ItemData[3];
        }

        /// <summary>
        /// 인벤토리 아이템 데이터
        /// </summary>
        public class ItemData
        {
            /// <summary>
            /// 아이템 고유 식별자
            /// </summary>
            public ItemCode ItemCode { get; set; }
            /// <summary>
            /// 아이템 타입
            /// </summary>
            public int ItemType { get; set; }
            /// <summary>
            /// 아이템 위치 좌표
            /// </summary>
            public double X { get; set; }
            /// <summary>
            /// 아이템 위치 좌표
            /// </summary>
            public double Y { get; set; }
            /// <summary>
            /// 아이템 zIndex
            /// </summary>
            public int zIndex { get; set; }
        }

        /// <summary>
        /// 식물 데이터
        /// </summary>
        public class PlantData : ItemData
        {
            public int lv { get; set; }
            public int hp { get; set; }
            public int exp { get; set; }

            public double playTime { get; set; }
            public DeokPalStatus Life { get; set; }
            
            // 개체값
            public int[] color { get; set; } = GameConfig.GetConfig().StartDefaultColor;
            public double scaleStart { get; set; }
            public double scale { get; set; }
            
            // 성장 요소
            public int water { get; set;}
            public int sunShine { get; set; }
            public int nutrition { get;set; }
        }
    }
}
