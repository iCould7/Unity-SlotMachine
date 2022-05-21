using System.Collections;
using ICouldGames.Extensions.System.Collections.Generic;
using ICouldGames.SlotMachine.Controller;
using ICouldGames.SlotMachine.Spin.Item;
using ICouldGames.SlotMachine.Spin.Outcome.Info;
using ICouldGames.SlotMachine.View.Column;
using ICouldGames.SlotMachine.View.Column.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace ICouldGames.SlotMachine.View
{
    public class SlotMachineObject : MonoBehaviour
    {
        [SerializeField] private Button spinButton;
        [SerializeField] private SlotMachineColumn column1;
        [SerializeField] private SlotMachineColumn column2;
        [SerializeField] private SlotMachineColumn column3;
        [SerializeField] private ParticleSystem coinParticleSystem;

        [Header("Settings")]
        [Range(0.5f, 500f)]
        [SerializeField] private float startingSpinSpeed;
        [Range(0.05f, 10f)]
        [SerializeField] private float startingSpinDuration;
        [Range(0.05f, 10f)]
        [SerializeField] private float fastSpinStopDuration;
        [Range(0.05f, 10f)]
        [SerializeField] private float[] criticalLastSpinStopDurations;

        private IEnumerator Start()
        {
            yield return new WaitUntil(IsDependenciesReady);

            spinButton.onClick.AddListener(() => StartCoroutine(Spin()));
        }

        private bool IsDependenciesReady()
        {
            return SlotMachineController.Instance.IsReady
                   && SpinItemImageProvider.Instance.IsReady;
        }

        private IEnumerator Spin()
        {
            spinButton.interactable = false;

            var spinOutcome = SlotMachineController.Instance.GetNextSpin();
            spinOutcome.SpinItemTypes.Shuffle();

            yield return StartCoroutine(column1.Spin(GenerateSpinSettings(spinOutcome, 0)));
            yield return StartCoroutine(column2.Spin(GenerateSpinSettings(spinOutcome, 1)));
            yield return StartCoroutine(column3.Spin(GenerateSpinSettings(spinOutcome, 2)));

            TryEmittingCoins(spinOutcome);

            spinButton.interactable = true;
        }

        private ColumnSpinSettings GenerateSpinSettings(SpinOutcomeInfo outcomeInfo, int spinNumber)
        {
            var columnSpinSettings = new ColumnSpinSettings();
            columnSpinSettings.StartingSpinSpeed = startingSpinSpeed;
            columnSpinSettings.StartingSpinDuration = startingSpinDuration;
            columnSpinSettings.ResultItemType = outcomeInfo.SpinItemTypes[spinNumber];

            if(spinNumber == 2 && outcomeInfo.SpinItemTypes[0] == outcomeInfo.SpinItemTypes[1])
            {
                columnSpinSettings.SpinStopDuration = criticalLastSpinStopDurations[Random.Range(0, criticalLastSpinStopDurations.Length)];
                columnSpinSettings.SlowingTweenType = LeanTweenType.easeOutSine;
            }
            else
            {
                columnSpinSettings.SpinStopDuration = fastSpinStopDuration;
                columnSpinSettings.SlowingTweenType = LeanTweenType.easeOutBack;
            }

            return columnSpinSettings;
        }

        private void TryEmittingCoins(SpinOutcomeInfo spinOutcome)
        {
            const int baseParticleCount = 4;
            const int extraParticleCoefficient = 2;

            if (spinOutcome.IsPrizeAvailable())
            {
                coinParticleSystem.Emit(baseParticleCount + extraParticleCoefficient * spinOutcome.PrizeTier);
            }
        }

        private void OnDestroy()
        {
            spinButton.onClick.RemoveAllListeners();
        }
    }
}