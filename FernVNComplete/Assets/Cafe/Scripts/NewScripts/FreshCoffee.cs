using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreshCoffee : MonoBehaviour
{
    public static event System.Action OnCupOfCoffeeSpawned;   // <- ADD 

    public GameObject cupOfCoffee; 

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("EmptyCup")) 
        { 
            Destroy(other.gameObject);
            Destroy(this.gameObject); 
            Instantiate(cupOfCoffee, transform.position - new Vector3(0, 2.5f, 0), transform.rotation);

            OnCupOfCoffeeSpawned?.Invoke();            // <- FIRE EVENT 
        } 
    }
}
