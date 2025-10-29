using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFloatingText : MonoBehaviour
{
    public float secondsToDestroy; 

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, secondsToDestroy); 
    } 
}
