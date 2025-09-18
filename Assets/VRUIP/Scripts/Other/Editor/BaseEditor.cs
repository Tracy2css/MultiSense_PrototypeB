using UnityEditor;
using UnityEngine;

namespace VRUIP
{
    public class BaseEditor : Editor
    {
        protected GUIStyle headerStyle;
        protected GUIStyle secondaryHeaderStyle;
        
        private void Awake()
        {
            // Editor styles
            headerStyle = new GUIStyle()
            {
                normal =
                {
                    textColor = Color.cyan
                },
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };

            secondaryHeaderStyle = new GUIStyle()
            {
                normal =
                {
                    textColor = Color.cyan
                },
                fontStyle = FontStyle.Italic,
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft
            };
        }
    }
}
