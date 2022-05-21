using UnityEditor;
using UnityEngine;

namespace ICouldGames.Data.Editor
{
    public static class PlayerPrefsModification
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            EditorUtility.DisplayDialog("Info", "PlayerPrefs are successfully cleared.", "OK");
        }
    }
}