using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public bool isLongNote;
    public bool isRapidPressNote;
    public bool noteHit;
    public KeyCode keyToPress;

    public GameObject OKEffect;
    public GameObject goodEffect;
    public GameObject perfectEffect;
    public GameObject missEffect;

    public float holdTimer = 0f;
    public float requiredHoldTime = 1f;
    private bool keyHeldDown = false;

    private Transform[] activatorTransforms;  // Array to hold Activators
    private int activatorIndex;               // Index to determine which activator to use

    // For rapid pressing notes
    public float rapidPressCount = 0;
    public float requiredPresses = 10; // Total presses required to hit the note

    void Start()
    {
        // Find the Activators in the scene and assign them in order
        activatorTransforms = new Transform[4];
        activatorTransforms[0] = GameObject.Find("ActivatorUp").transform;    // Up Arrow Activator
        activatorTransforms[1] = GameObject.Find("ActivatorRight").transform; // Right Arrow Activator
        activatorTransforms[2] = GameObject.Find("ActivatorLeft").transform;  // Left Arrow Activator
        activatorTransforms[3] = GameObject.Find("ActivatorDown").transform;  // Down Arrow Activator

        noteHit = false;
        keyHeldDown = false;
    }

    void Update()
    {
        // Handle key press for normal and rapid press notes
        if (Input.GetKeyDown(keyToPress) && canBePressed && !noteHit)
        {
            noteHit = true;
            SetActivatorIndex(); // Update the activator based on the key press

            if (isLongNote && !isRapidPressNote)
            {
                keyHeldDown = true; // Long note hold
            }
            else if (isRapidPressNote)
            {
                keyHeldDown = true; // Enable rapid press
            }
            else
            {
                // Evaluate the distance to the activator for regular notes
                EvaluateNoteHit();
            }
        }

        // Increment the hold time for long notes
        if (isLongNote && keyHeldDown && Input.GetKey(keyToPress) && !isRapidPressNote)
        {
            holdTimer += Time.deltaTime;
        }

        // Handle rapid pressing notes
        if (isRapidPressNote && keyHeldDown)
        {
            // Detect rapid key presses
            if (Input.GetKeyDown(keyToPress))
            { 
                rapidPressCount++;
                KaraokeManager.instance.currentMuliplier++; 
                // Evaluate the result continuously as the player presses keys
                //EvaluateRapidPressResult();

                if (rapidPressCount >= requiredPresses) 
                {
                    keyHeldDown = false;
                    gameObject.SetActive(false); // Deactivate note after reaching required presses
                }
            }
        }

        // Evaluate when key is released for long notes
        if (isLongNote && Input.GetKeyUp(keyToPress) && keyHeldDown)
        {
            EvaluateHoldNoteResult();
            keyHeldDown = false;
            gameObject.SetActive(false);
        }
    }

    // New method to evaluate how well the note was hit based on distance
    private void EvaluateNoteHit()
    { 

        float distanceToActivator = Vector2.Distance(transform.position, activatorTransforms[activatorIndex].position);

        // Thresholds for deciding the hit quality
        float perfectThreshold = 0.1f;  // Very close for Perfect
        float goodThreshold = 0.2f;     // Slightly farther for Good
        //float okThreshold = 0.3f;       // Even farther for OK

        if (distanceToActivator <= perfectThreshold)
        {
            KaraokeManager.instance.IncreaseRhythmMeter(); // Increase rhythm meter for hitting the note  
            KaraokeManager.instance.PerfectHit();
            Instantiate(perfectEffect, activatorTransforms[activatorIndex].position, perfectEffect.transform.rotation);
        }
        else if (distanceToActivator <= goodThreshold) 
        {
            KaraokeManager.instance.IncreaseRhythmMeter(); // Increase rhythm meter for hitting the note   
            KaraokeManager.instance.GoodHit();
            Instantiate(goodEffect, activatorTransforms[activatorIndex].position, goodEffect.transform.rotation);
        }
        //else if (distanceToActivator <= okThreshold)
        //{
            //KaraokeManager.instance.OKHit();
            //Instantiate(OKEffect, activatorTransforms[activatorIndex].position, OKEffect.transform.rotation);
        //}
        else
        {
            KaraokeManager.instance.IncreaseRhythmMeter(); // Increase rhythm meter for hitting the note  
            KaraokeManager.instance.OKHit();
            Instantiate(OKEffect, activatorTransforms[activatorIndex].position, OKEffect.transform.rotation);
        }

        // Deactivate the note after evaluating
        gameObject.SetActive(false);
    }

    // Set the activator index based on the key press
    private void SetActivatorIndex()
    {
        if (keyToPress == KeyCode.UpArrow)
        {
            activatorIndex = 0;
        }
        else if (keyToPress == KeyCode.RightArrow)
        {
            activatorIndex = 1;
        }
        else if (keyToPress == KeyCode.LeftArrow)
        {
            activatorIndex = 2;
        }
        else if (keyToPress == KeyCode.DownArrow)
        {
            activatorIndex = 3;
        }
    }

    private void EvaluateHoldNoteResult()
    {
        if (holdTimer >= requiredHoldTime)
        {
            KaraokeManager.instance.PerfectHold();
            Instantiate(perfectEffect, activatorTransforms[activatorIndex].position, perfectEffect.transform.rotation);
        }
        else if (holdTimer >= requiredHoldTime * 0.75f)
        {
            KaraokeManager.instance.GoodHold();
            Instantiate(goodEffect, activatorTransforms[activatorIndex].position, goodEffect.transform.rotation);
        }
        else if (holdTimer >= requiredHoldTime * 0.5f)
        {
            KaraokeManager.instance.OKHold();
            Instantiate(OKEffect, activatorTransforms[activatorIndex].position, OKEffect.transform.rotation);
        }
        else
        {
            KaraokeManager.instance.NoteMissed();
            Instantiate(missEffect, activatorTransforms[activatorIndex].position, missEffect.transform.rotation);
        }
    }

    // Evaluate the result of the rapid presses (based only on press count)
    private void EvaluateRapidPressResult()
    {
        // Check if rapidPressCount exactly equals requiredPresses for Perfect feedback
        if (rapidPressCount >= requiredPresses)
        {
            KaraokeManager.instance.PerfectRapidPress();
            Instantiate(perfectEffect, activatorTransforms[activatorIndex].position, perfectEffect.transform.rotation);
        }
        // Check if rapidPressCount is between 75% and less than 100% of required presses for Good feedback
        else if (rapidPressCount >= (requiredPresses * 0.75f) && rapidPressCount < requiredPresses)
        {
            KaraokeManager.instance.GoodRapidPress();
            Instantiate(goodEffect, activatorTransforms[activatorIndex].position, goodEffect.transform.rotation);
        }
        // Check if rapidPressCount is between 50% and less than 75% of required presses for OK feedback
        else if (rapidPressCount >= (requiredPresses * 0.5f) && rapidPressCount < (requiredPresses * 0.75f))
        {
            KaraokeManager.instance.OKRapidPress();
            Instantiate(OKEffect, activatorTransforms[activatorIndex].position, OKEffect.transform.rotation);
        }
        // If rapidPressCount is below 50% of the required presses, it's considered a Miss
        else
        {
            KaraokeManager.instance.NoteMissed();
            Instantiate(missEffect, activatorTransforms[activatorIndex].position, missEffect.transform.rotation);
        }

        // Ensure the object remains active until feedback is displayed
        //StartCoroutine(DeactivateAfterFeedback());
    } 

    // Deactivate the note object after displaying the feedback
    private IEnumerator DeactivateAfterFeedback()
    {
        yield return new WaitForSeconds(0.5f); // Give enough time for the feedback to show
        gameObject.SetActive(false);  // Deactivate the note object after feedback
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = true;

            // Set the activator index based on the Activator the note is currently over
            if (other.name == "ActivatorUp")
            {
                activatorIndex = 0;
            }
            else if (other.name == "ActivatorRight")
            {
                activatorIndex = 1;
            }
            else if (other.name == "ActivatorLeft")
            {
                activatorIndex = 2;
            }
            else if (other.name == "ActivatorDown")
            {
                activatorIndex = 3; 
            }
        }
    } 

    private void OnTriggerExit2D(Collider2D other) 
    { 

        if (other.tag == "Activator" && canBePressed && !noteHit) 
        {
            KaraokeManager.instance.DecreaseRhythmMeter(); // Decrease rhythm meter for missed note  
            KaraokeManager.instance.NoteMissed(); 
            Instantiate(missEffect, activatorTransforms[activatorIndex].position, missEffect.transform.rotation);
        }

        // If this is a rapid press note, evaluate the result when it exits the activator
        if (isRapidPressNote)
        {
            EvaluateRapidPressResult();
        }

        canBePressed = false;

        if (!isLongNote || (isLongNote && !keyHeldDown))
        {
            gameObject.SetActive(false);
        }
    }
}
