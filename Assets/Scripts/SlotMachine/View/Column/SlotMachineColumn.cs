using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICouldGames.SlotMachine.Controller;
using ICouldGames.SlotMachine.Spin.Item;
using ICouldGames.SlotMachine.View.Column.Settings;
using UnityEngine;

namespace ICouldGames.SlotMachine.View.Column
{
    public class SlotMachineColumn : MonoBehaviour
    {
        [SerializeField] private Transform spawnBorderPoint;
        [SerializeField] private Transform despawnBorderPoint; // If Springy ease tweens make items disappear, position despawnBorderPoint a little bit lower
        [SerializeField] private Transform spinItemsMoveContainer;
        [SerializeField] private Transform spawnItemsRoot;
        [SerializeField] private Transform tweenReachTransform;
        [SerializeField] private SlotMachineSpinItem spinItemPrefab;

        private Queue<SlotMachineSpinItem> _activeSpinItems = new();
        private Stack<SlotMachineSpinItem> _cachedSpinItems = new();

        private IEnumerator Start()
        {
            yield return new WaitUntil(IsDependenciesReady);
            FillInitialActiveSpinItems();
        }

        private void FillInitialActiveSpinItems()
        {
            var spinItems = GetComponentsInChildren<SlotMachineSpinItem>();

            foreach (var spinItem in spinItems)
            {
                spinItem.InitWithRandomSpinImages();
            }

            // Sort from bottom to top
            Array.Sort(spinItems, (c1, c2) =>
            {
                var spawnPosition = spawnBorderPoint.position;
                return (c2.transform.position - spawnPosition).sqrMagnitude
                    .CompareTo((c1.transform.position - spawnPosition).sqrMagnitude);
            });

            foreach (var spinItem in spinItems)
            {
                _activeSpinItems.Enqueue(spinItem);
            }
        }

        public IEnumerator Spin(ColumnSpinSettings spinSettings)
        {
            var itemSpacingVector = _activeSpinItems.ElementAt(0).transform.position - _activeSpinItems.ElementAt(1).transform.position;
            NormalizeStartingSpeed(ref spinSettings, itemSpacingVector.magnitude);

            // First spin
            var firstTweenEnded = false;
            var firstTweenEndPos = spinItemsMoveContainer.position
                                   + (spinSettings.StartingSpinSpeed * spinSettings.StartingSpinDuration * itemSpacingVector.normalized);
            tweenReachTransform.position = firstTweenEndPos;
            LeanTween.move(spinItemsMoveContainer.gameObject, tweenReachTransform, spinSettings.StartingSpinDuration)
                .setEase(LeanTweenType.linear)
                .setOnUpdate(CheckSpawns)
                .setOnComplete(() => firstTweenEnded = true);
            yield return new WaitUntil(() => firstTweenEnded);

            // Second spin
            var secondTweenEnded = false;
            var tweenDistanceInSpacing = Mathf.FloorToInt(spinSettings.StartingSpinSpeed * spinSettings.SpinStopDuration /
                                                     2f / itemSpacingVector.magnitude);
            tweenDistanceInSpacing = Mathf.Clamp(tweenDistanceInSpacing, 2, int.MaxValue);
            var tweenDistanceVector = tweenDistanceInSpacing * itemSpacingVector;
            tweenReachTransform.position += tweenDistanceVector;
            LeanTween.move(spinItemsMoveContainer.gameObject, tweenReachTransform, spinSettings.SpinStopDuration)
                .setEase(spinSettings.SlowingTweenType)
                .setOnUpdate(CheckSpawns)
                .setOnComplete(() => secondTweenEnded = true);
            yield return new WaitUntil(() => secondTweenEnded);

            Reset();
        }

        private void CheckSpawns(float tweenMoved)
        {
            var itemSpacingVector = _activeSpinItems.ElementAt(0).transform.position - _activeSpinItems.ElementAt(1).transform.position;

            // Despawn spin items
            while (_activeSpinItems.Any())
            {
                var oldestItem = _activeSpinItems.Peek();
                var itemToDespawnPosVector = despawnBorderPoint.transform.position - oldestItem.transform.position;
                if (Vector3.Dot(itemSpacingVector, itemToDespawnPosVector) < 0f)
                {
                    _cachedSpinItems.Push(_activeSpinItems.Dequeue());
                }
                else
                {
                    break;
                }
            }

            // Calculate spawn decisions
            bool isSpawnNeeded = false;
            Vector3 firstSpawnPoint = Vector3.zero;
            if (!_activeSpinItems.Any())
            {
                var lastCachedItem = _cachedSpinItems.Peek();
                var lastCachedPos = lastCachedItem.transform.position;
                var despawnPos = despawnBorderPoint.position;
                firstSpawnPoint = lastCachedPos + Mathf.Ceil(Vector3.Distance(lastCachedPos, despawnPos) / itemSpacingVector.magnitude) * (-itemSpacingVector);
                isSpawnNeeded = true;
            }
            else
            {
                var youngestItemPos = _activeSpinItems.Last().transform.position;
                var youngestItemToSpawnBorderVector = youngestItemPos - spawnBorderPoint.position;
                if (youngestItemToSpawnBorderVector.sqrMagnitude > itemSpacingVector.sqrMagnitude)
                {
                    firstSpawnPoint = youngestItemPos - itemSpacingVector;
                    isSpawnNeeded = true;
                }
            }

            // Spawn spin items
            if (isSpawnNeeded)
            {
                var itemSpawnPoint = firstSpawnPoint;
                while (Vector3.Dot(itemSpacingVector, itemSpawnPoint - spawnBorderPoint.position) > 0f)
                {
                    var spawnItem = GetSpawnItem();
                    spawnItem.InitWithRandomSpinImages();
                    spawnItem.transform.position = itemSpawnPoint;
                    _activeSpinItems.Enqueue(spawnItem);
                    itemSpawnPoint -= itemSpacingVector;
                }
            }
        }

        private SlotMachineSpinItem GetSpawnItem()
        {
            if (_cachedSpinItems.TryPop(out var spinItem))
            {
                return spinItem;
            }

            return Instantiate(spinItemPrefab, spinItemsMoveContainer);
        }

        public void NormalizeStartingSpeed(ref ColumnSpinSettings spinSettings, float spinItemSpacing)
        {
            spinSettings.StartingSpinSpeed = Mathf.Ceil(spinSettings.StartingSpinSpeed * spinSettings.StartingSpinDuration / spinItemSpacing)
                * spinItemSpacing / spinSettings.StartingSpinDuration;
        }

        private void Reset()
        {
            var spinItems = spinItemsMoveContainer.GetComponentsInChildren<SlotMachineSpinItem>();
            foreach (var spinItem in spinItems)
            {
                spinItem.transform.SetParent(spawnItemsRoot);
            }

            spinItemsMoveContainer.position = spawnItemsRoot.position;

            foreach (var spinItem in spinItems)
            {
                spinItem.transform.SetParent(spinItemsMoveContainer);
            }
        }

        private bool IsDependenciesReady()
        {
            return SlotMachineController.Instance.IsReady
                   && SpinItemImageProvider.Instance.IsReady;
        }
    }
}