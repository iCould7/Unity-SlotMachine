using ICouldGames.ScreenData;
using UnityEngine;

namespace ICouldGames.CameraControls
{
    [ExecuteAlways]
    public class CameraSizeCalculator : MonoBehaviour
    {
        private const float TARGET_ASPECT = 9f / 16f;
        private const float TARGET_SIZE = 7.5f;

        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            CalculateSize();
        }

        private void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ScreenOrientationChangeHandler.Instance.Listener.AddListener(CalculateSize);
            ScreenSizeChangeListener.Instance.Listener.AddListener(CalculateSize);
        }

        private void CalculateSize()
        {
            _cam.orthographicSize = Mathf.Clamp((TARGET_ASPECT / _cam.aspect) * TARGET_SIZE, 5f, float.MaxValue);
        }

        private void OnDestroy()
        {
            if (ScreenOrientationChangeHandler.Instance != null)
            {
                ScreenOrientationChangeHandler.Instance.Listener.RemoveListener(CalculateSize);
            }

            if (ScreenSizeChangeListener.Instance != null)
            {
                ScreenSizeChangeListener.Instance.Listener.RemoveListener(CalculateSize);
            }
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            _cam = GetComponent<Camera>();
        }

        private void Update()
        {
            CalculateSize();
        }
#endif
    }
}