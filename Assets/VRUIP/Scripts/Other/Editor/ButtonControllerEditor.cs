using UnityEditor;
using UnityEngine;

namespace VRUIP
{
    [CustomEditor(typeof(ButtonController))]
    public class ButtonControllerEditor : BaseEditor
    {
        // THEME PROPERTIES
        private SerializedProperty followsThemeProperty;
        // Normal color properties
        private SerializedProperty buttonNormalColorProperty;
        private SerializedProperty borderNormalColorProperty;
        private SerializedProperty textNormalColorProperty;
        // Hover color properties
        private SerializedProperty buttonHoverColorProperty;
        private SerializedProperty borderHoverColorProperty;
        private SerializedProperty textHoverColorProperty;
        // Click color properties
        private SerializedProperty buttonClickColorProperty;
        private SerializedProperty borderClickColorProperty;
        private SerializedProperty textClickColorProperty;
        
        // BUTTON PROPERTIES
        private SerializedProperty interactableProperty;
        private SerializedProperty textProperty;
        private SerializedProperty hasBorderProperty;
        private SerializedProperty borderWidthProperty;
        private SerializedProperty roundnessProperty;
        private SerializedProperty clickSoundEnabledProperty;
        private SerializedProperty clickSoundProperty;
        private SerializedProperty hoverSoundProperty;
        
        // BUTTON ANIMATION
        private SerializedProperty animationTypeProperty;

        // BUTTON COMPONENTS
        private SerializedProperty buttonProperty;
        private SerializedProperty buttonImageProperty;
        private SerializedProperty buttonBorderProperty;
        private SerializedProperty buttonTextProperty;
        private SerializedProperty expandBorderProperty;
        private SerializedProperty buttonShineProperty;
        private SerializedProperty buttonSlideProperty;

        private bool _foldOut;

        private void OnEnable()
        {
            // Theme properties
            followsThemeProperty = serializedObject.FindProperty("followsTheme");
            buttonNormalColorProperty = serializedObject.FindProperty("buttonNormalColor");
            borderNormalColorProperty = serializedObject.FindProperty("borderNormalColor");
            textNormalColorProperty = serializedObject.FindProperty("textNormalColor");
            buttonHoverColorProperty = serializedObject.FindProperty("buttonHoverColor");
            borderHoverColorProperty = serializedObject.FindProperty("borderHoverColor");
            textHoverColorProperty = serializedObject.FindProperty("textHoverColor");
            buttonClickColorProperty = serializedObject.FindProperty("buttonClickColor");
            borderClickColorProperty = serializedObject.FindProperty("borderClickColor");
            textClickColorProperty = serializedObject.FindProperty("textClickColor");
            
            // Button properties
            interactableProperty = serializedObject.FindProperty("interactable");
            textProperty = serializedObject.FindProperty("text");
            hasBorderProperty = serializedObject.FindProperty("hasBorder");
            borderWidthProperty = serializedObject.FindProperty("borderWidth");
            roundnessProperty = serializedObject.FindProperty("roundness");
            clickSoundEnabledProperty = serializedObject.FindProperty("clickSoundEnabled");
            clickSoundProperty = serializedObject.FindProperty("clickSound");
            hoverSoundProperty = serializedObject.FindProperty("hoverSound");
            
            // Button animation
            animationTypeProperty = serializedObject.FindProperty("animationType");

            // Button components
            buttonProperty = serializedObject.FindProperty("button");
            buttonImageProperty = serializedObject.FindProperty("buttonImage");
            buttonBorderProperty = serializedObject.FindProperty("buttonBorder");
            buttonTextProperty = serializedObject.FindProperty("buttonText");
            expandBorderProperty = serializedObject.FindProperty("expandBorder");
            buttonShineProperty = serializedObject.FindProperty("buttonShine");
            buttonSlideProperty = serializedObject.FindProperty("buttonSlide");
        }

        public override void OnInspectorGUI()
        {
            var button = (ButtonController)target;
            
            // Theme properties
            ConstructThemeSection(button);
            
            GUILayout.Space(20);
            
            // Button properties
            ConstructPropertiesSection();
            
            GUILayout.Space(20);
            
            // Button animation
            ConstructAnimationSection();
            
            GUILayout.Space(20);
            
            // Button components
            ConstructComponentSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void ConstructThemeSection(ButtonController button)
        {
            EditorGUILayout.LabelField("Theme Properties", headerStyle);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(followsThemeProperty);
            if (!button.followsTheme)
            {
                ConstructColorProperties();
            }
        }
        
        private void ConstructColorProperties()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Normal Colors", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(buttonNormalColorProperty);
            EditorGUILayout.PropertyField(borderNormalColorProperty);
            EditorGUILayout.PropertyField(textNormalColorProperty);
            
            EditorGUILayout.LabelField("Hover Colors", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(buttonHoverColorProperty);
            EditorGUILayout.PropertyField(borderHoverColorProperty);
            EditorGUILayout.PropertyField(textHoverColorProperty);
            
            EditorGUILayout.LabelField("Click Colors", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(buttonClickColorProperty);
            EditorGUILayout.PropertyField(borderClickColorProperty);
            EditorGUILayout.PropertyField(textClickColorProperty);
            EditorGUI.indentLevel--;
        }
        
        private void ConstructPropertiesSection()
        {
            EditorGUILayout.LabelField("Button Properties", headerStyle);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(interactableProperty);
            EditorGUILayout.PropertyField(textProperty);
            EditorGUILayout.PropertyField(roundnessProperty);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(hasBorderProperty);
            if (hasBorderProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(borderWidthProperty);
                
                EditorGUI.indentLevel--;
            }
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(clickSoundEnabledProperty);
            if (clickSoundEnabledProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(clickSoundProperty);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(hoverSoundProperty);
        }

        private void ConstructAnimationSection()
        {
            EditorGUILayout.LabelField("Animations", headerStyle);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(animationTypeProperty);
        }

        private void ConstructComponentSection()
        {
            EditorGUILayout.LabelField("Components", headerStyle);
            GUILayout.Space(4);
            _foldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_foldOut, "List of Components");
            if (!_foldOut) return;
            EditorGUILayout.PropertyField(buttonProperty);
            EditorGUILayout.PropertyField(buttonImageProperty);
            EditorGUILayout.PropertyField(buttonBorderProperty);
            EditorGUILayout.PropertyField(buttonTextProperty);
            EditorGUILayout.PropertyField(expandBorderProperty);
            EditorGUILayout.PropertyField(buttonShineProperty);
            EditorGUILayout.PropertyField(buttonSlideProperty);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
