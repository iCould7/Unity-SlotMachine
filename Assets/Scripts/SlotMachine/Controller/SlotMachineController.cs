﻿using System.Collections.Generic;
using System.Linq;
using ICouldGames.Deadline;
using ICouldGames.SlotMachine.Settings;
using ICouldGames.SlotMachine.Spin.Outcome.Info;
using ICouldGames.SlotMachine.Spin.Outcome.Periodic;
using UnityEngine;

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

        private Dictionary<int, PeriodicSpinOutcomeData> _periodicOutcomeDataById = new();
        private List<DataWithDeadline<SpinOutcomeInfo>> _activeWaitingSpinInfos = new();
        private int _currentSpinNumber = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            InitPeriodicData();

            for (int i = 0; i < spinAmount; i++)
            {
                GetNextSpin();
            }

            //TODO: Delete Debug
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
            foreach (var outcomeInfo in slotMachineSettings.spinOutcomes)
            {
                _periodicOutcomeDataById[outcomeInfo.Id] = new PeriodicSpinOutcomeData(outcomeInfo);
            }

            FetchNewArrivals();
        }

        private void FetchNewArrivals()
        {
            foreach (var periodicOutcomeData in _periodicOutcomeDataById.Values)
            {
                if (periodicOutcomeData.IsNextOutcomeReady(_currentSpinNumber))
                {
                    //TODO: Object pooling needed for DataWithDeadline
                    var spinInfoWithDeadline = new DataWithDeadline<SpinOutcomeInfo>(periodicOutcomeData.SpinOutcomeInfo,
                        periodicOutcomeData.GetNextDeadline());
                    _activeWaitingSpinInfos.Add(spinInfoWithDeadline);
                }
            }

            _activeWaitingSpinInfos.Sort();
        }

        //TODO: Too predictable, add randomness
        public SpinOutcomeInfo GetNextSpin()
        {
            if (!_activeWaitingSpinInfos.Any())
            {
                foreach (var periodicOutcomeData in _periodicOutcomeDataById.Values)
                {
                    if (periodicOutcomeData.IsShorterPeriodAvailable())
                    {
                        periodicOutcomeData.SwapWithLowerPeriod();
                        FetchNewArrivals();
                        break;
                    }
                }
            }

            var spinResult = _activeWaitingSpinInfos[0].Data;
            //TODO: Don't use Remove on lists, use indexes if possible
            _activeWaitingSpinInfos.RemoveAt(0);

            _currentSpinNumber++;
            if (_currentSpinNumber == 100)
            {
                Reset();
            }

            FetchNewArrivals();

            //TODO: Delete Debug line
            if (!_resultCountOfOutcomes.ContainsKey(spinResult))
                _resultCountOfOutcomes[spinResult] = 1;
            else
                _resultCountOfOutcomes[spinResult]++;

#if UNITY_EDITOR
            Debug.Log(spinResult);
#endif
            return spinResult;
        }

        private void Reset()
        {
            _currentSpinNumber = 0;
            foreach (var periodicOutcomeData in _periodicOutcomeDataById.Values)
            {
                periodicOutcomeData.Reset();
            }
        }
    }
}