using UnityEngine;

public class Agriculture : MonoBehaviour, IPowerNode
{
    public float CurrentValue { get; set; }
    public int Priority => 8;
    public string NodeName => "Agriculture";
    public bool isEnabled { get; set; } = true;

    public float plantAquaponicsPower = 1.211f; // kW
    public float fishAquaponicsPower = 1.205f; // kW
    public float powerPer1kgCrops = 56f; // kWh -> need time to grow
    public float powerPer1kgFish = 159f; // kWh -> need time to grow
    public float powerPerPerson = 3.19f; // kW: estimate based on reasonable distribution of crops and fish
    public float algaeFarmPower = 0.36f; // kW

    void Update()
    {
        UpdateNode(Time.deltaTime);
    }
    public void UpdateNode(float dt)
    {
        int count = ColonyManager.Instance.ColonistCount;
        // if (!isEnabled)
        // {
        //     CurrentValue = 0f;
        //     return;
        // }
        CurrentValue = -((powerPerPerson * count) + plantAquaponicsPower + fishAquaponicsPower + algaeFarmPower);
    }
}
