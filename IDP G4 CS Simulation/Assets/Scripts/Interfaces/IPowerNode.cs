using UnityEngine;

public interface IPowerNode {
    float CurrentValue { get; }    // +ve for production, –ve for consumption
    string NodeName { get; }
    int   Priority  { get; }       // Lower = more critical (1=life support)
    void  UpdateNode(float deltaTime);
}

