using UnityEngine;

namespace ICouldGames.AppQuality
{
    public class AppQualityController : MonoBehaviour
    {
        public void Start()
        {
            Application.targetFrameRate = 120;
        }
    }
}