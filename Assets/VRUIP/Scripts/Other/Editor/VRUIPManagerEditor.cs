using UnityEditor;
using UnityEngine;

namespace VRUIP
{
    [CustomEditor(typeof(VRUIPManager))]
    public class VRUIPManagerEditor : BaseEditor
    {
        private SerializedProperty colorModeProperty;
        private SerializedProperty colorThemeProperty;
        private SerializedProperty cameraProperty;
        private SerializedProperty lineRendererProperty;
        private SerializedProperty laserPointerProperty;
        private SerializedProperty leftHandProperty;
        private SerializedProperty rightHandProperty;
        // Meta SDK
        private SerializedProperty handRayInteractorProperty;
        private SerializedProperty controllerRayInteractorProperty;
        private SerializedProperty controllerGrabInteractorProperty;
        
        // Other UI Components
        private SerializedProperty scaleButtonProperty;
        private SerializedProperty keyboardProperty;

        private void OnEnable()
        {
            colorModeProperty = serializedObject.FindProperty("colorMode");
            colorThemeProperty = serializedObject.FindProperty("customColorTheme");
            cameraProperty = serializedObject.FindProperty("mainCamera");
            #if !META_SDK
            lineRendererProperty = serializedObject.FindProperty("lineRenderer");
            #endif
            laserPointerProperty = serializedObject.FindProperty("laserPointer");
            #if OCULUS_INTEGRATION
            leftHandProperty = serializedObject.FindProperty("leftHand");
            rightHandProperty = serializedObject.FindProperty("rightHand");
            #elif META_SDK
            handRayInteractorProperty = serializedObject.FindProperty("handRayInteractor");
            controllerRayInteractorProperty = serializedObject.FindProperty("controllerRayInteractor");
            controllerGrabInteractorProperty = serializedObject.FindProperty("controllerGrabInteractor");
            #endif
            scaleButtonProperty = serializedObject.FindProperty("scaleButton");
            keyboardProperty = serializedObject.FindProperty("keyboardPrefab");
        }
        
        public override void OnInspectorGUI()
        {
            var manager = (VRUIPManager)target;

            #if OCULUS_INTEGRATION
            EditorGUILayout.LabelField("Target Framework: Oculus Integration", headerStyle);
            #elif XR_ITK
            EditorGUILayout.LabelField("Target Framework: XRITK", headerStyle);
            #elif META_SDK
            EditorGUILayout.LabelField("Target Framework: Meta SDK", headerStyle);
            #else
            EditorGUILayout.LabelField("Target Framework: Unity/Testing", headerStyle);
            #endif
            
            GUILayout.Space(10);
            
            // Properties
            EditorGUILayout.PropertyField(colorModeProperty);
            if (manager.colorMode == VRUIPManager.ColorThemeMode.Custom)
            {
                EditorGUILayout.PropertyField(colorThemeProperty);
            }
            GUILayout.Space(10);
            GUILayout.Label("Press button to update theme.", secondaryHeaderStyle);
            if (GUILayout.Button("Update UI"))
            {
                manager.SetTheme();
            }
            
            GUILayout.Space(30);
            
            // Scene components
            EditorGUILayout.LabelField("Framework Specific Components", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(cameraProperty);
            if (EditorUtil.GetTargetFramework() != VRUIPManager.TargetFramework.UnityEditorTesting && EditorUtil.GetTargetFramework() != VRUIPManager.TargetFramework.MetaSDK)
            {
                EditorGUILayout.PropertyField(lineRendererProperty);
            }
            if (EditorUtil.GetTargetFramework() == VRUIPManager.TargetFramework.MetaSDK)
            {
                EditorGUILayout.PropertyField(handRayInteractorProperty);
                EditorGUILayout.PropertyField(controllerRayInteractorProperty);
                EditorGUILayout.PropertyField(controllerGrabInteractorProperty);
            }
            if (EditorUtil.GetTargetFramework() == VRUIPManager.TargetFramework.OculusIntegration)
            {
                EditorGUILayout.PropertyField(laserPointerProperty);
                #if OCULUS_INTEGRATION
                EditorGUILayout.PropertyField(leftHandProperty);
                EditorGUILayout.PropertyField(rightHandProperty);
                #endif
            }

            GUILayout.Space(30);

            // UI Components
            EditorGUILayout.LabelField("UI Universal Components", secondaryHeaderStyle);
            EditorGUILayout.PropertyField(scaleButtonProperty);
            EditorGUILayout.PropertyField(keyboardProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
