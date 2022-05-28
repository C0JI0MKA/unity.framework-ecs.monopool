using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace Leopotam.EcsLite.MonoPool.Object
{
    [CustomPropertyDrawer(typeof(ComponentForObject))]
    public sealed class ComponentForObjectDrawer : PropertyDrawer
    {
        private readonly string[] SOLUTION_NAMES = new[] { "Assembly-CSharp" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            height += (EditorGUIUtility.singleLineHeight + 2) * (1 + property.FindPropertyRelative("_value").CountInProperty());
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty valueFromProperty = property.FindPropertyRelative("_value");
                SerializedProperty typeFromProperty = property.FindPropertyRelative("_type");

                Rect rect = new(position.x, position.y, position.size.x, 18);
                rect.y += 20;

                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => SOLUTION_NAMES.Contains(x.GetName().Name)).ToArray()[0];
                List<Type> typeList = assembly.GetTypes().Where(x => x.IsValueType && x.IsEnum is not true && x.IsGenericType is not true).ToList();
                typeList.Remove(typeof(ComponentForObject));
                
                Type type = typeList[typeList.Select(x => x.Name).ToList().IndexOf(typeFromProperty.stringValue)];
                Type oldType = type;
                if(type == null)
                    type = typeList[0];

                type = typeList[EditorGUI.Popup(rect, typeList.IndexOf(type), typeList.Select(x => x.Name).ToArray())];
                if(type != oldType)
                    valueFromProperty.managedReferenceValue = Activator.CreateInstance(type);
                typeFromProperty.stringValue = type.Name;
                
                rect.y += 20;
                EditorGUI.PropertyField(rect, valueFromProperty, true);
            }
            EditorGUI.EndProperty();
        }
    }
}
