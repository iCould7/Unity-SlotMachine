using System.Collections.Generic;
using ICouldGames.SlotMachine.Settings;
using ICouldGames.SlotMachine.Settings.Constants;
using ICouldGames.SlotMachine.Spin.Pick;
using UnityEditor;
using UnityEngine;

namespace ICouldGames.SlotMachine.Editor.Testing
{
    public static class SlotMachineSpinTester
    {
        [MenuItem("Test/SlotMachine spin test")]
        public static void TestSpinOutcomes()
        {
            var slotMachineController = new EditorTestSlotMachineController(
                AssetDatabase.LoadAssetAtPath<SlotMachineSettings>(SlotMachineSettingsConstants.MAIN_SETTINGS_FILE_PATH));
            slotMachineController.Init();

            Dictionary<int, List<PickedSpinData>> spinTestDataByOutcomeId = new();

            for (int i = 0; i < 100; i++)
            {
                var pickedSpinData = slotMachineController.GetNextSpin(false);

                if (!spinTestDataByOutcomeId.ContainsKey(pickedSpinData.OutcomeInfo.Id))
                {
                    spinTestDataByOutcomeId[pickedSpinData.OutcomeInfo.Id] = new List<PickedSpinData>();
                }

                spinTestDataByOutcomeId[pickedSpinData.OutcomeInfo.Id].Add(pickedSpinData);
            }

            // Log results into console
            foreach (var spinDataList in spinTestDataByOutcomeId.Values)
            {
                var sameOutcomeDebugString = $"{spinDataList[0].OutcomeInfo} (Total: {spinDataList.Count})\n";
                foreach (var spinData in spinDataList)
                {
                    sameOutcomeDebugString += $"Expected Interval: {spinData.ExpectedArrivalInterval.Item1} - {spinData.ExpectedArrivalInterval.Item2}" +
                                              $" / Arrival Time: {spinData.ArrivalTime}\n";
                }
                Debug.Log(sameOutcomeDebugString);
            }
        }
    }
}