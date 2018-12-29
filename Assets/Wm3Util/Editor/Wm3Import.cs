using UnityEngine;
using UnityEditor;

namespace Wm3Util
{
    public class Wm3Import : EditorWindow
    {
        private static TextAsset s_levelData; 
        private static Material[] s_defaultMaterials = new Material[4];
        private static string[] s_materialLabels = {"Solid", "Overlay", "Sky", "Sprite" };

        [MenuItem("Window/WM3 Importer")]
        private static void OpenWindow()
        {
            Wm3Import window = GetWindow<Wm3Import>();
            window.titleContent = new GUIContent("WM3 Importer");
            window.hideFlags = HideFlags.HideAndDontSave;
            window.Load();
        }

        private void OnGUI()
        {
            DrawLevel();
            DrawMaterials();
            DrawButtons();
        }

        private void OnEnable()
        {
            Load();
        }

        private void OnDestroy()
        {
            Save();
        }

        private void DrawLevel()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Level Data", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            GUILayoutOption[] options1 = { GUILayout.MinWidth(100.0f) };
            GUILayoutOption[] options2 = { GUILayout.MinWidth(150.0f) };
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("WM3 File (.bytes)", options1);
            s_levelData = EditorGUILayout.ObjectField(s_levelData, typeof(TextAsset), false, options2) as TextAsset;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMaterials()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Default Materials", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            GUILayoutOption[] options1 = { GUILayout.MinWidth(100.0f) };
            GUILayoutOption[] options2 = { GUILayout.MinWidth(150.0f) };
            for (int i = 0; i < s_defaultMaterials.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(s_materialLabels[i], options1);
                s_defaultMaterials[i] = EditorGUILayout.ObjectField(s_defaultMaterials[i], typeof(Material), false, options2) as Material;
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawButtons()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayoutOption[] options = { GUILayout.MinWidth(80.0f), GUILayout.MaxWidth(80.0f) };

            Rect buttonClose = EditorGUILayout.GetControlRect(options);
            if (GUI.Button(buttonClose, "Close"))
            {
                Close();
            }

            Rect buttonClear = EditorGUILayout.GetControlRect(options);
            if (GUI.Button(buttonClear, "Clear"))
            {
                Clear();
            }

            EditorGUILayout.Space();

            Rect buttonOk = EditorGUILayout.GetControlRect(options);
            if (GUI.Button(buttonOk, "Import"))
            {
                Run();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void Init()
        {
            for (int i = 0; i < s_defaultMaterials.Length; i++)
            {
                s_defaultMaterials[i] = null;
            }
            s_levelData = null;
        }

        private void Clear()
        {
            DeleteSaveState();
            Init();
        }

        private void Run()
        {
            Save();

            bool proceed = true;
            if (s_levelData == null)
            {
                proceed = false;
                EditorUtility.DisplayDialog("Error", "No level data file configured.", "Ok");
            }

            if (proceed && s_defaultMaterials[0] == null)
            {
                proceed = false;
                EditorUtility.DisplayDialog("Error", "No solid material configured.", "Ok");
            }

            if (proceed)
            {
                Wm3Loader loader = new Wm3Loader();
                loader.LoadWM3(s_levelData);
                Wm3Data data = loader.Data;
                LevelBuilder builder = new LevelBuilder(s_defaultMaterials);
                builder.Construct(data);
            }
        }

        private void DeleteSaveState()
        {
            string prefId;
            for (int i = 0; i < s_defaultMaterials.Length; i++)
            {
                prefId = PlayerSettings.productName + "_m" + i;
                if (EditorPrefs.HasKey(prefId))
                {
                    EditorPrefs.DeleteKey(prefId);
                }
            }

            prefId = PlayerSettings.productName + "_l";
            if (EditorPrefs.HasKey(prefId))
            {
                EditorPrefs.DeleteKey(prefId);
            }
        }

        private void Save()
        {
            DeleteSaveState();
            string prefId;
            for (int i = 0; i < s_defaultMaterials.Length; i++)
            {
                if (s_defaultMaterials[i] != null)
                {
                    prefId = PlayerSettings.productName + "_m" + i;
                    EditorPrefs.SetString(prefId, s_defaultMaterials[i].name);
                }
            }

            if (s_levelData != null)
            {
                prefId = PlayerSettings.productName + "_l";
                EditorPrefs.SetString(prefId, s_levelData.name);
            }

        }

        private void Load()
        {
            string prefId;
            for (int i = 0; i < s_defaultMaterials.Length; i++)
            {
                prefId = PlayerSettings.productName + "_m" + i;
                if (EditorPrefs.HasKey(prefId))
                {
                    string name = EditorPrefs.GetString(prefId);
                    string[] matches = AssetDatabase.FindAssets(name + " t:Material");
                    if ((matches.Length > 0) && !string.IsNullOrEmpty(matches[0]))
                    {
                        string path = PathForMatches(matches);
                        s_defaultMaterials[i] = AssetDatabase.LoadAssetAtPath<Material>(path);
                    }
                }
            }

            prefId = PlayerSettings.productName + "_l";
            if (EditorPrefs.HasKey(prefId))
            {
                string name = EditorPrefs.GetString(prefId);
                string[] matches = AssetDatabase.FindAssets(name + " t:TextAsset");
                if ((matches.Length > 0) && !string.IsNullOrEmpty(matches[0]))
                {
                    string path = PathForMatches(matches);
                    s_levelData = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                }
            }
        }

        private string PathForMatches(string[] matches)
        {
            string path = AssetDatabase.GUIDToAssetPath(matches[0]);

            //ugly patch to skip search in editor folder
            int skipCounter = 1;
            while (path.Contains("Editor") && matches.Length > skipCounter)
            {
                path = AssetDatabase.GUIDToAssetPath(matches[skipCounter]);
                skipCounter++;
            }

            return path;
        }
    }
}
