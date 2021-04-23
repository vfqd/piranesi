using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

/*
CHILD CLASS USAGE:

#if UNITY_EDITOR
[UnityEditor.SettingsProvider]
public static UnityEditor.SettingsProvider CreateSettingsProvider()
{
    return CreateSettingsProviderInternal();
}
#endif
*/

namespace Framework
{
    public class ProjectSettingsObject<T> : ScriptableObject where T : ScriptableObject
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<T>(ASSET_PATH);

#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        FileUtils.CreateAssetFolders(ASSET_PATH);
                        _instance = CreateInstance<T>();
                        AssetDatabase.CreateAsset(_instance, ASSET_PATH);
                        AssetDatabase.SaveAssets();
                    }
#else
             Debug.LogError("Settings asset for "+ typeof(T).Name+ " not found at: "+ASSET_PATH);   
#endif
                }

                return _instance;
            }
        }

        private static T _instance;

        private static readonly string CATEGORY_NAME = StringUtils.Titelize(typeof(T).Name).RemoveFromEnd("Settings").Trim();
        private static readonly string ASSET_PATH = "Assets/Resources/" + CATEGORY_NAME + " Settings.asset";

#if UNITY_EDITOR
        protected static SettingsProvider CreateSettingsProviderInternal()
        {

            return new SettingsProvider("Project/" + CATEGORY_NAME, SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    SerializedObject serializedObject = new SerializedObject(Instance);

                    EditorGUI.BeginChangeCheck();

                    serializedObject.Update();
                    SerializedProperty property = serializedObject.GetIterator();
                    if (property.NextVisible(true))
                    {
                        do
                        {
                            if (property.name != "m_Script")
                            {
                                EditorGUILayout.PropertyField(property, true);
                            }
                        }
                        while (property.NextVisible(false));
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }


                },

                keywords = GetKeywords()

            };

            IEnumerable<string> GetKeywords()
            {
                yield return CATEGORY_NAME;

                SerializedObject serializedObject = new SerializedObject(Instance);

                serializedObject.Update();
                SerializedProperty property = serializedObject.GetIterator();
                if (property.NextVisible(true))
                {
                    do
                    {
                        if (property.propertyPath.CountOccurencesOf('.') < 2)
                        {
                            yield return property.displayName;
                        }
                    }
                    while (property.NextVisible(true));
                }

            }

        }
#endif

    }
}
