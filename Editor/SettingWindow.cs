using UnityEngine;
using UnityEditor;

namespace SpeechToText
{
    public class SettingWindow : EditorWindow
    {
        [MenuItem("Tools/SpeechToText/Setting Window")]
        public static void Open()
        {
            EditorWindow.GetWindow<SettingWindow>();
        }

        private string _apiKey;

        private void OnEnable()
        {
            _apiKey = PlayerPrefs.GetString(SpeechToTextController.ApiKeySaveWord);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            _apiKey = EditorGUILayout.TextField("Google Api Key", _apiKey);

            if (GUILayout.Button("Save"))
            {
                PlayerPrefs.SetString(SpeechToTextController.ApiKeySaveWord, _apiKey);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
