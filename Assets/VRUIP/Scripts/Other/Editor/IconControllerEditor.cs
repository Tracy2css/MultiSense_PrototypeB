using System;
using UnityEditor;
using UnityEngine;

namespace VRUIP
{
    [CustomEditor(typeof(IconController))]
    public class IconControllerEditor : BaseEditor
    {
        // THEME PROPERTIES
        private SerializedProperty followsThemeProperty;
        private SerializedProperty normalColorProperty;
        private SerializedProperty hoverColorProperty;
        private SerializedProperty clickColorProperty;
        
        // ICON PROPERTIES
        private SerializedProperty iconSpriteProperty;
        private SerializedProperty interactableProperty;
        private SerializedProperty clickSoundEnabledProperty;
        private SerializedProperty clickSoundProperty;
        private SerializedProperty hoverSoundProperty;
        private SerializedProperty onClickProperty;
        
        // ICON COMPONENTS
        private SerializedProperty iconImageProperty;
        private SerializedProperty buttonProperty;

        private void OnEnable()
        {
            // Theme properties
            followsThemeProperty = serializedObject.FindProperty("followsTheme");
            normalColorProperty = serializedObject.FindProperty("iconNormalColor");
            hoverColorProperty = serializedObject.FindProperty("iconHoverColor");
            clickColorProperty = serializedObject.FindProperty("iconClickColor");
            
            // Icon properties
            iconSpriteProperty = serializedObject.FindProperty("iconSprite");
            interactableProperty = serializedObject.FindProperty("interactable");
            clickSoundEnabledProperty = serializedObject.FindProperty("clickSoundEnabled");
            clickSoundProperty = serializedObject.FindProperty("clickSound");
            hoverSoundProperty = serializedObject.FindProperty("hoverSound");
            onClickProperty = serializedObject.FindProperty("onClick");
            
            // Icon components
            iconImageProperty = serializedObject.FindProperty("iconImage");
            buttonProperty = serializedObject.FindProperty("button");
        }

        public override void OnInspectorGUI()
        {
            var icon = (IconController) target;
            
            ConstructThemeSection(icon);
            
            GUILayout.Space(20);
            
            ConstructIconProperties();
            
            GUILayout.Space(20);
            
            ConstructComponentSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void ConstructThemeSection(IconController icon)
        {
            EditorGUILayout.LabelField("Theme Properties", headerStyle);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(followsThemeProperty);
            if (!icon.followsTheme)
            {
                ConstructColorProperties();
            }
        }
        
        private void ConstructColorProperties()
        {
            EditorGUILayout.LabelField("Color Properties", secondaryHeaderStyle);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(normalColorProperty);
            EditorGUILayout.PropertyField(hoverColorProperty);
            EditorGUILayout.PropertyField(clickColorProperty);
        }
        
        private void ConstructIconProperties()
        {
            EditorGUILayout.LabelField("Icon Properties", headerStyle);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(iconSpriteProperty);
            EditorGUILayout.PropertyField(interactableProperty);
            EditorGUILayout.PropertyField(clickSoundEnabledProperty);
            EditorGUILayout.PropertyField(clickSoundProperty);
            EditorGUILayout.PropertyField(hoverSoundProperty);
            EditorGUILayout.PropertyField(onClickProperty);
        }
        
        private void ConstructComponentSection()
        {
            EditorGUILayout.LabelField("Icon Components", headerStyle);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(iconImageProperty);
            EditorGUILayout.PropertyField(buttonProperty);
        }
    }
}
