using ICouldGames.SlotMachine.Spin.Item;
using UnityEngine;

namespace ICouldGames.SlotMachine.View.Column
{
    public class SlotMachineSpinItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer blurredItem;
        [SerializeField] private SpriteRenderer cleanItem;

        private void Awake()
        {
            ActivateCleanMode();
        }

        public void Init(Sprite blurredImage, Sprite cleanImage)
        {
            blurredItem.sprite = blurredImage;
            cleanItem.sprite = cleanImage;
        }

        public void ActivateBlurredMode()
        {
            blurredItem.gameObject.SetActive(true);
            cleanItem.gameObject.SetActive(false);
        }

        public void ActivateCleanMode()
        {
            cleanItem.gameObject.SetActive(true);
            blurredItem.gameObject.SetActive(false);
        }

        public void InitWithRandomSpinImages()
        {
            var randomItemType = SpinItemTypeGenerator.GetRandom();
            Init(SpinItemImageProvider.Instance.GetBlurredImage(randomItemType),
                SpinItemImageProvider.Instance.GetCleanImage(randomItemType));
        }
    }
}