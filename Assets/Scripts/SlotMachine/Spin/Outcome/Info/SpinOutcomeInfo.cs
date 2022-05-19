using System;
using System.Collections.Generic;
using ICouldGames.SlotMachine.Spin.Item;
using UnityEngine;

namespace ICouldGames.SlotMachine.Spin.Outcome.Info
{
    [Serializable]
    public class SpinOutcomeInfo : ISerializationCallbackReceiver
    {
        [SerializeField] private int id;
        [SerializeField] private SpinItemType firstItemType;
        [SerializeField] private SpinItemType secondItemType;
        [SerializeField] private SpinItemType thirdItemType;
        [SerializeField] private int probability;

        [NonSerialized] public List<SpinItemType> SpinItemTypes = new(3);

        public int Id => id;
        public int Probability => probability;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            SpinItemTypes.Clear();
            SpinItemTypes.Add(firstItemType);
            SpinItemTypes.Add(secondItemType);
            SpinItemTypes.Add(thirdItemType);
        }

        public override string ToString()
        {
            return $"{firstItemType},{secondItemType},{thirdItemType}";
        }
    }
}