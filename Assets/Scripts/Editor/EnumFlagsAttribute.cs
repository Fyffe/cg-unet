using UnityEngine;
using UnityEditor;

namespace LV
{
    public class EnumFlags : PropertyAttribute
    {
        public EnumFlags() { }
    }

    [CustomPropertyDrawer(typeof(EnumFlags))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginChangeCheck();
            int newValue = UnityEditor.EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
            if (EditorGUI.EndChangeCheck())
            {
                _property.intValue = newValue;
            }
        }
    }
}