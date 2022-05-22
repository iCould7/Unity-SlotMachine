using UnityEngine;
using UnityEngine.Events;

namespace ICouldGames.ScreenData
{
    public class ScreenOrientationChangeHandler : MonoBehaviour
    {
        public static ScreenOrientationChangeHandler Instance;

        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

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
            if (Screen.orientation != _lastOrientation)
            {
                Listener.Invoke();
            }

            _lastOrientation = Screen.orientation;
        }
    }
}