using UnityEditor;
using UnityEngine;

namespace ICouldGames.SlotMachine.Editor.Testing
{
    public class SlotMachineTestWindow : EditorWindow
    {
        private string _spinCountText = "100";
        int _spinCount = 100;
        private bool _isTextUnfocusAllowed = true;
        private Vector2 _scroll;
        private string _resultString;

        [MenuItem("Test/SlotMachine spin test")]
        static void ShowWindow()
        {
            SlotMachineTestWindow window = (SlotMachineTestWindow)GetWindow(typeof(SlotMachineTestWindow));
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Slot spin test");
        }

        void OnGUI()
        {
            if (Event.current.character < '0' || Event.current.character > '9')
            {
                Event.current.character = '\0';
            }

            _spinCountText = EditorGUILayout.TextField("Test Spin Count", _spinCountText);
            if (!string.IsNullOrEmpty(_spinCountText))
            {
                if (!int.TryParse(_spinCountText, out _spinCount))
                {
                    _spinCountText = int.MaxValue.ToString();
                    _spinCount = int.MaxValue;
                    GUI.FocusControl(null);
                }
                _isTextUnfocusAllowed = true;
            }

            if (_spinCount <= 0 && _isTextUnfocusAllowed)
            {
                _spinCountText = "";
                _isTextUnfocusAllowed = false;
                GUI.FocusControl(null);
            }

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_spinCountText));
            if (GUILayout.Button("Simulate", GUILayout.Width(100f), GUILayout.Height(30f)))
            {
                _resultString = SlotMachineSpinTester.TestSpinOutcomes(_spinCount);
            }
            EditorGUI.EndDisabledGroup();

            _scroll = EditorGUILayout.BeginScrollView(_scroll, true, true, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            var labelSize = GUI.skin.label.CalcSize(new GUIContent(_resultString));
            EditorGUILayout.SelectableLabel(_resultString, GUILayout.MinWidth(labelSize.x), GUILayout.MinHeight(labelSize.y), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }
}