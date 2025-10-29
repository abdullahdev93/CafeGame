using System.Collections.Generic;

public enum CafeItemList
{
    CoffeeBeans,
    EmptyCup,
    CupOfCoffee,
    WhippedCoffee,
    SprinkledWhippedCoffee,
    CoffeeMachine,
    WhipCream,
    SprinkleShaker
}

public static class CafeItemData
{
    public static Dictionary<CafeItemList, float> Prices = new Dictionary<CafeItemList, float>
    {
        //{ CafeItemList.CoffeeBeans, 1.0f },
        //{ CafeItemList.EmptyCup, 0.5f },
        { CafeItemList.CupOfCoffee, 3.0f },
        { CafeItemList.WhippedCoffee, 5.0f },
        { CafeItemList.SprinkledWhippedCoffee, 7.0f },
        //{ CafeItemList.CoffeeMachine, 100.0f },
        //{ CafeItemList.WhipCream, 2.0f },
        //{ CafeItemList.SprinkleShaker, 1.5f }
    };

    public static float GetPrice(CafeItemList item)
    {
        return Prices[item];
    }
} 