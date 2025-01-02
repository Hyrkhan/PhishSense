using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenResizing : MonoBehaviour
{
    public RectTransform coverPanel; 

    void Start()
    {
        AdjustScreen(coverPanel, 0.4515f, 0.4975f, 0.4065f, 0.5085f);
    }

    /// <summary>
    /// Adjusts the size and position of a given RectTransform based on proportional anchors.
    /// </summary>
    /// <param name="rectTransform">The RectTransform to adjust.</param>
    /// <param name="leftPercentage">Proportional left anchor (0 to 1).</param>
    /// <param name="rightPercentage">Proportional right anchor (0 to 1).</param>
    /// <param name="topPercentage">Proportional top anchor (0 to 1).</param>
    /// <param name="bottomPercentage">Proportional bottom anchor (0 to 1).</param>
    void AdjustScreen(RectTransform rectTransform, float leftPercentage, float rightPercentage, float topPercentage, float bottomPercentage)
    {
        // Set the proportional anchors
        rectTransform.anchorMin = new Vector2(leftPercentage, bottomPercentage);
        rectTransform.anchorMax = new Vector2(rightPercentage, topPercentage);

        // Maintain original offsets to preserve size
        Vector2 currentOffsetMin = rectTransform.offsetMin;
        Vector2 currentOffsetMax = rectTransform.offsetMax;

        rectTransform.offsetMin = currentOffsetMin;
        rectTransform.offsetMax = currentOffsetMax;
    }
}
