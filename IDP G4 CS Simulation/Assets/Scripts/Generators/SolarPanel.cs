using UnityEngine;

public class SolarPanel : MonoBehaviour, IPowerNode {
    public InstantSolarPower powerEstimator;
    public float CurrentValue => powerEstimator.InstantaneousPower;
    public int Priority => 3;
    public string NodeName => "Solar Array";
    void Update()
    {}
    public void UpdateNode(float deltaTime)
    {
    }

}