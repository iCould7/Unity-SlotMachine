using System;
using System.Linq;
using ICouldGames.Common.Interfaces.Startup;
using ICouldGames.SlotMachine.Settings;
using ICouldGames.SlotMachine.Spin.Outcome.Periodic;
using ICouldGames.SlotMachine.Spin.Pick;
using ICouldGames.SlotMachine.Spin.Simulation;
using UnityEngine;
using Random = System.Random;

namespace ICouldGames.SlotMachine.Controller
{
    public abstract class SlotMachineController : IInitializable
    {
        private readonly SlotMachineSettings _slotMachineSettings;
        private SpinSimulationData _pickabilityTestSimData;

        protected SpinSimulationData MainSimData;
        protected Random SpinPicker;
        protected int SpinPickerSeed;

        public SlotMachineController(SlotMachineSettings slotMachineSettings)
        {
            _slotMachineSettings = slotMachineSettings;
        }

        public virtual void Init()
        {
            InitPeriodicData();
        }

        private void InitPeriodicData()
        {
            MainSimData = new SpinSimulationData();
            _pickabilityTestSimData = new SpinSimulationData();
            foreach (var outcomeInfo in _slotMachineSettings.spinOutcomes)
            {
                MainSimData.PeriodicOutcomeDataList.Add(new PeriodicSpinOutcomeData(outcomeInfo));
                _pickabilityTestSimData.PeriodicOutcomeDataList.Add(new PeriodicSpinOutcomeData(outcomeInfo));
            }

            MainSimData.FetchNewArrivals();
        }

        public PickedSpinData GetNextSpin(bool triggerSave)
        {
            int pickedIndex = GetRandomSpinIndex();
            var spinResult = MainSimData.ActiveWaitingSpinInfos[pickedIndex].Data;
            var periodicData = MainSimData.PeriodicOutcomeDataList.First(x => x.SpinOutcomeInfo.Id == spinResult.Id);
            var pickedSpinData = new PickedSpinData(spinResult);
            pickedSpinData.ArrivalTime = MainSimData.CurrentSpinNumber;
            pickedSpinData.ExpectedArrivalInterval = (periodicData.CurrentDeadline - periodicData.CurrentPeriod, periodicData.CurrentDeadline - 1);
            periodicData.LastPickedSpinData = pickedSpinData;

            ProcessPickedSpin(MainSimData, pickedIndex);

            if (MainSimData.CurrentSpinNumber == 0)
            {
                SpinPickerSeed = Environment.TickCount;
            }

            if(triggerSave)
            {
                Save();
            }

#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                Debug.Log(spinResult);
            }
#endif

            return pickedSpinData;
        }

        private int GetRandomSpinIndex()
        {
            int lastPickableIndex = MainSimData.ActiveWaitingSpinInfos.Count - 1;
            while (lastPickableIndex >= 0)
            {
                if (IsSpinPickable(lastPickableIndex))
                {
                    break;
                }
                lastPickableIndex--;
            }

            return SpinPicker.Next(0, lastPickableIndex + 1);
        }

        private bool IsSpinPickable(int spinIndex)
        {
            _pickabilityTestSimData.Copy(MainSimData);
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

        protected abstract void Save();
    }
}