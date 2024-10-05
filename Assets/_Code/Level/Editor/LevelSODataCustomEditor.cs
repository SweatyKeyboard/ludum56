using System;
using UnityEditor;
using UnityEngine;

namespace _Code.Level.Editor
{
    [CustomEditor(typeof(LevelSOData))]
    public sealed class LevelSODataCustomEditor : UnityEditor.Editor
    {
        private SerializedProperty _width;
        private SerializedProperty _height;
        private SerializedProperty _cells;

        private void OnEnable()
        {
            _width = serializedObject.FindProperty("Width");
            _height = serializedObject.FindProperty("Height");
            _cells = serializedObject.FindProperty("Cells");
        }

        public override void OnInspectorGUI()
        {
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.PropertyField(_width);
                EditorGUILayout.PropertyField(_height);
                
                if (EditorGUI.EndChangeCheck())
                {
                    _cells.arraySize = _width.intValue * _height.intValue;
                }
            
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginVertical();
                for (int i = 0; i < _height.intValue; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < _width.intValue; j++)
                    {
                        EditorGUILayout.PropertyField(_cells.GetArrayElementAtIndex(j + (_height.intValue - i - 1) * _width.intValue), new GUIContent());
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
        }
    }
}   