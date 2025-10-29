using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonVisibility : MonoBehaviour
{
    public void HoverOverButtons() 
    {
        TownMapManager.instance.toCafeButton.gameObject.SetActive(true);
        TownMapManager.instance.toApartmentButton.gameObject.SetActive(true); 
    }

    public void HoverAwayFromButtons() 
    {
        TownMapManager.instance.toCafeButton.gameObject.SetActive(false);
        TownMapManager.instance.toApartmentButton.gameObject.SetActive(false); 
    }
}
