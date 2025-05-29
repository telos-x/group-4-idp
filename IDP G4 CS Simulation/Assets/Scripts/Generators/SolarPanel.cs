using UnityEngine;

public class SolarPanel : MonoBehaviour, IPowerNode {
    public InstantSolarPower powerEstimator;
    public float CurrentValue { get; set; }
    public int Priority => 2;
    public string NodeName => "Solar Array";
    public bool isEnabled { get; set; } = true;
    void Update()
    {
        UpdateNode(Time.deltaTime);
    }
    public void UpdateNode(float deltaTime)
    {
        CurrentValue = powerEstimator.InstantaneousPower;
        Debug.Log("SolarPanel UpdateNode CurrentValue: " + CurrentValue);
    }

}