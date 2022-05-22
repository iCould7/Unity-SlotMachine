using ICouldGames.Dependency.Singly;
using ICouldGames.SlotMachine.Spin.Item;
using UnityEngine;

namespace ICouldGames.SlotMachine.View.Column
{
    public class SlotMachineSpinItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer blurredItem;
        [SerializeField] private SpriteRenderer cleanItem;

        private SpinItemImageProvider _spinItemImageProvider;

        private void Awake()
        {
            _spinItemImageProvider = SingletonProvider.Instance.Get<SpinItemImageProvider>();
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

        public void ActivateMixedMode()
        {
            cleanItem.gameObject.SetActive(true);
            blurredItem.gameObject.SetActive(true);
        }

        public void SetTransitionAlphas(float blurAlpha)
        {
            var blurredItemColor = blurredItem.color;
            blurredItemColor.a = blurAlpha;
            blurredItem.color = blurredItemColor;

            var cleanItemColor = cleanItem.color;
            cleanItemColor.a = 1f - blurAlpha;
            cleanItem.color = cleanItemColor;
        }

        public void ResetAlphas()
        {
            var blurredItemColor = blurredItem.color;
            blurredItemColor.a = 1f;
            blurredItem.color = blurredItemColor;

            var cleanItemColor = cleanItem.color;
            cleanItemColor.a = 1f;
            cleanItem.color = cleanItemColor;
        }

        public void InitWithRandomSpinImages()
        {
            var randomItemType = SpinItemTypeGenerator.GetRandom();
            Init(_spinItemImageProvider.GetBlurredImage(randomItemType), _spinItemImageProvider.GetCleanImage(randomItemType));
        }
    }
}