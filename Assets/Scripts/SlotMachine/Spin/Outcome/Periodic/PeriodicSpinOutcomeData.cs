using System.Collections.Generic;
using System.Linq;
using ICouldGames.SlotMachine.Spin.Outcome.Info;
using ICouldGames.SlotMachine.Spin.Pick;

namespace ICouldGames.SlotMachine.Spin.Outcome.Periodic
{
    public class PeriodicSpinOutcomeData
    {
        private readonly Dictionary<int, int> _remainingCountOfPeriods = new();
        private int _shortPeriod;
        private int _longPeriod;
        private int _currentDeadline = 0;
        private int _currentPeriod = 0;

        public PickedSpinData LastPickedSpinData;
        public SpinOutcomeInfo SpinOutcomeInfo { get; private set; }
        public int CurrentDeadline => _currentDeadline;
        public int CurrentPeriod => _currentPeriod;

        public PeriodicSpinOutcomeData(SpinOutcomeInfo outcomeInfo)
        {
            SpinOutcomeInfo = outcomeInfo;
            Reset();
        }

        public bool IsNextOutcomeReady(int spinNumber)
        {
            return _currentDeadline <= spinNumber;
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

            _currentPeriod = period;
            _currentDeadline += period;
            return _currentDeadline;
        }

        public bool IsShorterPeriodAvailable()
        {
            return _shortPeriod < _currentPeriod
                   && _remainingCountOfPeriods[_shortPeriod] > 0;
        }

        public void SwapWithLowerPeriod()
        {
            LastPickedSpinData.ExpectedArrivalInterval.Item2--;
            _remainingCountOfPeriods[_longPeriod]++;
            _remainingCountOfPeriods[_shortPeriod]--;
            _currentDeadline--;
        }

        public void Reset()
        {
            _currentDeadline = 0;
            _currentPeriod = 0;
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
            _currentDeadline = copyData._currentDeadline;
            _currentPeriod = copyData._currentPeriod;
            SpinOutcomeInfo = copyData.SpinOutcomeInfo;
        }
    }
}