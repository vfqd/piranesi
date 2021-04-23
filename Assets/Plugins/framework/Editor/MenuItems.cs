using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Framework
{
    public static class MenuItems
    {

        [MenuItem("File/Open Player Log Folder")]
        public static void OpenPlayerLogFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName, "Player.log"));
        }

        [MenuItem("File/Open Editor Log Folder")]
        public static void OpenEditorLogFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Environment.GetEnvironmentVariable("AppData"), "..", "Local/Unity/Editor/Editor.log"));
        }


        [MenuItem("File/Open Project Folder")]
        public static void OpenProjectFolder()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
        }

        [MenuItem("CONTEXT/MonoScript/Create Custom Inspector")]
        private static void CreateCustomInspector(MenuCommand command)
        {
            string scriptName = command.context.name;
            string path = EditorUtility.SaveFilePanelInProject("New Editor Script", scriptName + "Editor", "cs", "");
            if (path.Length < 0) return;
            string editorScriptName = path.Substring(path.LastIndexOf('/') + 1, path.LastIndexOf('.') - path.LastIndexOf('/') - 1);

            StringBuilder code = new StringBuilder();
            code.Append("using System.Collections;\n");
            code.Append("using System.Collections.Generic;\n");
            code.Append("using UnityEditor;\n");
            code.Append("using UnityEngine;\n");
            code.Append("using Framework;\n\n");

            code.Append("[CanEditMultipleObjects]\n");
            code.Append("[CustomEditor(typeof(" + scriptName + "), true)]\n");
            code.Append("public class " + editorScriptName + " : Editor\n");
            code.Append("{\n");
            code.Append("\tpublic override void OnInspectorGUI()\n");
            code.Append("\t{\n");
            code.Append("\t\tbase.OnInspectorGUI();\n");
            code.Append("\t}\n");
            code.Append("}");

            EditorUtils.CreateTextFile(path, code.ToString());
        }

        [MenuItem("CONTEXT/MonoScript/Create Custom Property Drawer")]
        private static void CreateCustomPropertyDrawer(MenuCommand command)
        {
            string scriptName = command.context.name;
            string path = EditorUtility.SaveFilePanelInProject("New Property Drawer Script", scriptName + "Drawer", "cs", "");
            if (path.Length < 0) return;
            string editorScriptName = path.Substring(path.LastIndexOf('/') + 1, path.LastIndexOf('.') - path.LastIndexOf('/') - 1);

            StringBuilder code = new StringBuilder();
            code.Append("using System.Collections;\n");
            code.Append("using System.Collections.Generic;\n");
            code.Append("using UnityEditor;\n");
            code.Append("using UnityEngine;\n");
            code.Append("using Framework;\n\n");

            code.Append("[CanEditMultipleObjects]\n");
            code.Append("[CustomPropertyDrawer(typeof(" + scriptName + "), true)]\n");
            code.Append("public class " + editorScriptName + " : PropertyDrawer\n");
            code.Append("{\n");
            code.Append("\tpublic override void OnGUI(Rect position, SerializedProperty property, GUIContent label)\n");
            code.Append("\t{\n");
            code.Append("\t\tbase.OnGUI(position, property, label);\n");
            code.Append("\t}\n\n");
            code.Append("\tpublic override float GetPropertyHeight(SerializedProperty property, GUIContent label)\n");
            code.Append("\t{\n");
            code.Append("\t\treturn base.GetPropertyHeight(property, label);\n");
            code.Append("\t}\n");
            code.Append("}");

            EditorUtils.CreateTextFile(path, code.ToString());
        }


        static bool IsSelectionScriptableObject()
        {
            MonoScript script = Selection.activeObject as MonoScript;

            if (script != null)
            {
                return typeof(ScriptableObject).IsAssignableFrom(script.GetClass());
            }

            return false;
        }

        [MenuItem("Assets/Create/Scriptable Object Instance")]
        static void CreateScriptableObjectInstance()
        {
            MonoScript script = Selection.activeObject as MonoScript;

            if (script != null)
            {
                if (typeof(ScriptableObject).IsAssignableFrom(script.GetClass()))
                {
                    ScriptableObject obj = ScriptableObject.CreateInstance(script.name);
                    if (obj == null)
                    {
                        EditorUtility.DisplayDialog("Scriptable Object Creator", "Unable to instantiate class: " + script.name, "Ok");
                        return;
                    }

                    Debug.Log("Created scriptable object instance: " + script.name);

                    AssetDatabase.CreateAsset(obj, "Assets/" + script.name + ".asset");
                    AssetDatabase.SaveAssets();

                    Selection.activeObject = obj;
                }
                else
                {
                    EditorUtility.DisplayDialog("Scriptable Object Creator", "Selected object is not a ScriptableObject class.", "Ok");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Scriptable Object Creator", "Selected object is not a MonoScript", "Ok");

            }
        }

        [MenuItem("Assets/Duplicate Script")]
        private static void DuplicateScript(MenuCommand command)
        {
            void Duplicate(string name)
            {
                MonoScript script = Selection.activeObject as MonoScript;

                if (script.name != name)
                {

                    string text = script.text.Replace("\r\n", "\n");
                    text = text.Replace(script.name, name);

                    string path = AssetDatabase.GetAssetPath(script);
                    path = path.Substring(0, path.LastIndexOf(script.name)) + name + ".cs";

                    EditorUtils.CreateTextFile(path, text);
                }
            }

            TextFieldDialog.Show("Duplicate Script", "Script name:", "Ok", "Cancel", Selection.activeObject.name, Duplicate, null);
        }

        [MenuItem("Assets/Duplicate Script", true)]
        private static bool IsMonoScript()
        {
            return Selection.activeObject is MonoScript;
        }



        [MenuItem("CONTEXT/MonoBehaviour/Validate")]
        private static void ValidateMonoBehaviour(MenuCommand command)
        {
            MethodInfo methodInfo = command.context.GetType().GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo != null)
            {
                methodInfo.Invoke(command.context, new object[0]);
            }

            UnityEditor.EditorUtility.SetDirty(command.context);
        }

        [MenuItem("CONTEXT/Transform/Randomize Y Rotation")]
        private static void RandomizeYRotation(MenuCommand command)
        {
            Transform transform = (command.context as Transform);
            transform.localRotation = Quaternion.Euler(transform.eulerAngles.WithY(Random.Range(0, 360)));
            UnityEditor.EditorUtility.SetDirty(transform);
        }


        [MenuItem("GameObject/Group %G")]
        static void Group()
        {
            Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);


            if (transforms.Length > 0)
            {
                GameObject group = new GameObject(transforms.Length > 1 ? "Group" : transforms[0].name);

                Undo.RegisterCreatedObjectUndo(group, "Group");
                Transform commonParent = transforms[0].parent;
                Vector3 totalPosition = Vector3.zero;

                for (int i = 1; i < transforms.Length; i++)
                {
                    if (commonParent != transforms[i].parent)
                    {
                        commonParent = null;
                        break;
                    }
                }

                if (commonParent != null)
                {
                    group.transform.parent = commonParent;
                }

                for (int i = 0; i < transforms.Length; i++)
                {
                    totalPosition += transforms[i].position;
                }

                group.transform.position = totalPosition / transforms.Length;

                for (int i = 0; i < transforms.Length; i++)
                {
                    Undo.SetTransformParent(transforms[i], group.transform, "Group");
                }


                Selection.activeGameObject = group;
            }
        }

        [MenuItem("CONTEXT/SkinnedMeshRenderer/Update Bones To New Root")]
        private static void UpdateBonesToNewRoot(MenuCommand command)
        {
            SkinnedMeshRenderer renderer = command.context as SkinnedMeshRenderer;
            GameObject meshAssetObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(renderer.sharedMesh)) as GameObject;

            if (meshAssetObject != null)
            {
                if (renderer.rootBone == null)
                {
                    int depth = SceneUtils.GetChildDepth(meshAssetObject.transform, meshAssetObject.transform.FindInChildren(renderer.name));

                    Transform parent = renderer.transform;
                    for (int i = 0; i < depth; i++)
                    {
                        parent = parent.parent;
                    }

                    SkinnedMeshRenderer assetRenderer = meshAssetObject.transform.FindInChildren(renderer.name).GetComponent<SkinnedMeshRenderer>();
                    renderer.rootBone = parent.FindInChildren(assetRenderer.rootBone.name);
                }

                SkinnedMeshRenderer[] childRenderers = meshAssetObject.GetComponentsInChildren<SkinnedMeshRenderer>();

                for (int i = 0; i < childRenderers.Length; i++)
                {
                    if (childRenderers[i].sharedMesh == renderer.sharedMesh)
                    {
                        Transform[] bones = new Transform[childRenderers[i].bones.Length];
                        for (int j = 0; j < bones.Length; j++)
                        {
                            bones[j] = renderer.rootBone.FindInChildren(childRenderers[i].bones[j].name);
                        }

                        renderer.bones = bones;
                        EditorUtility.SetDirty(renderer);
                        break;
                    }
                }
            }
        }

        [MenuItem("Window/Collapse Hierarchy %#C")]
        public static void CollapseHierarchy()
        {
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            EditorWindow hierarchyWindow = EditorWindow.focusedWindow;
            MethodInfo expandMethodInfo = hierarchyWindow.GetType().GetMethod("SetExpandedRecursive");
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                expandMethodInfo.Invoke(hierarchyWindow, new object[] { root.GetInstanceID(), false });
            }
        }

        [MenuItem("Tools/Update Generated Constants")]
        private static void UpdateGeneratedConstants()
        {

            string[] sortingLayerNames = (string[])(typeof(UnityEditorInternal.InternalEditorUtility)).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, new object[0]); ;
            string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;
            List<string> sceneNames = new List<string>();
            List<string> santitizedSceneNames = new List<string>();
            int[] sortingLayerValues = (int[])(typeof(UnityEditorInternal.InternalEditorUtility)).GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, new object[0]);
            int[] layerValues = new int[layerNames.Length];
            LayerMask[] layerMaskValues = new LayerMask[layerNames.Length];
            string[] collisionMatrixNames = new string[layerNames.Length];
            LayerMask[] collisionMatrixValues = new LayerMask[layerNames.Length];
            string[] tagNames = UnityEditorInternal.InternalEditorUtility.tags;
            string[] tagValues = new string[tagNames.Length];

            for (int i = 0; i < layerNames.Length; i++)
            {
                layerNames[i] = StringUtils.Santise(layerNames[i], false, false);
                layerValues[i] = LayerMask.NameToLayer(layerNames[i]);
                layerMaskValues[i] = 1 << layerValues[i];
                collisionMatrixNames[i] = layerNames[i] + "CollisionMask";
                collisionMatrixValues[i] = PhysicsUtils.GetCollisionMatrixMask(i);
            }

            for (int i = 0; i < sortingLayerNames.Length; i++)
            {
                sortingLayerNames[i] = StringUtils.Santise(sortingLayerNames[i], false, false);
            }

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                string path = EditorBuildSettings.scenes[i].path;

                if (!string.IsNullOrEmpty(path))
                {
                    int slashIndex = path.LastIndexOf('/') + 1;
                    string sceneName = path.Substring(slashIndex, path.Length - slashIndex - 6);
                    string sanitizedName = StringUtils.Santise(StringUtils.Titelize(sceneName), false, false);

                    if (!sceneNames.Contains(sceneName) && File.Exists(path))
                    {
                        sceneNames.Add(sceneName);
                        santitizedSceneNames.Add(sanitizedName);
                    }
                }
            }


            for (int i = 0; i < tagNames.Length; i++)
            {
                tagValues[i] = tagNames[i];
                tagNames[i] = StringUtils.Santise(tagNames[i], false, false);
            }


            List<CodeGenerator.CodeDefintion> definitions = new List<CodeGenerator.CodeDefintion>();

            definitions.Add(CodeGenerator.CreateEnumDefinition("LayerName", layerNames, layerValues));
            definitions.Add(CodeGenerator.CreateEnumDefinition("SortingLayerName", sortingLayerNames, sortingLayerValues));

            definitions.Add(CodeGenerator.CreateClass("Layer", CodeGenerator.CreateConstantInts(layerNames, layerValues), true, false));
            definitions.Add(CodeGenerator.CreateClass("SortingLayer", CodeGenerator.CreateConstantInts(sortingLayerNames, sortingLayerValues), true, false));
            definitions.Add(CodeGenerator.CreateClass("Tag", CodeGenerator.CreateConstantStrings(tagNames, tagValues), true, false));
            definitions.Add(CodeGenerator.CreateClass("LayerMasks", CodeGenerator.CreateConstantLayerMasks(layerNames, layerMaskValues, true), true, true));
            definitions.Add(CodeGenerator.CreateClass("CollisionMatrix", CodeGenerator.CreateConstantLayerMasks(collisionMatrixNames, collisionMatrixValues, true), true, false));
            definitions.Add(CodeGenerator.CreateClass("SceneNames", CodeGenerator.CreateConstantStrings(santitizedSceneNames, sceneNames), true, false));

            Type[] scriptableEnumTypes = typeof(ScriptableEnum).GetAllSubtypesInUnityAssemblies();


            for (int i = 0; i < scriptableEnumTypes.Length; i++)
            {
                CodeGenerator.CodeDefintion enumClass = CodeGenerator.CreateClass(scriptableEnumTypes[i].Name, CodeGenerator.CreateScriptableEnumConstants(scriptableEnumTypes[i]), false, true);
                string titleName = StringUtils.Titelize(scriptableEnumTypes[i].Name);
                enumClass.AppendAttribute("CreateAssetMenu", "fileName = \"" + titleName + "\"", "menuName = \"Scriptable Enum/" + titleName + "\"");

                definitions.Add(enumClass);
            }

            for (int i = 0; i < scriptableEnumTypes.Length; i++)
            {
                definitions.Add(CodeGenerator.CreateScriptableEnumMaskClass(scriptableEnumTypes[i]));
            }

            CodeGenerator.CreateSourceFile("AutoGeneratedConstants", definitions);
        }




    }

    public class TextFieldDialog : EditorWindow
    {
        private string _value;
        private string _labelText;
        private string _submitText;
        private string _cancelText;
        private Action<string> _onSubmit;
        private Action _onCancel;

        public static void Show(string titleText, string labelText, string submitText, string cancelText, string defaultValue, Action<string> onSubmit, Action onCancel = null)
        {
            TextFieldDialog dialog = CreateInstance<TextFieldDialog>();

            dialog._labelText = labelText;
            dialog._submitText = submitText;
            dialog._cancelText = cancelText;
            dialog._value = defaultValue;
            dialog._onSubmit = onSubmit;
            dialog._onCancel = onCancel;

            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowAuxWindow();
            dialog.CenterInScreen(250, 100);
        }

        void OnGUI()
        {
            const float MARGIN = 20;

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                Close();
                _onSubmit?.Invoke(_value);
            }

            GUILayout.Space(MARGIN);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);
            GUILayout.Label(_labelText);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);
            _value = EditorGUILayout.TextField(_value, GUILayout.Width(position.width - MARGIN * 2));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(position.height - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3 - MARGIN * 2);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);

            if (GUILayout.Button(_submitText))
            {
                Close();
                _onSubmit?.Invoke(_value);
            }

            if (GUILayout.Button(_cancelText))
            {
                Close();
                _onCancel?.Invoke();
            }

            GUILayout.Space(MARGIN);
            EditorGUILayout.EndHorizontal();
        }

    }

    public class ChooseAssetDialog : EditorWindow
    {
        private Object _value;
        private Type _type;
        private bool _allowSceneObjects;
        private string _labelText;
        private string _submitText;
        private string _cancelText;
        private Action<Object> _onSubmit;
        private Action _onCancel;

        public static void Show(string titleText, string labelText, string submitText, string cancelText, Type type, Object defaultValue, bool allowSceneObjects, Action<Object> onSubmit, Action onCancel = null)
        {
            ChooseAssetDialog dialog = CreateInstance<ChooseAssetDialog>();

            dialog._labelText = labelText;
            dialog._submitText = submitText;
            dialog._cancelText = cancelText;
            dialog._value = defaultValue;
            dialog._type = type;
            dialog._allowSceneObjects = allowSceneObjects;
            dialog._onSubmit = onSubmit;
            dialog._onCancel = onCancel;

            dialog.titleContent = new GUIContent(titleText);
            dialog.ShowAuxWindow();
            dialog.CenterInScreen(250, 100);
        }

        void OnGUI()
        {
            const float MARGIN = 20;

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                Close();
                _onSubmit?.Invoke(_value);
            }

            GUILayout.Space(MARGIN);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);
            GUILayout.Label(_labelText);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);
            _value = EditorGUILayout.ObjectField(_value, _type, _allowSceneObjects, GUILayout.Width(position.width - MARGIN * 2));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(position.height - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3 - MARGIN * 2);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);

            if (GUILayout.Button(_submitText))
            {
                Close();
                _onSubmit?.Invoke(_value);
            }

            if (GUILayout.Button(_cancelText))
            {
                Close();
                _onCancel?.Invoke();
            }

            GUILayout.Space(MARGIN);
            EditorGUILayout.EndHorizontal();
        }

    }
}