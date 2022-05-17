using System;
using System.Collections.Generic;
using UnityEngine;

namespace ICouldGames.SlotMachine.Spin.Item
{
    public class SpinItemImageProvider : MonoBehaviour
    {
        public static SpinItemImageProvider Instance { get; private set; }

        public bool IsReady { get; private set; } = false;

        private Dictionary<SpinItemType, Sprite> _blurredSpinImages = new();
        private Dictionary<SpinItemType, Sprite> _cleanSpinImages = new();

        private const string SPIN_ITEM_IMAGES_FOLDER = "SpinItemImages/";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Init();
            IsReady = true;
        }

        private void Init()
        {
            foreach (var spinItemType in Enum.GetValues(typeof(SpinItemType)))
            {
                var itemId = (int) spinItemType;
                var itemType = (SpinItemType) itemId;

                _blurredSpinImages[itemType] = Resources.Load<Sprite>(SPIN_ITEM_IMAGES_FOLDER + $"SpinItem_Blurred_{itemId.ToString()}");
                _cleanSpinImages[itemType] = Resources.Load<Sprite>(SPIN_ITEM_IMAGES_FOLDER + $"SpinItem_Clean_{itemId.ToString()}");
            }
        }

        public Sprite GetCleanImage(SpinItemType itemType)
        {
            return _cleanSpinImages[itemType];
        }

        public Sprite GetBlurredImage(SpinItemType itemType)
        {
            return _blurredSpinImages[itemType];
        }
    }
}