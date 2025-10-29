using UnityEngine;

public class ActivityTracker : MonoBehaviour
{
    public static ActivityTracker Instance;

    [SerializeField] private string currentActivity = "None"; // Default activity 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ActivityTracker Initialized."); // Debugging Step
        }
        else
        {
            Debug.LogWarning("Duplicate ActivityTracker detected. Destroying this instance."); 
            Destroy(gameObject);
        }
    }

    //private void Update()
    //{
    //GetSelectedActivity(); 
    //}

    public void SetActivity(string activity)
    {
        if (!string.IsNullOrEmpty(activity))
        {
            currentActivity = activity;
            Debug.Log("Activity Successfully Updated in ActivityTracker: " + currentActivity);
        }
    } 

    public string GetSelectedActivity()
    {
        Debug.Log("Retrieved Activity: " + currentActivity); // Debugging 
        return currentActivity;
    }
}
