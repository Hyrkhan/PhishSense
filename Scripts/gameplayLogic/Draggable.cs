using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private RectTransform canvasRectTransform;
    private RectTransform objectRectTransform;

    void Start()
    {
        // Get the RectTransform of the Canvas and the object to be dragged
        canvasRectTransform = transform.parent.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        objectRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - mousePosition;
                //Debug.Log("Drag Started");

                // Bring the object to a specific position in the hierarchy when clicked
                BringToSpecificSiblingPosition();
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            Vector3 newPosition = mousePosition + offset;

            // Clamp position to canvas boundaries
            newPosition = ClampToCanvas(newPosition);

            transform.position = newPosition;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            //Debug.Log("Drag Stopped");
        }
    }

    private Vector3 ClampToCanvas(Vector3 position)
    {
        // Convert position to local space relative to the canvas
        Vector3 localPosition = canvasRectTransform.InverseTransformPoint(position);

        // Get the size of the canvas and the draggable object's size
        Vector2 canvasSize = canvasRectTransform.rect.size;
        Vector2 objectSize = objectRectTransform.rect.size;

        // Clamp the position to the canvas boundaries
        localPosition.x = Mathf.Clamp(localPosition.x, -canvasSize.x / 2 + objectSize.x / 2, canvasSize.x / 2 - objectSize.x / 2);
        localPosition.y = Mathf.Clamp(localPosition.y, -canvasSize.y / 2 + objectSize.y / 2, canvasSize.y / 2 - objectSize.y / 2);

        // Convert the local position back to world space
        return canvasRectTransform.TransformPoint(localPosition);
    }

    // Function to bring the object to a specific position in the hierarchy
    public void BringToSpecificSiblingPosition()
    {
        // Get the parent of this object
        Transform parentTransform = transform.parent;

        // Check how many siblings there are
        int siblingCount = parentTransform.childCount;

        // Calculate the index of the fourth last sibling
        int targetSiblingIndex = Mathf.Max(siblingCount - 5, 0);

        // Set the new sibling index (move the object before the fourth last sibling)
        transform.SetSiblingIndex(targetSiblingIndex);
    }
}