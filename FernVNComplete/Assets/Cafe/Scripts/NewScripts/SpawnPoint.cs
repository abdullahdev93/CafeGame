using UnityEngine;
using UnityEngine.UI;

public class SpawnPoint : MonoBehaviour
{
    public Vector2 spawnLocation;
    public bool IsOccupied { get; set; }

    private void Start()
    {
        // Initialize and hide the timer image at the start
        Image timerImage = transform.Find("TimerImage")?.GetComponent<Image>();
        if (timerImage != null)
        {
            timerImage.gameObject.SetActive(false); // enabled = false;
        }
    }
}
