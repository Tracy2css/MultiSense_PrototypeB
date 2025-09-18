using UnityEditor;
using UnityEngine;

namespace VRUIP
{
    [CustomEditor(typeof(TextController))]
    public class TextControllerEditor : BaseEditor
    {
        // THEME PROPERTIES
        private SerializedProperty _followsThemeProperty;
        
        // TEXT PROPERTIES
        private SerializedProperty _enableTypingProperty;
        private SerializedProperty _typingOnAwakeProperty;
        private SerializedProperty _pauseLongerOnPeriodProperty;
        private SerializedProperty _delayBetweenLettersProperty;
        private SerializedProperty _onTypingFinishedProperty;
        private SerializedProperty _textProperty;

        private void OnEnable()
        {
            _followsThemeProperty = serializedObject.FindProperty("followsTheme");
            _enableTypingProperty = serializedObject.FindProperty("enableTyping");
            _typingOnAwakeProperty = serializedObject.FindProperty("typingOnAwake");
            _pauseLongerOnPeriodProperty = serializedObject.FindProperty("pauseLongerOnPeriod");
            _delayBetweenLettersProperty = serializedObject.FindProperty("delayBetweenLetters");
            _onTypingFinishedProperty = serializedObject.FindProperty("onTypingFinished");
            _textProperty = serializedObject.FindProperty("text");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Theme Properties", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(_followsThemeProperty);
            
            GUILayout.Space(20);
            
            EditorGUILayout.LabelField("Text Properties", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(_enableTypingProperty);
            if (_enableTypingProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_typingOnAwakeProperty);
                EditorGUILayout.PropertyField(_pauseLongerOnPeriodProperty);
                EditorGUILayout.PropertyField(_delayBetweenLettersProperty);
                EditorGUILayout.PropertyField(_onTypingFinishedProperty);
                EditorGUI.indentLevel--;
            }
            
            GUILayout.Space(20);
            
            EditorGUILayout.LabelField("Components", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(_textProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
