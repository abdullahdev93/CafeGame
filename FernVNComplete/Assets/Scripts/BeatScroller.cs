using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo;

    public bool hasStarted;

    void Start()
    {
        beatTempo = beatTempo / 60f; // Convert tempo from BPM to speed
    }

    void Update()
    {
        if (hasStarted)
        {
            transform.position -= new Vector3(beatTempo * Time.deltaTime, 0f, 0f);
        }
    }
}

