using RasingDeokPal.Character.Item;
using static RasingDeokPal.Common.Enums.CommonEnum;

namespace RasingDeokPal.Common
{
    internal class PickUpManager
    {
        /// <summary>
        /// 랜덤 아이템 반환
        /// </summary>
        /// <returns></returns>
        public static GameItem GetRandomItem()
        {
            Dictionary<ItemCode, double> pickUpData = GameConfig.GetConfig().pickUpData;
            GameItem defaultItem = GameItem.GetItem((int)ItemCode.NormalPot);

            return CalRandom(pickUpData, defaultItem);
        }

        /// <summary>
        /// 랜덤 캐릭터 반환
        /// </summary>
        /// <returns></returns>
        public static GameItem GetRandomCharacter()
        {
            Dictionary<ItemCode, double> pickUpData = GameConfig.GetConfig().pickUpCharacter;
            GameItem defaultItem = GameItem.GetItem((int)ItemCode.GoldenDeokPal);

            return CalRandom(pickUpData, defaultItem);
        }

        /// <summary>
        /// 랜덤 아이템 계산
        /// </summary>
        /// <param name="itemPool"></param>
        /// <param name="defaultItem"></param>
        /// <returns></returns>
        private static GameItem CalRandom(Dictionary<ItemCode, double> itemPool, GameItem defaultItem)
        {
            double cumulativeProbability = 0.0;
            foreach (ItemCode itemCode in itemPool.Keys)
            {
                // 확률 계산
                double per = itemPool[itemCode];
                cumulativeProbability += per;

                bool pickupSuccess = WindowControlMethod.GetRandomBool(cumulativeProbability);
                if (pickupSuccess)
                {
                    // 픽업 아이템 생성
                    GameItem? item = GameItem.GetItem((int)itemCode);
                    if (item != null)
                    {
                        return item;
                    }
                    break;
                }
            }
            return defaultItem;
        }
    }
}
