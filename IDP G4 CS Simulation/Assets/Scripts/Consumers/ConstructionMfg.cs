using UnityEngine;

public class ConstructionMfg : MonoBehaviour, IPowerNode
{
    public float CurrentValue { get; set; }
    public int Priority => 9;
    public string NodeName => "ConstructionMfg";
    public bool isEnabled { get; set; } = true;
    
    public float constructionPower = 15.759f; // kW -> Projected construction time: 1 year, 5 days/week, 12 hours a day
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
        CurrentValue = -constructionPower;
    }
}
