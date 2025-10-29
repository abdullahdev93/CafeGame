using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaSmoothie : MonoBehaviour
{
    [Header("Movement Parameters")]
    public Vector2 smoothieSlots;
    public float speed;
    public float snapDistance = 1.0f; // Maximum distance to snap to a point

    private bool shouldMove = false;
    public bool isSnapped = false;
    public Vector2 snapPosition;
    public float snapSpeed = 5f;
    private bool shouldMoveBack = false;
    private Collider2D collider2D;
    private SnapPointManager snapPointManager;
    private Blender blender;

    public delegate void ItemDestroyed();
    public event ItemDestroyed OnDestroyed;

    void Start()
    {
        // Get the Collider2D component
        collider2D = GetComponent<Collider2D>();
        // Find the SnapPointManager in the scene
        snapPointManager = FindObjectOfType<SnapPointManager>();
        // Find the Blender in the scene
        blender = FindObjectOfType<Blender>();
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
        else
        {
            shouldMove = false; // Disable movement towards the blender if snapped
            shouldMoveBack = true; // Ensure that the smoothie can move back to the snap position if it moves away
            if (blender != null)
            {
                blender.SmoothieMovedAway();
            }
        }
    }

    void Update()
    {
        if (shouldMove && !isSnapped)
        {
            // Move the object towards the target slot
            transform.position = Vector2.MoveTowards(transform.position, smoothieSlots, speed * Time.deltaTime);

            // Check if the object has reached the target slot
            if (Vector2.Distance(transform.position, smoothieSlots) < 0.01f)
            {
                transform.position = smoothieSlots;
                shouldMove = false;
                // Re-enable the collider after reaching the target
                StartCoroutine(ReenableColliderAfterDelay(0.1f));
                CheckReturnToBlender();
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
        if (blender != null)
        {
            blender.SmoothieDestroyed();
        }
    }

    public void SetSnapped(bool snapped, Vector2 snapPos)
    {
        isSnapped = snapped;
        snapPosition = snapPos;

        // If the smoothie is snapped, ensure it only moves back to the snap position
        shouldMove = !snapped;
        shouldMoveBack = snapped;

        if (snapped)
        {
            if (blender != null)
            {
                blender.DisableReturnToBlender();  // Disable return logic to the Blender when snapped
            }
        }
    }

    private void CheckReturnToBlender()
    {
        // Only return to the blender if not snapped
        if (!isSnapped && blender != null)
        {
            blender.allowReturnToBlender = true;
            // blender.CheckForDuplicateSmoothies(gameObject); // Implement if needed
        }
    }
}
