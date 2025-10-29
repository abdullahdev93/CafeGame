using System.Collections.Generic;
using UnityEngine;

public class SnapPointManager : MonoBehaviour
{
    public static SnapPointManager Instance;
    public List<SnapPoint> snapPoints; // List of snap points in the scene
    public float snapDistance = 1.0f; // Maximum distance to snap to a point
    public QuoteMenu quoteMenu; // Reference to the QuoteMenu
    public PlayerStats playerStats; // Reference to the PlayerStats
    private Transform currentSnappedItem;
    private int currentSnapPointIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (currentSnappedItem != null)
        {
            SnapPoint currentSnapPoint = snapPoints[currentSnapPointIndex - 1];
            float distance = Vector3.Distance(currentSnappedItem.position, currentSnapPoint.transform.position);

            // Check if the snapped item is moved away or destroyed
            if (distance > snapDistance || currentSnappedItem == null || !currentSnappedItem.gameObject.activeInHierarchy)
            {
                var itemComponent = currentSnappedItem.GetComponent<ItemComponent>();
                if (itemComponent != null)
                {
                    itemComponent.SetSnapped(true, currentSnapPoint.SnapLocation);
                }

                currentSnappedItem = null;
            }
        }
    } 

    public void SnapItemToClosestPoint(Transform item)
    {
        if (IsItemSnappable(item))
        {
            SnapPoint closestSnapPoint = GetClosestSnapPoint(item);
            if (closestSnapPoint != null && Vector3.Distance(item.position, closestSnapPoint.transform.position) <= snapDistance)
            {
                // Snap the item to the closest point
                item.position = new Vector3(closestSnapPoint.SnapLocation.x, closestSnapPoint.SnapLocation.y, item.position.z);
                int snapPointIndex = snapPoints.IndexOf(closestSnapPoint) + 1;
                quoteMenu.SnapItem(item.gameObject, snapPointIndex); // Notify QuoteMenu of the snapped item and which snap point

                currentSnappedItem = item;
                currentSnapPointIndex = snapPointIndex;

                // Set snapped state for different items and subscribe to destruction events
                SetItemSnappedState(item, true, closestSnapPoint.SnapLocation);
            }
            else
            {
                SetItemSnappedState(item, false, Vector2.zero);
            }
        }
    }

    private bool IsItemSnappable(Transform item)
    {
        string tag = item.tag.ToLower();
        return !(tag == "coffeebeans" || tag == "sprinkleshaker" || tag == "whipcream" || tag == "cupstack" || tag == "strawberries" || tag == "blueberries" || tag == "bananas"); 
    }

    private SnapPoint GetClosestSnapPoint(Transform item)
    {
        SnapPoint closestPoint = null;
        float minDistance = Mathf.Infinity;

        foreach (SnapPoint snapPoint in snapPoints)
        {
            float distance = Vector3.Distance(item.position, snapPoint.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = snapPoint;
            }
        }

        return closestPoint;
    }

    public void SetEmpathyLevel(int empathyLevel)
    {
        quoteMenu.SetEmpathyLevel(empathyLevel);
    }

    private void SetItemSnappedState(Transform item, bool snapped, Vector2 snapPosition)
    {
        var itemComponent = item.GetComponent<ItemComponent>();
        if (itemComponent != null)
        {
            itemComponent.SetSnapped(snapped, snapPosition);
            if (snapped) itemComponent.OnDestroyed += HandleItemDestroyed;
        }

        var cup = item.GetComponent<Cup>();
        if (cup != null)
        {
            cup.SetSnapped(snapped, snapPosition);
            if (snapped) cup.OnDestroyed += HandleItemDestroyed;
        }

        var cupOfCoffee = item.GetComponent<CupOfCoffee>();
        if (cupOfCoffee != null)
        {
            cupOfCoffee.SetSnapped(snapped, snapPosition);
            if (snapped) cupOfCoffee.OnDestroyed += HandleItemDestroyed;
        }

        var cupOfLemonade = item.GetComponent<CupOfLemonade>();
        if (cupOfLemonade != null)
        {
            cupOfLemonade.SetSnapped(snapped, snapPosition);
            if (snapped) cupOfLemonade.OnDestroyed += HandleItemDestroyed;
        }

        var cupOfWhippedCoffee = item.GetComponent<CupOfWhippedCoffee>();
        if (cupOfWhippedCoffee != null)
        {
            cupOfWhippedCoffee.SetSnapped(snapped, snapPosition);
            if (snapped) cupOfWhippedCoffee.OnDestroyed += HandleItemDestroyed;
        }

        var cupOfSprinkledWhippedCoffee = item.GetComponent<CupOfSprinkledWhippedCoffee>();
        if (cupOfSprinkledWhippedCoffee != null)
        {
            cupOfSprinkledWhippedCoffee.SetSnapped(snapped, snapPosition);
            if (snapped) cupOfSprinkledWhippedCoffee.OnDestroyed += HandleItemDestroyed;
        }

        var strawberrySmoothie = item.GetComponent<StrawberrySmoothie>();
        if (strawberrySmoothie != null)
        {
            strawberrySmoothie.SetSnapped(snapped, snapPosition);
            if (snapped) strawberrySmoothie.OnDestroyed += HandleItemDestroyed; 
        }

        var blueberrySmoothie = item.GetComponent<BlueberrySmoothie>();
        if (blueberrySmoothie != null)
        {
            blueberrySmoothie.SetSnapped(snapped, snapPosition);
            if (snapped) blueberrySmoothie.OnDestroyed += HandleItemDestroyed;
        }

        var bananaSmoothie = item.GetComponent<BananaSmoothie>();
        if (bananaSmoothie != null)
        {
            bananaSmoothie.SetSnapped(snapped, snapPosition);
            if (snapped) bananaSmoothie.OnDestroyed += HandleItemDestroyed;
        }

        var berrySmoothie = item.GetComponent<BerrySmoothie>(); 
        if (berrySmoothie != null)
        {
            berrySmoothie.SetSnapped(snapped, snapPosition);
            if (snapped) berrySmoothie.OnDestroyed += HandleItemDestroyed; 
        }
    }

    private void HandleItemDestroyed()
    {
        if (currentSnappedItem != null)
        {
            StartCoroutine(quoteMenu.HideButtonsTemporarily(currentSnapPointIndex, 2f));
            currentSnappedItem = null;
        }
    }
}
