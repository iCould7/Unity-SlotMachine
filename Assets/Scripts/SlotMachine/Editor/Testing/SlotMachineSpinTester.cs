using System.Collections.Generic;
using System.Text;
using ICouldGames.SlotMachine.Settings;
using ICouldGames.SlotMachine.Settings.Constants;
using ICouldGames.SlotMachine.Spin.Pick;
using UnityEditor;

namespace ICouldGames.SlotMachine.Editor.Testing
{
    public static class SlotMachineSpinTester
    {
        public static string TestSpinOutcomes(int spinCount)
        {
            var slotMachineController = new EditorTestSlotMachineController(
                AssetDatabase.LoadAssetAtPath<SlotMachineSettings>(SlotMachineSettingsConstants.MAIN_SETTINGS_FILE_PATH));
            slotMachineController.Init();

            Dictionary<int, List<PickedSpinData>> spinTestDataByOutcomeId = new();

            for (int i = 0; i < spinCount; i++)
            {
                var globalPeriodCount = i / 100;
                var pickedSpinData = slotMachineController.GetNextSpin(false);

                if (!spinTestDataByOutcomeId.ContainsKey(pickedSpinData.OutcomeInfo.Id))
                {
                    spinTestDataByOutcomeId[pickedSpinData.OutcomeInfo.Id] = new List<PickedSpinData>();
                }

                pickedSpinData.ArrivalTime += globalPeriodCount * 100;
                pickedSpinData.ExpectedArrivalInterval.Item1 += globalPeriodCount * 100;
                pickedSpinData.ExpectedArrivalInterval.Item2 += globalPeriodCount * 100;
                spinTestDataByOutcomeId[pickedSpinData.OutcomeInfo.Id].Add(pickedSpinData);
            }

            // Build result string
            var stringBuilder = new StringBuilder();
            foreach (var spinDataList in spinTestDataByOutcomeId.Values)
            {
                stringBuilder.Append(spinDataList[0].OutcomeInfo);
                stringBuilder.Append("(Total: ");
                stringBuilder.Append(spinDataList.Count);
                stringBuilder.Append(")\n");
                foreach (var spinData in spinDataList)
                {
                    stringBuilder.Append("Expected Interval: ");
                    stringBuilder.Append(spinData.ExpectedArrivalInterval.Item1);
                    stringBuilder.Append(" - ");
                    stringBuilder.Append(spinData.ExpectedArrivalInterval.Item2);
                    stringBuilder.Append(" / Arrival Time: ");
                    stringBuilder.Append(spinData.ArrivalTime);
                    stringBuilder.Append("\n");
                }

                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
    }
}