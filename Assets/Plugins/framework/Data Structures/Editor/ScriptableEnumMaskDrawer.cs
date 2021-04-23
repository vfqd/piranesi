using System;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(ScriptableEnumMask), true)]
    public class ScriptableEnumMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty isEverythingProperty = property.FindPropertyRelative("_isEverything");
            SerializedProperty listProperty = property.FindPropertyRelative("_values");

            ScriptableEnumMask mask = property.GetValue<ScriptableEnumMask>();

            Type valueType = mask.GetType().BaseType.GetGenericArguments()[0];

            ScriptableEnum[] values = ScriptableEnum.GetValues(valueType);
            //  values.Sort((x, y) => x.name.CompareTo(y.name));

            string[] labels = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                labels[i] = values[i].name;
            }

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            int newMaskValue = EditorGUI.MaskField(position, label, isEverythingProperty.boolValue ? ~0 : mask.GetBitMask(), labels);

            if (EditorGUI.EndChangeCheck())
            {
                mask.SetFromBitMask(newMaskValue);

                ScriptableEnum[] maskValues = mask.GetRawValues();

                listProperty.arraySize = maskValues.Length;

                for (int i = 0; i < maskValues.Length; i++)
                {
                    listProperty.GetArrayElementAtIndex(i).objectReferenceValue = maskValues[i];
                }

                isEverythingProperty.boolValue = mask.IsEverything;

                //    EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorGUI.EndProperty();
        }


    }
}

