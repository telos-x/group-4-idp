using UnityEngine;

public interface IPowerNode
{
    float CurrentValue { get; set; }    // +ve for production, –ve for consumption
    string NodeName { get; }
    int Priority { get; }       // Lower = more critical (1=life support)
    bool isEnabled { get; set; }
    void UpdateNode(float deltaTime);
}

