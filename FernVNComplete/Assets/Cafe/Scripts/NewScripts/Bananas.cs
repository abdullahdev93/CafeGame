using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bananas : MonoBehaviour
{
    [Header("Movement Parameters")]
    public Vector2 bananaSlots;
    public float speed;

    private bool shouldMove = false;
    private Collider2D collider2D;

    void Start()
    {
        // Get the Collider2D component
        collider2D = GetComponent<Collider2D>();
    }

    void OnMouseUp()
    {
        shouldMove = true;
        // Disable the collider to prevent collisions while moving
        collider2D.enabled = false;
    }

    void Update()
    {
        if (shouldMove)
        {
            // Move the object towards the target slot
            transform.position = Vector2.MoveTowards(transform.position, bananaSlots, speed * Time.deltaTime);

            // Check if the object has reached the target slot
            if (Vector2.Distance(transform.position, bananaSlots) < 0.01f)
            {
                transform.position = bananaSlots;
                shouldMove = false;
                // Re-enable the collider after reaching the target
                StartCoroutine(ReenableColliderAfterDelay(0.1f));
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
        BananaBin bananaBin = FindObjectOfType<BananaBin>();
        if (bananaBin != null)
        {
            bananaBin.BananaSlicesDestroyed(); 
        }
    }
}
