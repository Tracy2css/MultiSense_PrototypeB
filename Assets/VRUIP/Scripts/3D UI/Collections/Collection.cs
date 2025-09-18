using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VRUIP
{
    public class Collection : MonoBehaviour
    {
        [SerializeField] private Element elementType;
        [SerializeField] protected int elementsPerRow;
        [SerializeField][Range(0,5)] private int horizontalSpacing;
        [SerializeField][Range(0,5)] private int verticalSpacing;
        [SerializeField] private ElementInfo[] elements;

        private const float SPACING_MULTIPLIER = 0.1f;

        /// <summary>
        /// Initialize this collection based on provided properties.
        /// </summary>
        [ContextMenu("Initialize Collection")]
        public void Initialize()
        {
            // Check if there are any elements to create.
            if (elements.Length == 0) return;
            
            var positions = CalculatePositions();
            for (var x = 0; x < elements.Length; x++)
            {
#if UNITY_EDITOR
                elementType.CreateElementFromInfoEditor(elements[x], positions[x], transform);
#else
                elementType.CreateElementFromInfo(elements[x], positions[x], transform);
#endif
            }
        }
        
        // Calculate positions for elements of this collection based element count.
        private List<Vector3> CalculatePositions()
        {
            var positionList = new List<Vector3>();
            var size = elementType.Size;
            var rows = elements.Length / elementsPerRow;
            var elementSpacingX = (1 + horizontalSpacing * SPACING_MULTIPLIER) * size.x;
            var elementSpacingY = (1 + verticalSpacing * SPACING_MULTIPLIER) * size.y;
            var startingPointX = -elementSpacingX / 2f * (elementsPerRow - 1);
            var startingPointY = elementSpacingY / 2f * (rows - 1);
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < elementsPerRow; x++)
                {
                    positionList.Add(new Vector3(startingPointX + x * elementSpacingX, startingPointY - y * elementSpacingY));
                }
            }
            return positionList;
        }

        /// <summary>
        /// Clear all elements of this collection.
        /// </summary>
        [ContextMenu("Clear Collection")]
        public void Clear()
        {
            var children = (from Transform child in transform select child.gameObject).ToList();
            children.ForEach(DestroyImmediate);
        }
        
        [Serializable]
        public class ElementInfo
        {
            public string title;
            public Sprite sprite;
        }
    }
}