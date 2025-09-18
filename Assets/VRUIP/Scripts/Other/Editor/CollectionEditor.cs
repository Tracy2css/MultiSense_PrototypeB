using UnityEditor;
using UnityEngine;

namespace VRUIP
{
    [CustomEditor(typeof(Collection))]
    public class CollectionEditor : BaseEditor
    {
        private SerializedProperty elementTypeProperty;
        private SerializedProperty elementsPerRowProperty;
        private SerializedProperty horizontalSpacingProperty;
        private SerializedProperty verticalSpacingProperty;
        private SerializedProperty elementsProperty;

        private void OnEnable()
        {
            elementTypeProperty = serializedObject.FindProperty("elementType");
            elementsPerRowProperty = serializedObject.FindProperty("elementsPerRow");
            horizontalSpacingProperty = serializedObject.FindProperty("horizontalSpacing");
            verticalSpacingProperty = serializedObject.FindProperty("verticalSpacing");
            elementsProperty = serializedObject.FindProperty("elements");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Collection Properties", headerStyle);
            
            EditorGUILayout.PropertyField(elementTypeProperty);
            EditorGUILayout.PropertyField(elementsPerRowProperty);
            EditorGUILayout.PropertyField(horizontalSpacingProperty);
            EditorGUILayout.PropertyField(verticalSpacingProperty);
            EditorGUILayout.PropertyField(elementsProperty);
            
            serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Initialize Collection"))
            {
                ((Collection) target).Initialize();
            }
            
            if (GUILayout.Button("Clear Collection"))
            {
                ((Collection) target).Clear();
            }
        }
    }
}
