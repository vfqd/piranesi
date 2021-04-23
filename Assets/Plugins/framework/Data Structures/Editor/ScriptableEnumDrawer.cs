using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Framework
{
    [CustomPropertyDrawer(typeof(ScriptableEnum), true)]
    [CanEditMultipleObjects]
    public class ScriptableEnumDrawer : PropertyDrawer
    {
        const float PICKER_WIDTH = 36f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() : fieldInfo.FieldType;
            int selectedIndex = SetupValuesAndLabels(type, property.objectReferenceValue, out List<ScriptableEnum> values, out GUIContent[] labels, true);

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position.WithWidth(position.width - PICKER_WIDTH), label, selectedIndex, labels);
            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = values[selectedIndex];
            }
            EditorGUI.indentLevel = 0;

            Rect rect = new Rect(position.xMax - PICKER_WIDTH, position.y, PICKER_WIDTH, position.height);
            EditorGUI.PropertyField(rect, property, GUIContent.none);

            EditorGUI.EndProperty();

        }

        static int SetupValuesAndLabels(Type type, Object currentValue, out List<ScriptableEnum> values, out GUIContent[] labels, bool allowNull)
        {
            values = new List<ScriptableEnum>(ScriptableEnum.GetValues(type));
            values.Sort((x, y) => x.name.CompareTo(y.name));

            if (allowNull)
            {
                values.Insert(0, null);
            }

            int selectedIndex = 0;
            if (currentValue != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] == currentValue)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            labels = new GUIContent[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == null)
                {
                    labels[i] = new GUIContent("NONE");
                }
                else
                {
                    labels[i] = new GUIContent(values[i].name);
                }
            }

            return selectedIndex;
        }

        public static T DrawField<T>(Rect position, GUIContent label, T currentValue, bool allowNull = true) where T : ScriptableEnum
        {

            int selectedIndex = SetupValuesAndLabels(typeof(T), currentValue, out List<ScriptableEnum> values, out GUIContent[] labels, allowNull);

            if (label == GUIContent.none)
            {
                selectedIndex = EditorGUI.Popup(position.WithWidth(position.width - PICKER_WIDTH), selectedIndex, labels);
            }
            else
            {
                selectedIndex = EditorGUI.Popup(position.WithWidth(position.width - PICKER_WIDTH), label, selectedIndex, labels);
            }


            currentValue = (T)values[selectedIndex];

            Rect rect = new Rect(position.xMax - PICKER_WIDTH, position.y, PICKER_WIDTH, position.height);
            currentValue = (T)EditorGUI.ObjectField(rect, GUIContent.none, currentValue, typeof(T), false);

            return currentValue;
        }

        public static T DrawField<T>(GUIContent label, T currentValue, bool allowNull = true) where T : ScriptableEnum
        {

            int selectedIndex = SetupValuesAndLabels(typeof(T), currentValue, out List<ScriptableEnum> values, out GUIContent[] labels, allowNull);

            EditorGUILayout.BeginHorizontal();


            if (label == GUIContent.none)
            {
                selectedIndex = EditorGUILayout.Popup(selectedIndex, labels);
            }
            else
            {
                selectedIndex = EditorGUILayout.Popup(label, selectedIndex, labels);
            }


            currentValue = (T)values[selectedIndex];

            currentValue = (T)EditorGUILayout.ObjectField(GUIContent.none, currentValue, typeof(T), false, GUILayout.Width(PICKER_WIDTH));

            EditorGUILayout.EndHorizontal();

            return currentValue;
        }

        public static T DrawPopup<T>(Rect position, GUIContent label, T currentValue, bool allowNull = true) where T : ScriptableEnum
        {
            int selectedIndex = SetupValuesAndLabels(typeof(T), currentValue, out List<ScriptableEnum> values, out GUIContent[] labels, allowNull);

            selectedIndex = EditorGUI.Popup(position.WithWidth(position.width), label, selectedIndex, labels);
            currentValue = (T)values[selectedIndex];

            return currentValue;
        }

        public static T DrawPopup<T>(GUIContent label, T currentValue, bool allowNull = true) where T : ScriptableEnum
        {

            int selectedIndex = SetupValuesAndLabels(typeof(T), currentValue, out List<ScriptableEnum> values, out GUIContent[] labels, allowNull);

            EditorGUILayout.BeginHorizontal();

            if (label == GUIContent.none)
            {
                selectedIndex = EditorGUILayout.Popup(selectedIndex, labels);
            }
            else
            {
                selectedIndex = EditorGUILayout.Popup(label, selectedIndex, labels);
            }


            currentValue = (T)values[selectedIndex];

            EditorGUILayout.EndHorizontal();

            return currentValue;
        }


    }
}

