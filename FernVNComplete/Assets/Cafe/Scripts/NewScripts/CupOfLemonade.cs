using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupOfLemonade : MonoBehaviour
{
    [Header("Movement Parameters")]
    public Vector2 lemonadeSlots;
    public float speed;
    public float snapDistance = 1.0f; // Maximum distance to snap to a point

    private bool shouldMove = false;
    public bool isSnapped = false;
    public Vector2 snapPosition;
    public float snapSpeed = 5f;
    private bool shouldMoveBack = false;
    private Collider2D collider2D;
    private SnapPointManager snapPointManager;
    private LemonadeDispenser lemonadeDispenser;

    public delegate void ItemDestroyed();
    public event ItemDestroyed OnDestroyed;

    void Start()
    {
        // Get the Collider2D component
        collider2D = GetComponent<Collider2D>();
        // Find the SnapPointManager in the scene
        snapPointManager = FindObjectOfType<SnapPointManager>();
        // Find the LemonadeDispenser in the scene
        lemonadeDispenser = FindObjectOfType<LemonadeDispenser>();
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
        else if (lemonadeDispenser != null)
        {
            lemonadeDispenser.LemonadeMovedAway();
        }
    }

    void Update()
    {
        if (shouldMove)
        {
            // Move the object towards the target slot
            transform.position = Vector2.MoveTowards(transform.position, lemonadeSlots, speed * Time.deltaTime);

            // Check if the object has reached the target slot
            if (Vector2.Distance(transform.position, lemonadeSlots) < 0.01f)
            {
                transform.position = lemonadeSlots;
                shouldMove = false;
                // Re-enable the collider after reaching the target
                StartCoroutine(ReenableColliderAfterDelay(0.1f));
                CheckReturnToDispenser();
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
        if (lemonadeDispenser != null)
        {
            lemonadeDispenser.LemonadeDestroyed();
        }
    }

    public void SetSnapped(bool snapped, Vector2 snapPos)
    {
        isSnapped = snapped;
        snapPosition = snapPos;
        if (isSnapped && lemonadeDispenser != null)
        {
            lemonadeDispenser.EnableCollider();
        }
        if (snapped)
        {
            shouldMoveBack = true;
        }
    }

    private void CheckReturnToDispenser()
    {
        if (!isSnapped && lemonadeDispenser != null)
        {
            lemonadeDispenser.CheckForDuplicateCups(gameObject);
        }
    }
}
