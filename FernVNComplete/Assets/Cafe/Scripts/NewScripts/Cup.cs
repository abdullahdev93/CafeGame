using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [Header("Movement Parameters")]
    public Vector2 cupSlots;
    public float speed;
    public float snapDistance = 1.0f; // Maximum distance to snap to a point

    private bool shouldMove = false;
    public bool isSnapped = false;
    public Vector2 snapPosition;
    public float snapSpeed = 5f;
    private bool shouldMoveBack = false;
    private Collider2D collider2D;
    private SnapPointManager snapPointManager;
    private CupStack cupStack;

    public delegate void ItemDestroyed();
    public event ItemDestroyed OnDestroyed;

    void Start()
    {
        // Get the Collider2D component
        collider2D = GetComponent<Collider2D>();
        // Find the SnapPointManager in the scene
        snapPointManager = FindObjectOfType<SnapPointManager>();
        // Find the CupStack in the scene
        cupStack = FindObjectOfType<CupStack>();
    }

    public void OnMouseUp()
    {
        // Attempt to snap to a snap point
        snapPointManager.SnapItemToClosestPoint(transform);
        if (!isSnapped)
        {
            shouldMove = true;
            // Disable the collider to prevent collisions while moving
            collider2D.enabled = false;
        }
        else if (cupStack != null)
        {
            cupStack.CupMovedAway();
        }
    }

    void Update()
    {
        if (shouldMove)
        {
            // Move the object towards the target slot
            transform.position = Vector2.MoveTowards(transform.position, cupSlots, speed * Time.deltaTime);

            // Check if the object has reached the target slot
            if (Vector2.Distance(transform.position, cupSlots) < 0.01f)
            {
                transform.position = cupSlots;
                shouldMove = false;
                // Re-enable the collider after reaching the target
                StartCoroutine(ReenableColliderAfterDelay(0.1f));
                CheckReturnToStack();
            }
        }

        if (isSnapped && shouldMoveBack)
        {
            // Move back to the snap position
            transform.position = Vector2.MoveTowards(transform.position, snapPosition, snapSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, snapPosition) < 0.01f)
            {
                transform.position = snapPosition;
                shouldMoveBack = false;
            }
        }
    }

    private IEnumerator ReenableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        collider2D.enabled = true;
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
        if (cupStack != null)
        {
            cupStack.CupDestroyed();
        }
    }

    public void SetSnapped(bool snapped, Vector2 snapPos)
    {
        isSnapped = snapped;
        snapPosition = snapPos;
        if (isSnapped && cupStack != null)
        {
            cupStack.EnableCollider();
        }
        if (snapped)
        {
            shouldMoveBack = true;
        }
    }

    private void CheckReturnToStack()
    {
        if (!isSnapped && cupStack != null)
        {
            cupStack.CheckForDuplicateCups(gameObject);
        }
    }
}
