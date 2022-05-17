using UnityEngine;

namespace ICouldGames.SlotMachine.View
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
    }
}