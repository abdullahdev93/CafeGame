using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public AudioClip trashSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("WhipCream") || other.gameObject.CompareTag("SprinkleShaker"))  
            return;

        //TheAudioManager.Instance.PlaySound(trashSound);
        Destroy(other.gameObject); 
    }
}
