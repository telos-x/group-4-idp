using UnityEngine;

public class ResearchManagement : MonoBehaviour, IPowerNode
{
    public float CurrentValue { get; set; }
    public int Priority => 10;
    public string NodeName => "Research";
    public bool isEnabled { get; set; } = true;

    public float researchPower = 5f; // kW: allow user input to change
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
        CurrentValue = -researchPower;
    }
}
