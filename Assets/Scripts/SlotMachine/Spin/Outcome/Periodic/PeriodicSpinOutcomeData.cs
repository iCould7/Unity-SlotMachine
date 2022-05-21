using System.Collections.Generic;
using System.Linq;
using ICouldGames.SlotMachine.Spin.Outcome.Info;

namespace ICouldGames.SlotMachine.Spin.Outcome.Periodic
{
    public class PeriodicSpinOutcomeData
    {
        private readonly Dictionary<int, int> _remainingCountOfPeriods = new();
        private int _shortPeriod;
        private int _longPeriod;
        private int _lastDeadline = 0;
        private int _lastPeriod = 0;

        public SpinOutcomeInfo SpinOutcomeInfo { get; private set; }

        public PeriodicSpinOutcomeData(SpinOutcomeInfo outcomeInfo)
        {
            SpinOutcomeInfo = outcomeInfo;
            Reset();
        }

        public bool IsOutcomeAvailable()
        {
            return _remainingCountOfPeriods[_shortPeriod] > 0
                   || _remainingCountOfPeriods[_longPeriod] > 0;
        }

        public bool IsNextOutcomeReady(int spinNumber)
        {
            return _lastDeadline <= spinNumber;
        }

        public int GetNextDeadline()
        {
            int period;
            if (_remainingCountOfPeriods[_longPeriod] > 0)
            {
                _remainingCountOfPeriods[_longPeriod]--;
                period = _longPeriod;
            }
            else
            {
                _remainingCountOfPeriods[_shortPeriod]--;
                period = _shortPeriod;
            }

            _lastPeriod = period;
            _lastDeadline += period;
            return _lastDeadline;
        }

        public bool IsShorterPeriodAvailable()
        {
            return _shortPeriod < _lastPeriod
                   && _remainingCountOfPeriods[_shortPeriod] > 0;
        }

        public void SwapWithLowerPeriod()
        {
            _remainingCountOfPeriods[_longPeriod]++;
            _remainingCountOfPeriods[_shortPeriod]--;
            _lastDeadline--;
        }

        public void Reset()
        {
            _lastDeadline = 0;
            _lastPeriod = 0;
            var probability = SpinOutcomeInfo.Probability;
            _shortPeriod = 100 / probability;
            _longPeriod = _shortPeriod + 1;

            var longPeriodCount = 100 % probability;
            var shortPeriodCount = probability - longPeriodCount;
            _remainingCountOfPeriods[_shortPeriod] = shortPeriodCount;
            _remainingCountOfPeriods[_longPeriod] = longPeriodCount;
        }

        public void Copy(PeriodicSpinOutcomeData copyData)
        {
            foreach (var key in _remainingCountOfPeriods.Keys.ToList())
            {
                _remainingCountOfPeriods[key] = copyData._remainingCountOfPeriods[key];
            }

            _shortPeriod = copyData._shortPeriod;
            _longPeriod = copyData._longPeriod;
            _lastDeadline = copyData._lastDeadline;
            _lastPeriod = copyData._lastPeriod;
            SpinOutcomeInfo = copyData.SpinOutcomeInfo;
        }
    }
}