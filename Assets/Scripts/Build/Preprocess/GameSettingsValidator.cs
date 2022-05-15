using System;
using ICouldGames.SlotMachine.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ICouldGames.Build.Preprocess
{
    public class GameSettingsValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var slotMachineSettings = AssetDatabase.LoadAssetAtPath<SlotMachineSettings>(SlotMachineSettings.FILE_PATH);

            try
            {
                slotMachineSettings.Validate();
            }
            catch (Exception e)
            {
                throw new BuildFailedException($"Build failed on game settings validation with message: {e.Message}");
            }
        }
    }
}