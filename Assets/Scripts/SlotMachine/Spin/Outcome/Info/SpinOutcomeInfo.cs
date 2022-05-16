using System;
using ICouldGames.SlotMachine.Spin.Item;
using UnityEngine;

namespace ICouldGames.SlotMachine.Spin.Outcome.Info
{
    [Serializable]
    public class SpinOutcomeInfo
    {
        [SerializeField] private int id;
        [SerializeField] private SpinItemType firstItemType;
        [SerializeField] private SpinItemType secondItemType;
        [SerializeField] private SpinItemType thirdItemType;
        [SerializeField] private int probability;

        public int Id => id;
        public int Probability => probability;

        public override string ToString()
        {
            return $"{firstItemType},{secondItemType},{thirdItemType}";
        }
    }
}