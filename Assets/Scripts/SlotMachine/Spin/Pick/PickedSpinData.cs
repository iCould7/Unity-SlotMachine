using System;
using ICouldGames.SlotMachine.Spin.Outcome.Info;

namespace ICouldGames.SlotMachine.Spin.Pick
{
    public class PickedSpinData
    {
        public SpinOutcomeInfo OutcomeInfo;
        public int ArrivalTime;
        public ValueTuple<int, int> ExpectedArrivalInterval;

        public PickedSpinData(SpinOutcomeInfo outcomeInfo)
        {
            OutcomeInfo = outcomeInfo;
        }
    }
}