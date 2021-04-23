using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    [CustomPropertyDrawer(typeof(PrefabDropdownAttribute), true)]
    public class PrefabDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            bool isGameObjectField = false;
            bool allowNull = (attribute as PrefabDropdownAttribute).AllowNull;
            Type type = (attribute as PrefabDropdownAttribute).Type;
            Type fieldType = property.GetValueType();


            if (type == null)
            {
                type = fieldType;
            }
            else if (fieldType == typeof(GameObject) || fieldType == typeof(GameObject[]) || fieldType == typeof(List<GameObject>))
            {
                isGameObjectField = true;
            }

            EditorGUI.BeginProperty(rect, label, property);

            EditorGUI.BeginChangeCheck();

            List<UnityEngine.Object> prefabs = new List<Object>(Resources.LoadAll("", type));
            if (allowNull)
            {
                prefabs.Insert(0, null);
            }
            UnityEngine.Object[] choices = prefabs.ToArray();
            GUIContent[] names = new GUIContent[choices.Length];

            int selectedIndex = 0;
            for (int i = 0; i < choices.Length; i++)
            {
                if (choices[i] == null)
                {
                    names[i] = new GUIContent("NONE");
                }
                else
                {
                    if (isGameObjectField)
                    {
                        choices[i] = (choices[i] as Component).gameObject;
                    }

                    names[i] = new GUIContent(choices[i].name);
                }

                if (choices[i] == property.objectReferenceValue)
                {
                    selectedIndex = i;
                }
            }

            if (choices.Length == 0)
            {
                choices = new[] { (UnityEngine.Object)null };
                names = new[] { new GUIContent("NONE") };
            }

            int newChoice = EditorGUI.Popup(rect, label, selectedIndex, names);

            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = choices[newChoice];
            }

            EditorGUI.EndProperty();

        }
    }
}