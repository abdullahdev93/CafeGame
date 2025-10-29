using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    // This method is triggered when a note enters the Kill Zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject); 
    }
}
