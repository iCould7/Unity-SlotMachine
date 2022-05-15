using System;
using System.Collections.Generic;
using ICouldGames.SlotMachine.Spin.Outcome;
using UnityEngine;

namespace ICouldGames.SlotMachine.Settings
{
    // Uncomment the line below if you want to Create a new scriptable object.
    // [CreateAssetMenu(fileName = "SlotMachineSettings", menuName = "SlotMachine/Settings", order = 1)]
    public class SlotMachineSettings : ScriptableObject
    {
        [SerializeField] private List<SpinOutcomeInfo> spinOutcomes;

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
    }
}