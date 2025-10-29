using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationEvent
{
    public string locationName;
    public List<string> eventFiles;
}

public class RandomEncounters : MonoBehaviour
{
    public static RandomEncounters Instance { get; private set; }

    [Header("Assign Events Per Location")]
    public List<LocationEvent> locationEvents = new List<LocationEvent>();

    private Dictionary<string, List<string>> locationEventMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            locationEventMap = new Dictionary<string, List<string>>();
            foreach (var entry in locationEvents)
            {
                if (!locationEventMap.ContainsKey(entry.locationName))
                {
                    locationEventMap.Add(entry.locationName, entry.eventFiles);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<string> GetEventsForLocation(string location)
    {
        if (locationEventMap != null && locationEventMap.ContainsKey(location))
            return locationEventMap[location];
        return null;
    }
}
