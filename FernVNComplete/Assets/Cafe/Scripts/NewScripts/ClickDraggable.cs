using UnityEngine;

public class ClickDraggable : MonoBehaviour
{
    private Vector3 dragOffset;
    private Camera cam;
    private SnapPointManager snapPointManager;
    private bool isDragging = false;

    private Collider2D collider2D;

    void Awake()
    {
        cam = Camera.main;
        snapPointManager = FindObjectOfType<SnapPointManager>();
        collider2D = GetComponent<Collider2D>();
    }

    void OnMouseDown()
    {
        dragOffset = transform.position - GetMousePos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        transform.position = GetMousePos() + dragOffset;
    }

    void OnMouseUp()
    {
        isDragging = false;
        snapPointManager.SnapItemToClosestPoint(transform);
        if (!GetComponent<Cup>().isSnapped)
        {
            GetComponent<Cup>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<CupOfCoffee>().isSnapped) 
        {
            GetComponent<CupOfCoffee>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<CupOfWhippedCoffee>().isSnapped)
        {
            GetComponent<CupOfWhippedCoffee>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<CupOfSprinkledWhippedCoffee>().isSnapped)
        {
            GetComponent<CupOfSprinkledWhippedCoffee>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<CupOfLemonade>().isSnapped)
        {
            GetComponent<CupOfLemonade>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<StrawberrySmoothie>().isSnapped)
        {
            GetComponent<StrawberrySmoothie>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<BlueberrySmoothie>().isSnapped)
        {
            GetComponent<BlueberrySmoothie>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<BananaSmoothie>().isSnapped)
        {
            GetComponent<BananaSmoothie>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

        if (!GetComponent<BerrySmoothie>().isSnapped) 
        {
            GetComponent<BerrySmoothie>().OnMouseUp(); // Ensure the cup checks if it should move back to the stack
        }

    }

    Vector3 GetMousePos()
    {
        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    public bool IsDragging()
    {
        return isDragging;
    }
}
