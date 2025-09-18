using System.Collections.Generic;
using UnityEngine;

namespace VRUIP.Drawing
{
    public class Eraser : A_Grabbable
    {
        private readonly List<GameObject> _undoList = new();
        private const int MAX_SAVED_DRAWINGS = 10;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Drawing>() != null)
            {
                AddToUndoList(other.transform.parent.gameObject);
            }
        }

        private void AddToUndoList(GameObject drawing)
        {
            // If the list is full, remove the first drawing
            if (_undoList.Count == MAX_SAVED_DRAWINGS)
            {
                var firstDrawing = _undoList[0];
                _undoList.RemoveAt(0);
                Destroy(firstDrawing);
            }
            
            // Add the new drawing to the list
            _undoList.Add(drawing);
            drawing.SetActive(false);
        }
    }
}
