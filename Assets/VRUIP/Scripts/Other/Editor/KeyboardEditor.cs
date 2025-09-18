using UnityEngine;
using UnityEditor;

namespace VRUIP
{
    [CustomEditor(typeof(Keyboard))]
    public class KeyboardEditor : BaseEditor
    {
        // KEYBOARD PROPERTIES
        private SerializedProperty fieldTypeProperty;
        private SerializedProperty tmpInputProperty;
        private SerializedProperty tmpTextProperty;
        private SerializedProperty tabSpacesProperty;
        private SerializedProperty followThemeProperty;

        // KEYBOARD COMPONENTS
        private SerializedProperty backgroundProperty;
        private SerializedProperty normalSectionProperty;
        private SerializedProperty shiftSectionProperty;
        private SerializedProperty contentTransformProperty;
        private SerializedProperty handlesProperty;
        private SerializedProperty canvasProperty;
        
        // KEYBOARD AUDIO
        private SerializedProperty audioSourceProperty;
        private SerializedProperty buttonClickSoundProperty;
        private SerializedProperty shiftCapsClickSoundProperty;
        private SerializedProperty spaceClickSoundProperty;
        private SerializedProperty backspaceClickSoundProperty;
        private SerializedProperty enterClickSoundProperty;

        private bool _foldOut;
        
        private void OnEnable()
        {
            // Keyboard properties
            fieldTypeProperty = serializedObject.FindProperty("fieldType");
            tmpInputProperty = serializedObject.FindProperty("tmpInput");
            tmpTextProperty = serializedObject.FindProperty("tmpText");
            tabSpacesProperty = serializedObject.FindProperty("tabSpaces");
            followThemeProperty = serializedObject.FindProperty("followsTheme");
            
            // Keyboard components
            backgroundProperty = serializedObject.FindProperty("background");
            normalSectionProperty = serializedObject.FindProperty("normalSection");
            shiftSectionProperty = serializedObject.FindProperty("shiftSection");
            contentTransformProperty = serializedObject.FindProperty("contentTransform");
            handlesProperty = serializedObject.FindProperty("handles");
            canvasProperty = serializedObject.FindProperty("canvas");

            // Keyboard audio
            audioSourceProperty = serializedObject.FindProperty("audioSource");
            buttonClickSoundProperty = serializedObject.FindProperty("buttonClickSound");
            shiftCapsClickSoundProperty = serializedObject.FindProperty("shiftCapsClickSound");
            spaceClickSoundProperty = serializedObject.FindProperty("spaceClickSound");
            backspaceClickSoundProperty = serializedObject.FindProperty("backspaceClickSound");
            enterClickSoundProperty = serializedObject.FindProperty("enterClickSound");
        }
        
        public override void OnInspectorGUI()
        {
            var keyboard = (Keyboard)target;
            
            EditorGUILayout.LabelField("Keyboard Properties", headerStyle);
            EditorGUILayout.PropertyField(followThemeProperty);
            EditorGUILayout.PropertyField(tabSpacesProperty);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(fieldTypeProperty);
            EditorGUILayout.PropertyField(keyboard.IsTmpInput ? tmpInputProperty : tmpTextProperty);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Keyboard Audio", headerStyle);
            EditorGUILayout.PropertyField(audioSourceProperty);
            EditorGUILayout.PropertyField(buttonClickSoundProperty);
            EditorGUILayout.PropertyField(shiftCapsClickSoundProperty);
            EditorGUILayout.PropertyField(spaceClickSoundProperty);
            EditorGUILayout.PropertyField(backspaceClickSoundProperty);
            EditorGUILayout.PropertyField(enterClickSoundProperty);
            
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Keyboard Components", headerStyle);
            _foldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_foldOut, "List of Components");
            if (_foldOut)
            {
                EditorGUILayout.PropertyField(backgroundProperty);
                EditorGUILayout.PropertyField(normalSectionProperty);
                EditorGUILayout.PropertyField(shiftSectionProperty);
                EditorGUILayout.PropertyField(contentTransformProperty);
                EditorGUILayout.PropertyField(handlesProperty);
                EditorGUILayout.PropertyField(canvasProperty);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
