using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public bool quoted = false;
    public bool snapped = false;
    public QuoteMenu.Quote quote; // Store the quote
    public Vector2 snapPosition;
    public float snapSpeed = 5f;

    private bool shouldMoveBack = false;

    public delegate void ItemDestroyed();
    public event ItemDestroyed OnDestroyed;

    void Update()
    {
        if (snapped && shouldMoveBack)
        {
            // Move back to the snap position
            transform.position = Vector2.MoveTowards(transform.position, snapPosition, snapSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, snapPosition) < 0.01f)
            {
                transform.position = snapPosition;
                shouldMoveBack = false;
            }
        }
    }

    public void SetSnapped(bool isSnapped, Vector2 snapPos)
    {
        snapped = isSnapped;
        snapPosition = snapPos;
        if (isSnapped)
        {
            shouldMoveBack = true;
        }
    }

    private void OnDestroy()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed.Invoke();
        }
    }
}
