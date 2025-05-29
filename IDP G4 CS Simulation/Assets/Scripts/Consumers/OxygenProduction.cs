using UnityEngine;

public class OxygenProduction : MonoBehaviour, IPowerNode {
    public float oxygenPerColonist = 0.0035f; // oxygen production rate in liters per second per person
    public float totalOxygenProduced = 0f; // total oxygen produced so far in liters
    public float powerPerColonist = 0.35f; // kW 
    public float CurrentValue { get; set; }
    public string NodeName => "Oxygen Generator";
    public int Priority => 4;
    public bool isEnabled { get; set; } = true;

    void Update()
    {
        // Debug.Log("OxygenProduction CurrentValue: " + CurrentValue);
        UpdateNode(Time.deltaTime);
    }

    public void UpdateNode(float deltaTime)
    {
        // if (isEnabled == false)
        // {
        //     CurrentValue = 0f;
        //     return;
        // }
        int count = ColonyManager.Instance.ColonistCount;
        CurrentValue = -(count * powerPerColonist);
        float produced = count * oxygenPerColonist * deltaTime;
        totalOxygenProduced += produced;
    }

    public float GetTotalOxygenProduced()
    {
        return totalOxygenProduced;
    }
}
