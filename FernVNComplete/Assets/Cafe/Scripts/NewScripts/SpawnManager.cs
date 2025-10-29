using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public SpawnPoint[] spawnPoints;
    public float minSpawnTime = 20f;
    public float maxSpawnTime = 60f;
    public float customerTimer = 60f; // Customer timer, adjusted by difficulty curve
    public CafeItem[] cafeItems; // Array to hold cafe items

    public Dictionary<CustomerBehavior.CustomerType, GameObject> customerTypePrefabs; // Dictionary to hold prefabs for each customer type

    private bool stopSpawning = false; // Flag to stop spawning customers
    private Dictionary<Sprite, int> itemSpawnCount = new Dictionary<Sprite, int>(); // Dictionary to keep track of item counts
    private bool isGameStarted = false; // Flag to check if the game has started 

    private bool randomSpawningEnabled = true; 

    private void Start()
    {
        // Initialize the dictionary with item sprites
        foreach (var item in cafeItems)
        {
            itemSpawnCount[item.sprite] = 0;
        }

        // Initialize the dictionary with customer type prefabs
        customerTypePrefabs = new Dictionary<CustomerBehavior.CustomerType, GameObject>
        {
            { CustomerBehavior.CustomerType.Regular, Resources.Load<GameObject>("Prefabs/RegularCustomer") },
            { CustomerBehavior.CustomerType.Impatient, Resources.Load<GameObject>("Prefabs/ImpatientCustomer") },
            { CustomerBehavior.CustomerType.GenerousTipper, Resources.Load<GameObject>("Prefabs/GenerousTipper") },
            { CustomerBehavior.CustomerType.BusyProfessional, Resources.Load<GameObject>("Prefabs/BusyProfessional") },
            { CustomerBehavior.CustomerType.SocialButterfly, Resources.Load<GameObject>("Prefabs/SocialButterfly") },
            { CustomerBehavior.CustomerType.Tourist, Resources.Load<GameObject>("Prefabs/Tourist") },
            { CustomerBehavior.CustomerType.Student, Resources.Load<GameObject>("Prefabs/Student") },
            { CustomerBehavior.CustomerType.Elderly, Resources.Load<GameObject>("Prefabs/ElderlyCustomer") },
            { CustomerBehavior.CustomerType.NightOwl, Resources.Load<GameObject>("Prefabs/NightOwl") },
            { CustomerBehavior.CustomerType.Emo, Resources.Load<GameObject>("Prefabs/EmoCustomer") },
            { CustomerBehavior.CustomerType.Athletic, Resources.Load<GameObject>("Prefabs/AthleticCustomer") },
            { CustomerBehavior.CustomerType.Sad, Resources.Load<GameObject>("Prefabs/SadCustomer") }
        };

        // You can call this method periodically to try spawning customers
        InvokeRepeating(nameof(SpawnCustomer), 2f, 5f);
    }

    public void StartGame()
    {
        isGameStarted = true;
    }

    public void StopSpawning()
    {
        stopSpawning = true;
    }

    void SpawnCustomer()
    {
        if (stopSpawning || !isGameStarted || !randomSpawningEnabled) return; 

        //if (stopSpawning || !isGameStarted) return;

        foreach (var point in spawnPoints)
        {
            if (!point.IsOccupied)
            {
                // Randomly assign a customer type
                CustomerBehavior.CustomerType randomCustomerType = (CustomerBehavior.CustomerType)Random.Range(0, System.Enum.GetValues(typeof(CustomerBehavior.CustomerType)).Length);

                // Select the corresponding prefab from the dictionary
                GameObject customerPrefab = customerTypePrefabs[randomCustomerType];
                Vector3 spawnPosition = new Vector3(point.spawnLocation.x, point.spawnLocation.y, 0);
                GameObject customer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
                point.IsOccupied = true;

                // Initialize the customer with the spawn point, random timer, and item details
                CustomerBehavior customerBehavior = customer.GetComponent<CustomerBehavior>();
                customerBehavior.customerType = randomCustomerType;
                customerBehavior.cafeItems = cafeItems; // Pass the cafe items array

                // Assign an item that is not over the limit
                int attempts = 0;
                CafeItem selectedItem = null;
                while (attempts < cafeItems.Length * 2) // Try a reasonable number of attempts
                {
                    int randomIndex = Random.Range(0, cafeItems.Length);
                    selectedItem = cafeItems[randomIndex];
                    if (itemSpawnCount[selectedItem.sprite] < 2)
                    {
                        itemSpawnCount[selectedItem.sprite]++;
                        break;
                    }
                    attempts++;
                }

                float elapsedTime = GameSession.Instance.GetElapsedTime();
                float gameProgress = elapsedTime / GameSession.Instance.GetInitialTime();
                customerBehavior.Initialize(point, gameProgress, selectedItem);

                break;
            }
        }
    }

    public CustomerBehavior SpawnTutorialCustomer(
    CustomerBehavior.CustomerType type,
    bool forceCupOfCoffee,
    bool showPatienceMeter,
    bool allowTips)
    {
        // find a free spawn point
        foreach (var point in spawnPoints)
        {
            if (!point.IsOccupied)
            {
                GameObject prefab = customerTypePrefabs[type];
                Vector3 pos = new Vector3(point.spawnLocation.x, point.spawnLocation.y, 0);
                GameObject go = Instantiate(prefab, pos, Quaternion.identity);
                point.IsOccupied = true;

                // choose forced Cup-of-Coffee item from cafeItems by name or tag
                // Here: pick the first item that matches your Cup of Coffee sprite/name convention

                // choose forced Cup-of-Coffee item by logical tag
                CafeItem coffeeItem = null;

                if (forceCupOfCoffee)
                {
                    coffeeItem = FindCafeItemByPrefabTag("CupOfCoffee"); // <- uses GameObject.tag on the prefab
                }

                if (coffeeItem == null)
                {
                    // Fallback if tag/prefab/sprite isn’t wired yet
                    coffeeItem = cafeItems[Random.Range(0, cafeItems.Length)];
                } 

                //if (!forceCupOfCoffee) coffeeItem = cafeItems[Random.Range(0, cafeItems.Length)];

                // Initialize
                var cb = go.GetComponent<CustomerBehavior>();
                cb.customerType = type;
                cb.cafeItems = cafeItems;
                cb.showPatienceUI = showPatienceMeter;
                cb.allowTipsOverride = allowTips;

                float elapsedTime = GameSession.Instance.GetElapsedTime();
                float gameProgress = elapsedTime / GameSession.Instance.GetInitialTime();
                cb.Initialize(point, gameProgress, coffeeItem);

                return cb;
            }
        }
        return null;
    }

    private CafeItem FindCafeItemByPrefabTag(string tag)
    {
        // Use any existing reference in your scene that already points to the prefab.
        var machine = FindObjectOfType<CoffeeMachineCom>();
        if (machine != null && machine.cupOfCoffee != null && machine.cupOfCoffee.CompareTag(tag))
        {
            // Get the sprite from the prefab (child-safe)
            var sr = machine.cupOfCoffee.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                var targetSprite = sr.sprite;
                // Match that sprite to your CafeItem entries (no prefab field needed)
                foreach (var ci in cafeItems)
                {
                    if (ci != null && ci.sprite == targetSprite)
                        return ci;
                }
            }
        }

        // If nothing found, return null so caller can fall back to random.
        return null;
    } 

    public void SetRandomSpawningEnabled(bool enabled)
    {
        randomSpawningEnabled = enabled;
    } 

    public void DecreaseItemCount(Sprite item)
    {
        if (itemSpawnCount.ContainsKey(item))
        {
            itemSpawnCount[item]--;
        }
    }
}
