using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICouldGames.Dependency.Singly;
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
        private bool _firstTweenEnded = false;
        private bool _secondTweenEnded = false;
        private int _resultOffsetFromMid = 0;
        private int _nextSpawnOffsetFromMid = 0;
        private ColumnSpinSettings _spinSettings;
        private SpinItemImageProvider _spinItemImageProvider;

        private void Awake()
        {
            _spinItemImageProvider = SingletonProvider.Instance.Get<SpinItemImageProvider>();
        }

        private void Start()
        {
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
            _spinSettings = spinSettings;
            var itemSpacingVector = _activeSpinItems.ElementAt(0).transform.position - _activeSpinItems.ElementAt(1).transform.position;
            NormalizeStartingSpeed(ref spinSettings, itemSpacingVector.magnitude);

            // First tween
            SetBlurredEveryItem();
            var firstTweenEndPos = spinItemsMoveContainer.position
                                   + (spinSettings.StartingSpinSpeed * spinSettings.StartingSpinDuration * itemSpacingVector.normalized);
            tweenReachTransform.position = firstTweenEndPos;
            LeanTween.move(spinItemsMoveContainer.gameObject, tweenReachTransform, spinSettings.StartingSpinDuration)
                .setEase(LeanTweenType.linear)
                .setOnUpdate((float _) =>
                {
                    CheckSpawns();
                    SetBlurredEveryItem();
                })
                .setOnComplete(() => _firstTweenEnded = true);
            yield return new WaitUntil(() => _firstTweenEnded);

            // Second tween
            var tweenDistanceInSpacing = Mathf.FloorToInt(spinSettings.StartingSpinSpeed * spinSettings.SpinStopDuration /
                                                          2f / itemSpacingVector.magnitude);
            tweenDistanceInSpacing = Mathf.Clamp(tweenDistanceInSpacing, 2, int.MaxValue);
            _resultOffsetFromMid = tweenDistanceInSpacing;
            CheckActiveItemsForResult(itemSpacingVector);
            var tweenDistanceVector = tweenDistanceInSpacing * itemSpacingVector;
            tweenReachTransform.position += tweenDistanceVector;
            LeanTween.move(spinItemsMoveContainer.gameObject, tweenReachTransform, spinSettings.SpinStopDuration)
                .setEase(spinSettings.SlowingTweenType)
                .setOnUpdate((float _) => CheckSpawns())
                .setOnComplete(() => _secondTweenEnded = true);
            LeanTween.value(1f, 0f, spinSettings.SpinStopDuration)
                .setOnUpdate((x) =>
                {
                    foreach (var item in _activeSpinItems)
                    {
                        item.ActivateMixedMode();
                        item.SetTransitionAlphas(x);
                    }
                })
                .setOnComplete(() =>
                {
                    foreach (var item in _activeSpinItems)
                    {
                        item.ResetAlphas();
                        item.ActivateCleanMode();
                    }
                })
                .setEase(spinSettings.SlowingTweenType);

            yield return new WaitUntil(() => _secondTweenEnded);

            Reset();
        }

        private void SetBlurredEveryItem()
        {
            foreach (var item in _activeSpinItems)
            {
                item.ActivateBlurredMode();
            }
        }

        private void CheckSpawns()
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
                var offsetFromLastCachedPointInSpacing = Mathf.CeilToInt(Vector3.Distance(lastCachedPos, despawnPos) / itemSpacingVector.magnitude);
                firstSpawnPoint = lastCachedPos + offsetFromLastCachedPointInSpacing * (-itemSpacingVector);
                isSpawnNeeded = true;
                if (_firstTweenEnded)
                {
                    _nextSpawnOffsetFromMid += offsetFromLastCachedPointInSpacing - 1;
                }
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
                    if (_firstTweenEnded)
                    {
                        if (_resultOffsetFromMid == _nextSpawnOffsetFromMid)
                        {
                            spawnItem.Init(_spinItemImageProvider.GetBlurredImage(_spinSettings.ResultItemType),
                                _spinItemImageProvider.GetCleanImage(_spinSettings.ResultItemType));
                        }
                        _nextSpawnOffsetFromMid++;
                    }
                }
            }
        }

        private int CalculateMidOffset(SlotMachineSpinItem item, Vector3 itemSpacingVector)
        {
            var itemToMidVector = spawnItemsRoot.transform.position - item.transform.position;
            var tolerance = itemSpacingVector.magnitude / 2f;

            int offsetSign;
            if (Vector3.Dot(itemToMidVector, itemSpacingVector) > 0f)
            {
                offsetSign = 1;
            }
            else
            {
                offsetSign = -1;
            }

            return Mathf.FloorToInt((itemToMidVector.magnitude + tolerance) / itemSpacingVector.magnitude) * offsetSign;
        }

        private void CheckActiveItemsForResult(Vector3 itemSpacingVector)
        {
            foreach (var spinItem in _activeSpinItems)
            {
                var midOffset = CalculateMidOffset(spinItem, itemSpacingVector);
                if (midOffset == _resultOffsetFromMid)
                {
                    spinItem.Init(_spinItemImageProvider.GetBlurredImage(_spinSettings.ResultItemType),
                        _spinItemImageProvider.GetCleanImage(_spinSettings.ResultItemType));
                }

                if (midOffset >= 0)
                {
                    _nextSpawnOffsetFromMid++;
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

            _firstTweenEnded = false;
            _secondTweenEnded = false;
            _resultOffsetFromMid = 0;
            _nextSpawnOffsetFromMid = 0;
        }
    }
}