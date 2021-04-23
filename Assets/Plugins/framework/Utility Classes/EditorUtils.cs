

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Object = UnityEngine.Object;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Framework
{
    public static class EditorUtils
    {
#if UNITY_EDITOR
        public const float INDENT_WIDTH = 15;
        public delegate void PropertyCallback(SerializedProperty property);
        public delegate void SceneProcessingAction(SceneAsset scene, string scenePath);

        private static Mesh[] _defaultPrimitiveMeshes;

        public static ReorderableList CreateReorderableList(SerializedProperty property, bool displayFoldout, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            return CreateReorderableList(property, property.displayName, displayFoldout, displayAddButton, displayRemoveButton);
        }

        public static ReorderableList CreateReorderableList(SerializedProperty property, string header, bool displayFoldout, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, displayAddButton, displayRemoveButton);

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                if (!displayFoldout || property.isExpanded)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight), list.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
                }
            };

            list.drawFooterCallback = (rect) =>
            {
                if (!displayFoldout || property.isExpanded)
                {
                    list.footerHeight = 13f;
                    list.draggable = true;
                    ReorderableList.defaultBehaviours.DrawFooter(rect, list);
                }
                else
                {
                    list.footerHeight = 0;
                    list.draggable = false;
                }
            };

            list.drawHeaderCallback = rect =>
            {
                if (displayFoldout)
                {
                    property.isExpanded = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height), property.isExpanded, header, true);
                }
                else
                {
                    EditorGUI.LabelField(rect, header);
                }
            };

            list.elementHeightCallback = (index) =>
            {
                return EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index)) + 2;
            };

            //   if (displayFoldout)
            //  {
            //      list.elementHeightCallback = index =>
            //       {
            //           return property.isExpanded ? list.elementHeight : 0;
            //       };
            //   }

            return list;
        }



        public static void PerformActionInScene(string scenePath, Action action)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                try
                {
                    if (EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single).IsValid())
                    {
                        action();
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
                }
            }
        }

        public static void PerformActionInScenes(string progressBarTitle, SceneProcessingAction action)
        {
            PerformActionInScenes(progressBarTitle, (Predicate<string>)null, action);
        }

        public static void PerformActionInScenes(string progressBarTitle, IList<string> scenePaths, SceneProcessingAction action)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                try
                {
                    for (int i = 0; i < scenePaths.Count; i++)
                    {
                        SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePaths[i]);

                        if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, "(" + (i + 1) + "/" + scenePaths.Count + ") " + scene.name, ((float)i) / scenePaths.Count))
                        {
                            break;
                        }


                        if (EditorSceneManager.OpenScene(scenePaths[i], OpenSceneMode.Single).IsValid())
                        {
                            action(scene, scenePaths[i]);
                        }

                    }

                    EditorUtility.ClearProgressBar();
                }
                catch (Exception e)
                {

                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
                    EditorUtility.ClearProgressBar();
                }
            }
        }


        public static void PerformActionInScenes(string progressBarTitle, Predicate<string> scenePathFilter, SceneProcessingAction action)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

                try
                {
                    string[] scenesGUIDs = AssetDatabase.FindAssets("t:Scene");

                    List<string> scenePaths = new List<string>();
                    for (int i = 0; i < scenesGUIDs.Length; i++)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(scenesGUIDs[i]);
                        if (path.StartsWith("Assets/") && (scenePathFilter == null || scenePathFilter(path)))
                        {
                            scenePaths.Add(path);
                        }
                    }

                    for (int i = 0; i < scenePaths.Count; i++)
                    {

                        SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePaths[i]);

                        if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, "(" + (i + 1) + " / " + scenePaths.Count + ") " + scene.name, ((float)i) / scenePaths.Count))
                        {
                            break;
                        }


                        if (EditorSceneManager.OpenScene(scenePaths[i], OpenSceneMode.Single).IsValid())
                        {
                            action(scene, scenePaths[i]);
                        }

                    }

                    EditorUtility.ClearProgressBar();
                }
                catch (Exception e)
                {

                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
                    EditorUtility.ClearProgressBar();
                }
            }
        }


        public static Material GetDefaultMaterial()
        {
            return AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        }

        public static Mesh GetDefaultPrimitiveMesh(PrimitiveType primitiveType)
        {
            if (_defaultPrimitiveMeshes == null)
            {
                _defaultPrimitiveMeshes = new Mesh[EnumUtils.GetCount<PrimitiveType>()];
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath("Library/unity default resources");

                for (int i = 0; i < objects.Length; i++)
                {
                    for (int j = 0; j < _defaultPrimitiveMeshes.Length; j++)
                    {
                        if (objects[i].name == ((PrimitiveType)j).ToString())
                        {
                            _defaultPrimitiveMeshes[j] = (Mesh)objects[i];
                        }
                    }
                }
            }

            return _defaultPrimitiveMeshes[(int)primitiveType];
        }

        public static string GetLocalPath(string absolouteAssetPath)
        {
            string path = SanitizeFilePath(absolouteAssetPath);
            return path.Substring(path.IndexOf("Assets/"));
        }

        public static string GetResourcePath(string absolouteAssetPath)
        {
            string path = SanitizeFilePath(absolouteAssetPath);

            path = path.Substring(path.IndexOf("Assets/Resources/") + 17);
            int dotIndex = path.LastIndexOf('.');

            return dotIndex >= 0 ? path.Substring(0, dotIndex) : path;
        }

        public static string GetAbsolutePath(string localAssetPath)
        {
            string path = SanitizeFilePath(localAssetPath);

            if (path.StartsWith("Assets/"))
            {
                path = path.Substring(path.IndexOf("/Assets/") + 8);
            }

            return Application.dataPath + "/" + path;
        }

        public static string SanitizeFilePath(string path)
        {
            return path.Replace('\\', '/').Trim('/');
        }

        public static T[] GetAssetsInFolder<T>(string folderPath, bool includeChildDirectories) where T : Object
        {
            List<T> assets = new List<T>();

            if (AssetDatabase.IsValidFolder(folderPath))
            {
                string[] paths = Directory.GetFiles(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + folderPath, "*", includeChildDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                for (var i = 0; i < paths.Length; i++)
                {
                    string path = paths[i].Substring(Application.dataPath.Length - 6);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                    if (asset != null)
                    {
                        assets.Add(asset);
                    }
                }
            }

            return assets.ToArray();
        }

        public static T[] GetAllAssets<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets.ToArray();
        }

        public static void CreateAssetFolders(string fileOrFolderPath)
        {
            if (fileOrFolderPath == "Assets") return;

            fileOrFolderPath = SanitizeFilePath(fileOrFolderPath);

            if (fileOrFolderPath.Contains('.'))
            {
                fileOrFolderPath = fileOrFolderPath.Substring(0, fileOrFolderPath.LastIndexOf('/'));
            }

            int numFolders = fileOrFolderPath.CountOccurencesOf('/');
            string lastPath = "";

            for (int i = 0; i < numFolders; i++)
            {
                string path = fileOrFolderPath.Substring(0, fileOrFolderPath.NthIndexOf(i + 1, '/'));
                string folderName = path.Substring(path.LastIndexOf('/') + 1);

                if (i != 0 || folderName != "Assets")
                {
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        AssetDatabase.CreateFolder(lastPath, folderName);
                    }
                }

                lastPath = path;
            }

            if (!AssetDatabase.IsValidFolder(fileOrFolderPath))
            {
                AssetDatabase.CreateFolder(lastPath, fileOrFolderPath.Substring(fileOrFolderPath.LastIndexOf('/') + 1));
            }
        }

        public static void ForEachChildProperty(SerializedProperty property, PropertyCallback propertyCallback)
        {
            SerializedProperty endProperty = property.GetEndProperty(false);

            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(property, endProperty))
                {
                    return;
                }

                enterChildren = false;
                propertyCallback(property);
            }
        }

        public static void ForEachProperty(SerializedObject serializedObject, PropertyCallback propertyCallback)
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    propertyCallback(property);
                }
                while (property.NextVisible(false));
            }

        }

        public static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            if (parent != null)
            {
                go.transform.parent = parent;
            }
            go.transform.position = position;
            go.transform.rotation = rotation;

            return go;
        }


        public static Quaternion QuaternionField(GUIContent label, Quaternion quaternion, params GUILayoutOption[] options)
        {
            return QuaternionField(EditorGUILayout.GetControlRect(options), label, quaternion);
        }

        public static Quaternion QuaternionField(Rect rect, GUIContent label, Quaternion quaternion)
        {
            return Quaternion.Euler(EditorGUI.Vector3Field(rect, label, quaternion.eulerAngles));
        }

        public static void QuaternionPropertyField(Rect rect, GUIContent label, SerializedProperty property)
        {
            EditorGUI.BeginProperty(rect, label, property);
            property.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(rect, label, property.quaternionValue.eulerAngles));
            EditorGUI.EndProperty();
        }

        public static T PropertyField<T>(GUIContent label, object value, params GUILayoutOption[] options)
        {
            return (T)PropertyField(typeof(T), label, value, options);
        }

        public static object PropertyField(Type type, GUIContent label, object value, params GUILayoutOption[] options)
        {

            // generic
            // object
            //layermask
            //enum
            //array



            if (type.IsArray)
            {

            }

            if (type == typeof(int)) return EditorGUILayout.IntField(label, (int)value, options);
            if (type == typeof(bool)) return EditorGUILayout.Toggle(label, (bool)value, options);
            if (type == typeof(string)) return EditorGUILayout.TextField(label, (string)value, options);
            if (type == typeof(float)) return EditorGUILayout.FloatField(label, (float)value, options);
            if (type == typeof(char)) return EditorGUILayout.IntField(label, (int)value, options);
            if (type == typeof(Color)) return EditorGUILayout.ColorField(label, (Color)value, options);

            if (type == typeof(Vector2)) return EditorGUILayout.Vector2Field(label, (Vector2)value, options);
            if (type == typeof(Vector3)) return EditorGUILayout.Vector3Field(label, (Vector3)value, options);
            if (type == typeof(Vector4)) return EditorGUILayout.Vector4Field(label, (Vector4)value, options);

            if (type == typeof(Vector2Int)) return EditorGUILayout.Vector2IntField(label, (Vector2Int)value, options);
            if (type == typeof(Vector3Int)) return EditorGUILayout.Vector3IntField(label, (Vector3Int)value, options);

            if (type == typeof(Rect)) return EditorGUILayout.RectField(label, (Rect)value, options);
            if (type == typeof(RectInt)) return EditorGUILayout.RectIntField(label, (RectInt)value, options);

            if (type == typeof(Bounds)) return EditorGUILayout.BoundsField(label, (Bounds)value, options);
            if (type == typeof(BoundsInt)) return EditorGUILayout.BoundsIntField(label, (BoundsInt)value, options);

            if (type == typeof(AnimationCurve)) return EditorGUILayout.CurveField(label, (AnimationCurve)value, options);
            if (type == typeof(Gradient)) return EditorGUILayout.GradientField(label, (Gradient)value, options);
            if (type == typeof(Quaternion)) return QuaternionField(label, (Quaternion)value, options);


            return null;
        }


        /// <summary>
        /// Checks wether a file with a certain name exists in the Assets directory, outs the path if it does.
        /// </summary>
        /// <param name="filename">The file name to check</param>
        /// <param name="filepath">The local file path (if a match is found, otherwise null)</param>
        /// <returns>Whether or not a file with that name was found in the Assets directory</returns>
        public static bool GetAssetFilePath(string filename, out string filepath)
        {
            string[] files = Directory.GetFiles(Application.dataPath, filename, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                filepath = files[0].Replace('\\', '/');
                filepath = "Assets/" + filepath.Replace(Application.dataPath + "/", String.Empty);
                return true;
            }
            filepath = null;
            return false;
        }

        public static bool AssetFileExists(string filename)
        {
            return Directory.GetFiles(Application.dataPath, filename, SearchOption.AllDirectories).Length > 0;
        }

        /// <summary>
        /// Creates/changes a text file and imports it.
        /// </summary>
        /// <param name="path">The path of the file to create</param>
        /// <param name="text">The contents of the text file</param>
        public static void CreateTextFile(string localPath, string text)
        {
            bool fileExists = AssetFileExists(localPath.Substring(localPath.LastIndexOf("/") + 1));

            File.WriteAllText(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/" + localPath, text.Replace('\r', '\n').Replace("\n", "\r\n"));

            AssetDatabase.Refresh();
            UnityEngine.Debug.Log((fileExists ? "Updating" : "Creating") + " file: " + localPath, AssetDatabase.LoadMainAssetAtPath(localPath));
        }

        public static Component MoveComponent(Component component, GameObject destinationObject)
        {
            if (component == null) return null;

            Component newComponent = destinationObject.AddComponent(component.GetType());
            if (newComponent != null)
            {
                EditorUtility.CopySerialized(component, newComponent);
                component.SmartDestroy();
                return newComponent;
            }

            return null;
        }




#endif
    }
}

