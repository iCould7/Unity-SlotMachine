using System.Collections.Generic;
using ICouldGames.Deadline;
using ICouldGames.SlotMachine.Spin.Outcome.Info;
using ICouldGames.SlotMachine.Spin.Outcome.Periodic;

namespace ICouldGames.SlotMachine.Spin.Simulation
{
    public class SpinSimulationData
    {
        public List<PeriodicSpinOutcomeData> PeriodicOutcomeDataList = new();
        public List<DataWithDeadline<SpinOutcomeInfo>> ActiveWaitingSpinInfos = new();
        public int CurrentSpinNumber = 0;

        public void FetchNewArrivals()
        {
            foreach (var periodicOutcomeData in PeriodicOutcomeDataList)
            {
                if (periodicOutcomeData.IsNextOutcomeReady(CurrentSpinNumber))
                {
                    //TODO: Object pooling needed for DataWithDeadline
                    var spinInfoWithDeadline = new DataWithDeadline<SpinOutcomeInfo>(periodicOutcomeData.SpinOutcomeInfo,
                        periodicOutcomeData.GetNextDeadline());
                    ActiveWaitingSpinInfos.Add(spinInfoWithDeadline);
                }
            }

            ActiveWaitingSpinInfos.Sort();
        }

        public bool IsAnyDeadlineFailed()
        {
            foreach (var waitingSpinInfo in ActiveWaitingSpinInfos)
            {
                if (waitingSpinInfo.Deadline <= CurrentSpinNumber)
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            CurrentSpinNumber = 0;
            foreach (var periodicOutcomeData in PeriodicOutcomeDataList)
            {
                periodicOutcomeData.Reset();
            }
        }

        public void Copy(SpinSimulationData copyData)
        {
            for (int i = 0; i < PeriodicOutcomeDataList.Count; i++)
            {
                PeriodicOutcomeDataList[i].Copy(copyData.PeriodicOutcomeDataList[i]);
            }

            //TODO: Object pooling needed
            ActiveWaitingSpinInfos.Clear();
            for (int i = 0; i < copyData.ActiveWaitingSpinInfos.Count; i++)
            {
                ActiveWaitingSpinInfos.Add(new DataWithDeadline<SpinOutcomeInfo>(copyData.ActiveWaitingSpinInfos[i].Data,
                    copyData.ActiveWaitingSpinInfos[i].Deadline));
            }

            CurrentSpinNumber = copyData.CurrentSpinNumber;
        }
    }
}