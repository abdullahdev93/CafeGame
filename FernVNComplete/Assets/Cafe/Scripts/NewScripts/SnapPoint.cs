using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public Vector2 SnapLocation;

    void Start()
    {
        // Update SnapLocation to the current position of the Snap Point in the scene
        SnapLocation = new Vector2(transform.position.x, transform.position.y);
    } 
}
