using UnityEngine;

public class ColonyManager : MonoBehaviour
{
    public static ColonyManager Instance { get; private set; }

    [Tooltip("Total number of colonists in the colony.")]
    public int ColonistCount = 50;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
