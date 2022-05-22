using System;
using ICouldGames.SlotMachine.Settings;
using UnityEngine;
using Random = System.Random;

namespace ICouldGames.SlotMachine.Controller
{
    public class GameSlotMachineController : SlotMachineController
    {
        private const string RANDOM_SEED_SAVE_KEY = "slotMachine_spin_random_seed";
        private const string LAST_SESSION_SPIN_NUMBER_SAVE_KEY = "slotMachine_last_session_spin_number";

        public GameSlotMachineController(SlotMachineSettings slotMachineSettings) : base(slotMachineSettings)
        {
        }

        public override void Init()
        {
            base.Init();
            SpinPickerSeed = PlayerPrefs.GetInt(RANDOM_SEED_SAVE_KEY, Environment.TickCount);
            SpinPicker = new Random(SpinPickerSeed);
            ProcessInitialSimulationData();
        }

        private void ProcessInitialSimulationData()
        {
            var lastSessionSpinNumber = PlayerPrefs.GetInt(LAST_SESSION_SPIN_NUMBER_SAVE_KEY, 0);
            for (int i = 0; i < lastSessionSpinNumber; i++)
            {
                GetNextSpin(false);
            }
        }

        protected override void Save()
        {
            PlayerPrefs.SetInt(RANDOM_SEED_SAVE_KEY, SpinPickerSeed);
            PlayerPrefs.SetInt(LAST_SESSION_SPIN_NUMBER_SAVE_KEY, MainSimData.CurrentSpinNumber);
            PlayerPrefs.Save();
        }
    }
}