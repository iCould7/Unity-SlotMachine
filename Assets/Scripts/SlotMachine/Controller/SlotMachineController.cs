using System.Collections.Generic;
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

        //TODO: Delete Debug field
        public int spinAmount = 10;
        private Dictionary<SpinOutcomeInfo, int> _resultCountOfOutcomes = new();

        public bool IsReady { get; private set; } = false;

        private SpinSimulationData _mainSimData;
        private SpinSimulationData _pickabilityTestSimData;
        private Random _spinPicker;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            _spinPicker = new Random();
            InitPeriodicData();

            //TODO: Delete Debug
             for (int i = 0; i < spinAmount; i++)
             {
                 GetNextSpin();
             }
            PrintResults();

            IsReady = true;
        }

        //TODO: Delete Debug
        private void PrintResults()
        {
            foreach (var key in _resultCountOfOutcomes.Keys)
            {
                Debug.Log(key + ": " + _resultCountOfOutcomes[key]);
            }
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

        //TODO: Too predictable, add randomness
        public SpinOutcomeInfo GetNextSpin()
        {
            int pickedIndex = GetRandomSpinIndex();
            var spinResult = _mainSimData.ActiveWaitingSpinInfos[pickedIndex].Data;
            ProcessPickedSpin(_mainSimData, pickedIndex);

            //TODO: Delete Debug line
            if (!_resultCountOfOutcomes.ContainsKey(spinResult))
                _resultCountOfOutcomes[spinResult] = 1;
            else
                _resultCountOfOutcomes[spinResult]++;

#if UNITY_EDITOR
            //Debug.Log(spinResult);
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

            int earliestDeadlineIndex = 0;
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
    }
}