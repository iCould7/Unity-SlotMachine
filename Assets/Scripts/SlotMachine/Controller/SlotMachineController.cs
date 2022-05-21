﻿using System;
using System.Linq;
using ICouldGames.SlotMachine.Settings;
using ICouldGames.SlotMachine.Spin.Outcome.Info;
using ICouldGames.SlotMachine.Spin.Outcome.Periodic;
using ICouldGames.SlotMachine.Spin.Simulation;
using UnityEngine;
using Random = System.Random;

namespace ICouldGames.SlotMachine.Controller
{
    public class SlotMachineController : MonoBehaviour
    {
        public static SlotMachineController Instance { get; private set; }

        [SerializeField] private SlotMachineSettings slotMachineSettings;

        public bool IsReady { get; private set; } = false;

        private SpinSimulationData _mainSimData;
        private SpinSimulationData _pickabilityTestSimData;
        private Random _spinPicker;
        private int _spinPickerSeed;

        private const string RANDOM_SEED_SAVE_KEY = "slotMachine_spin_random_seed";
        private const string LAST_SESSION_SPIN_NUMBER_SAVE_KEY = "slotMachine_last_session_spin_number";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Init();

            IsReady = true;
        }

        private void Init()
        {
            _spinPickerSeed = PlayerPrefs.GetInt(RANDOM_SEED_SAVE_KEY, Environment.TickCount);
            _spinPicker = new Random(_spinPickerSeed);
            InitPeriodicData();
            ProcessInitialSimulationData();
        }

        private void InitPeriodicData()
        {
            _mainSimData = new SpinSimulationData();
            _pickabilityTestSimData = new SpinSimulationData();
            foreach (var outcomeInfo in slotMachineSettings.spinOutcomes)
            {
                _mainSimData.PeriodicOutcomeDataList.Add(new PeriodicSpinOutcomeData(outcomeInfo));
                _pickabilityTestSimData.PeriodicOutcomeDataList.Add(new PeriodicSpinOutcomeData(outcomeInfo));
            }

            _mainSimData.FetchNewArrivals();
        }

        private void ProcessInitialSimulationData()
        {
            var lastSessionSpinNumber = PlayerPrefs.GetInt(LAST_SESSION_SPIN_NUMBER_SAVE_KEY, 0);
            for (int i = 0; i < lastSessionSpinNumber; i++)
            {
                GetNextSpin(false);
            }
        }

        public SpinOutcomeInfo GetNextSpin(bool triggerSave)
        {
            int pickedIndex = GetRandomSpinIndex();
            var spinResult = _mainSimData.ActiveWaitingSpinInfos[pickedIndex].Data;
            ProcessPickedSpin(_mainSimData, pickedIndex);

            if (_mainSimData.CurrentSpinNumber == 0)
            {
                _spinPickerSeed = Environment.TickCount;
            }

            if(triggerSave)
            {
                Save();
            }

#if UNITY_EDITOR
            Debug.Log(spinResult);
#endif

            return spinResult;
        }

        private int GetRandomSpinIndex()
        {
            int lastPickableIndex = _mainSimData.ActiveWaitingSpinInfos.Count - 1;
            while (lastPickableIndex >= 0)
            {
                if (IsSpinPickable(lastPickableIndex))
                {
                    break;
                }
                lastPickableIndex--;
            }

            return _spinPicker.Next(0, lastPickableIndex + 1);
        }

        private bool IsSpinPickable(int spinIndex)
        {
            _pickabilityTestSimData.Copy(_mainSimData);
            ProcessPickedSpin(_pickabilityTestSimData, spinIndex);
            if (_pickabilityTestSimData.IsAnyDeadlineFailed())
            {
                return false;
            }

            const int earliestDeadlineIndex = 0;
            while (_pickabilityTestSimData.CurrentSpinNumber != 0)
            {
                ProcessPickedSpin(_pickabilityTestSimData, earliestDeadlineIndex);
                if (_pickabilityTestSimData.IsAnyDeadlineFailed())
                {
                    return false;
                }
            }

            return true;
        }

        private void ProcessPickedSpin(SpinSimulationData simData, int pickedIndex)
        {
            simData.ActiveWaitingSpinInfos.RemoveAt(pickedIndex);

            simData.CurrentSpinNumber++;
            if (simData.CurrentSpinNumber == 100)
            {
                simData.Reset();
            }

            simData.FetchNewArrivals();

            if (!simData.ActiveWaitingSpinInfos.Any())
            {
                foreach (var periodicOutcomeData in simData.PeriodicOutcomeDataList)
                {
                    if (periodicOutcomeData.IsShorterPeriodAvailable())
                    {
                        periodicOutcomeData.SwapWithLowerPeriod();
                        simData.FetchNewArrivals();
                        break;
                    }
                }
            }
        }

        public void Save()
        {
            PlayerPrefs.SetInt(RANDOM_SEED_SAVE_KEY, _spinPickerSeed);
            PlayerPrefs.SetInt(LAST_SESSION_SPIN_NUMBER_SAVE_KEY, _mainSimData.CurrentSpinNumber);
            PlayerPrefs.Save();
        }
    }
}