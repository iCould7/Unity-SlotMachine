using UnityEngine;
using UnityEngine.Events;

namespace ICouldGames.ScreenData
{
    public class ScreenSizeChangeListener : MonoBehaviour
    {
        public static ScreenSizeChangeListener Instance;

        private Vector2Int _lastScreenSize = new(0, 0);

        public UnityEvent Listener { get; } = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            if (Screen.width != _lastScreenSize.x
                || Screen.height != _lastScreenSize.y)
            {
                Listener.Invoke();
            }

            _lastScreenSize.x = Screen.width;
            _lastScreenSize.y = Screen.height;
        }
    }
}