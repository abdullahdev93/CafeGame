using UnityEngine;

public class CafeSceneManager : MonoBehaviour
{
    private void Start()
    {
        // Instantiate GameSession and UpgradeMenu only in Cafe scene
        if (GameSession.Instance == null)
        {
            GameObject gameSessionPrefab = Resources.Load<GameObject>("GameSessionPrefab");
            Instantiate(gameSessionPrefab);
        }

        if (UpgradeMenu.Instance == null)
        {
            GameObject upgradeMenuPrefab = Resources.Load<GameObject>("UpgradeMenuPrefab");
            Instantiate(upgradeMenuPrefab);
        }
    }

    /*private void OnDestroy()
    {
        // Optionally destroy instances when leaving the scene
        GameSession.DestroyInstance();
        UpgradeMenu.DestroyInstance();
    }*/ 
}
