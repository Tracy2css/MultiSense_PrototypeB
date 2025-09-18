using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterTextMeshProPivot : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetPivotAndAdjustPosition(new Vector2(0.5f, 0.5f));
    }

    public void SetPivotAndAdjustPosition(Vector2 newPivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - newPivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);

        // Adjust pivot
        rectTransform.pivot = newPivot;
        // Adjust position to offset the change in pivot
        rectTransform.localPosition -= deltaPosition;

        // Ensure anchored position is centered
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
