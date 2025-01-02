using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCollider : MonoBehaviour
{
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (rectTransform != null && boxCollider != null)
        {
            // Adjust the BoxCollider2D size to match the RectTransform size
            boxCollider.size = rectTransform.rect.size;
            boxCollider.offset = Vector2.zero; // Ensure it's centered
        }
    }
}
