using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinkleShaker : MonoBehaviour
{
    [Header("Movement Parameters")]
    public Vector2 sprinkleShakerSlot;
    public float speed;

    private bool shouldMove = false;
    private Collider2D collider2D;

    void Start()
    {
        // Get the Collider2D component
        collider2D = GetComponent<Collider2D>();
    } 

    void OnMouseDown()
    {
        // Rotate the bottle to pouring position
        transform.rotation = Quaternion.Euler(0f, 0f, 170f);  
    }

    void OnMouseUp()
    {
        // Rotate the bottle back to the original position and start moving towards the slot
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        shouldMove = true;
        collider2D.enabled = false; 
    }

    void Update()
    {
        if (shouldMove)
        {
            // Move the bottle towards the whipCreamSlot
            transform.position = Vector2.MoveTowards(transform.position, sprinkleShakerSlot, speed * Time.deltaTime);

            // Check if the bottle has reached the whipCreamSlot
            if (Vector2.Distance(transform.position, sprinkleShakerSlot) < 0.01f)
            {
                transform.position = sprinkleShakerSlot;
                shouldMove = false;
                collider2D.enabled = true; 
            }
        }
    }
}
