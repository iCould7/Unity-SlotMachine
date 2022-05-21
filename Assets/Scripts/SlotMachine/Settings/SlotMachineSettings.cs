using System;
using System.Collections.Generic;
using ICouldGames.SlotMachine.Spin.Outcome.Info;
using UnityEngine;

namespace ICouldGames.SlotMachine.Settings
{
    [CreateAssetMenu(fileName = "SlotMachineSettings", menuName = "SlotMachine/Settings", order = 1)]
    public class SlotMachineSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<SpinOutcomeInfo> spinOutcomes;

        public void Validate()
        {
            var totalProbability = 0;

            foreach (var outcome in spinOutcomes)
            {
                totalProbability += outcome.Probability;
            }

            if (totalProbability != 100)
            {
                throw new Exception("SlotMachine spin outcomes total probability must be equal to 100");
            }
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            Validate();
        }
#endif
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            var prizeAvailableOutcomes = new List<SpinOutcomeInfo>();
            foreach (var outcomeInfo in spinOutcomes)
            {
                if (outcomeInfo.CoinPrize > 0)
                {
                    prizeAvailableOutcomes.Add(outcomeInfo);
                }
                else
                {
                    outcomeInfo.PrizeTier = 0;
                }
            }

            prizeAvailableOutcomes.Sort((c1, c2) =>c1.CoinPrize.CompareTo(c2.CoinPrize));

            for (var i = 0; i < prizeAvailableOutcomes.Count; i++)
            {
                prizeAvailableOutcomes[i].PrizeTier = i + 1;
            }
        }
    }
}