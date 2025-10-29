using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    public Animator earningsAnimator; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            earningsAnimator.SetTrigger("CafeMoneyAdded");
        }
    }
} 
