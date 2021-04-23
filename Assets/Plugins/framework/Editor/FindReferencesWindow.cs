using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Framework
{
    public class FindReferencesWindow : EditorWindow
    {

        enum Tab
        {
            Asset,
            FieldValue,
        }

        private Tab _tab;
        private int _selectedFieldIndex;
        private Object _selectedAsset;
        private bool _includePlugins;

        [MenuItem("Assets/Find References", false)]
        static void ShowWindow()
        {
            FindReferencesWindow window = GetWindow<FindReferencesWindow>(true, "Find References", true);

            if (IsValidAsset(Selection.activeObject) || IsValidSceneAsset(Selection.activeObject))
            {
                window._selectedAsset = Selection.activeObject;
            }

            window.ShowUtility();
            window.CenterInScreen(350, 202);
        }

        private void OnSelectionChange()
        {
            _selectedFieldIndex = 0;
            Repaint();
        }

        void OnGUI()
        {
            _tab = (Tab)GUILayout.Toolbar((int)_tab, Enum.GetNames(typeof(Tab)).Select(s => StringUtils.Titelize(s)).ToArray());

            EditorGUILayout.Space();

            switch (_tab)
            {
                case Tab.Asset: AssetTab(); break;
                case Tab.FieldValue: FieldTab(); break;
            }
        }

        void AssetTab()
        {
            bool isValidAsset = IsValidAsset(_selectedAsset);
            bool isValidSceneAsset = IsValidSceneAsset(_selectedAsset);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            _selectedAsset = EditorGUILayout.ObjectField(GUIContent.none, _selectedAsset, typeof(Object), true);
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            _includePlugins = GUILayout.Toggle(_includePlugins, "Include Plugin Folders");
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(!isValidAsset);

            if (GUILayout.Button("Find References In Prefabs"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInPrefabs(_selectedAsset, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }
            }

            if (GUILayout.Button("Find References In Scriptable Objects"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInScriptableObjects(_selectedAsset, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }
            }

            if (GUILayout.Button("Find References In All Assets"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInAssets(_selectedAsset, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }
            }

            EditorGUI.BeginDisabledGroup(!isValidSceneAsset);

            if (GUILayout.Button("Find References In Current Scene"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInCurrentScene(_selectedAsset);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Obj.name + "\n" + references[i].Path, references[i].Obj);
                }

            }

            if (GUILayout.Button("Find References In All Scenes"))
            {
                List<ReferenceFinder.AssetReference> references = ReferenceFinder.FindReferencesInAllScenes(_selectedAsset, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].Path.Substring(references[i].Path.LastIndexOf('/') + 1) + "\n" + references[i].Path, references[i].Scene);
                }
            }

            if (GUILayout.Button("Count References In All Scenes"))
            {
                List<Tuple<SceneAsset, int>> counts = ReferenceFinder.CountReferencesInScenes(_selectedAsset);
                for (int i = 0; i < counts.Count; i++)
                {
                    Debug.Log(counts[i].First.name + ": " + counts[i].Second, counts[i].First);
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }

        void FieldTab()
        {
            bool isComponent = IsValidComponent(_selectedAsset);
            bool isScriptableObject = IsValidScriptableObject(_selectedAsset);

            Type type = null;
            FieldInfo field = null;

            if (isScriptableObject || isComponent)
            {
                _selectedAsset = EditorGUILayout.ObjectField(new GUIContent("Script"), _selectedAsset, typeof(MonoScript), false);

                type = (_selectedAsset as MonoScript).GetClass();
                FieldInfo[] fields = TypeUtils.GetSerializedFields(type);

                if (fields.Length > 0)
                {
                    _selectedFieldIndex = EditorGUILayout.Popup("Field", _selectedFieldIndex, fields.Select(f => f.Name).ToArray());
                    field = fields[_selectedFieldIndex];
                }
                else
                {
                    EditorGUILayout.LabelField("Field", "Script has no fields");
                }

            }
            else
            {
                _selectedAsset = EditorGUILayout.ObjectField(new GUIContent("Script"), null, typeof(MonoScript), false);
                EditorGUILayout.LabelField("Field", "NO SCRIPT SELECTED");
            }

            _includePlugins = EditorGUILayout.Toggle(new GUIContent("Include Plugin Folders"), _includePlugins);


            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isComponent);

            if (GUILayout.Button("Find Values In Prefabs"))
            {
                List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindReferencesInPrefabs(type, field, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!isScriptableObject);

            if (GUILayout.Button("Find Values In Scriptable Objects"))
            {
                List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindReferencesInScriptableObjects(type, field, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!isComponent);


            if (GUILayout.Button("Find Values In Current Scene"))
            {
                List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindComponentFieldValuesInCurrentScene(type, field);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Obj);
                }

            }

            if (GUILayout.Button("Find Values In All Scenes"))
            {
                List<ReferenceFinder.ValueReference> references = ReferenceFinder.FindComponentFieldValuesInAllScenes(type, field, _includePlugins);
                for (int i = 0; i < references.Count; i++)
                {
                    Debug.Log(references[i].ValueString + "\n" + references[i].Path, references[i].Scene);
                }
            }
            EditorGUI.EndDisabledGroup();

        }


        static bool IsValidAsset(Object obj)
        {
            if (obj == null) return false;
            if (obj is GameObject go && go.IsPrefabAsset()) return true;
            if (obj is DefaultAsset) return false;

            return true;
        }

        static bool IsValidComponent(Object obj)
        {
            if (obj == null) return false;
            if (obj is MonoScript script)
            {
                Type type = script.GetClass();
                if (type != null)
                {
                    if (typeof(Component).IsAssignableFrom(type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsValidScriptableObject(Object obj)
        {
            if (obj == null) return false;
            if (obj is MonoScript script)
            {
                Type type = script.GetClass();
                if (type != null)
                {
                    if (typeof(ScriptableObject).IsAssignableFrom(type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsValidSceneAsset(Object obj)
        {
            if (obj == null) return false;
            if (obj is GameObject go && go.IsPrefabAsset()) return true;
            if (obj is MonoScript) return true;

            return false;
        }
    }
}
