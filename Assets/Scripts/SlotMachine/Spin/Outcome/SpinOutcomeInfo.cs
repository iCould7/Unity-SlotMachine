using System;
using ICouldGames.SlotMachine.Spin.Item;
using UnityEngine;

namespace ICouldGames.SlotMachine.Spin.Outcome
{
    [Serializable]
    public class SpinOutcomeInfo
    {
        [SerializeField] private SpinItemType firstItemType;
        [SerializeField] private SpinItemType secondItemType;
        [SerializeField] private SpinItemType thirdItemType;
        [SerializeField] private int probability;

        public int Probability => probability;
    }
}