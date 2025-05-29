using UnityEngine;

public class WaterManagement : MonoBehaviour, IPowerNode
{
    public float CurrentValue { get; set; }
    public int Priority => 7;
    public string NodeName => "Water Generator";
    public bool isEnabled { get; set; } = true;

    public float waterNeededDaily = 25000f; // L/day 
    // ~150 L/day per person -> includes water for showers, laundry, etc.
    public float powerToWaterEfficiency = 0.001906f; // kW/L
    void Update()
    {
        UpdateNode(Time.deltaTime);
    }
    public void UpdateNode(float dt)
    {
        // if (!isEnabled)
        // {
        //     CurrentValue = 0f;
        //     return;
        // }
        CurrentValue = -(waterNeededDaily * powerToWaterEfficiency);
    }
}
