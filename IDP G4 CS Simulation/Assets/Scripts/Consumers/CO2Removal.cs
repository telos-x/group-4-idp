using UnityEngine;

public class CO2Removal : MonoBehaviour, IPowerNode
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float powerPerCrew = 30f; // kW
    public float co2PerCrewDaily = 1f; // Approximate 1kg of CO2 produced per person, per day
    private float totalCO2Removed; // total in kg

    // IPowerNode implementation
    public float CurrentValue { get; set; }
    public int Priority => 5;
    public string NodeName => "CO2 Removal System";
    public bool isEnabled { get; set; } = true;

    // Update is called once per frame
    void Update()
    {
        UpdateNode(Time.deltaTime);
    }
    public void UpdateNode(float dt)
    {
        int crewCount = ColonyManager.Instance.ColonistCount; 
        // if (!isEnabled)
        // {
        //     CurrentValue = 0;
        //     return;
        // }
        float co2Rate = crewCount * co2PerCrewDaily / 86400f; // kg per second per person

        CurrentValue = -(crewCount * powerPerCrew);
        float removedThisFrame = co2Rate * dt;
        totalCO2Removed += removedThisFrame;
    }

    public float GetTotalCO2Removed()
    {
        return totalCO2Removed;
    }
}
