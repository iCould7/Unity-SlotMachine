using System;
using Random = UnityEngine.Random;

namespace ICouldGames.SlotMachine.Spin.Item
{
    // Don't change integer correspondents!
    public enum SpinItemType
    {
        Ace = 1,
        Seven = 2,
        Jackpot = 3,
        Wild = 4,
        Bonus = 5
    }

    public static class SpinItemTypeGenerator {
        public static SpinItemType GetRandom()
        {
            var enumList = Enum.GetValues(typeof(SpinItemType));

            return (SpinItemType) enumList.GetValue(Random.Range(0,enumList.Length));
        }
    }
}