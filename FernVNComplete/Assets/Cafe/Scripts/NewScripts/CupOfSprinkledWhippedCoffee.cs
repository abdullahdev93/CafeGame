using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupOfSprinkledWhippedCoffee : MonoBehaviour
{
    [Header("Movement Parameters")]
    public Vector2 coffeeSlots;
    public float speed;
    public float snapDistance = 1.0f; // Maximum distance to snap to a point

    private bool shouldMove = false;
    public bool isSnapped = false;
    public Vector2 snapPosition;
    public float snapSpeed = 5f;
    private bool shouldMoveBack = false;
    private Collider2D collider2D;
    private SnapPointManager snapPointManager;
    private CoffeeMachineCom coffeeMachineCom;

    public delegate void ItemDestroyed();
    public event ItemDestroyed OnDestroyed;

    void Start()
    {
        // Get the Collider2D component
        collider2D = GetComponent<Collider2D>();
        // Find the SnapPointManager in the scene
        snapPointManager = FindObjectOfType<SnapPointManager>();
        // Find the CoffeeMachineCom in the scene
        coffeeMachineCom = FindObjectOfType<CoffeeMachineCom>();
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
            shouldMove = false; // Disable movement towards the coffee machine if snapped
            shouldMoveBack = true; // Ensure that the cup can move back to the snap position if it moves away
            if (coffeeMachineCom != null)
            {
                coffeeMachineCom.CoffeeMovedAway();
            }
        }
    }

    void Update()
    {
        if (shouldMove && !isSnapped)
        {
            // Move the object towards the target slot
            transform.position = Vector2.MoveTowards(transform.position, coffeeSlots, speed * Time.deltaTime);

            // Check if the object has reached the target slot
            if (Vector2.Distance(transform.position, coffeeSlots) < 0.01f)
            {
                transform.position = coffeeSlots;
                shouldMove = false;
                // Re-enable the collider after reaching the target
                StartCoroutine(ReenableColliderAfterDelay(0.1f));
                CheckReturnToMachine();
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
        if (coffeeMachineCom != null)
        {
            coffeeMachineCom.CoffeeDestroyed();
        }
    }

    public void SetSnapped(bool snapped, Vector2 snapPos)
    {
        isSnapped = snapped;
        snapPosition = snapPos;

        // If the cup is snapped, ensure it only moves back to the snap position
        shouldMove = !snapped;
        shouldMoveBack = snapped;

        if (snapped)
        {
            if (coffeeMachineCom != null)
            {
                coffeeMachineCom.DisableReturnToMachine();  // Disable return logic to the Coffee Machine when snapped
            }
        }
    }

    private void CheckReturnToMachine()
    {
        // Only return to the machine if not snapped
        if (!isSnapped && coffeeMachineCom != null)
        {
            coffeeMachineCom.allowReturnToMachine = true;
        }
    }
}
