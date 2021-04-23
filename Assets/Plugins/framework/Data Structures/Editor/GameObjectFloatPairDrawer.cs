using UnityEditor;
using UnityEngine;

namespace Framework
{
    [CustomPropertyDrawer(typeof(GameObjectFloatPair))]
    public class GameObjectFloatPairDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            Rect amountRect = new Rect(rect.x, rect.y, 60 + (EditorGUI.indentLevel * 15), rect.height);
            Rect objectRect = new Rect(rect.x + 65, rect.y, rect.width - 65, rect.height);

            EditorGUI.PropertyField(objectRect, property.FindPropertyRelative("_gameObject"), GUIContent.none);
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("_number"), GUIContent.none);

            EditorGUI.EndProperty();
        }

    }
}



