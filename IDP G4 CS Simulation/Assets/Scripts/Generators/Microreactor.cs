using UnityEngine;

public class Microreactor : MonoBehaviour, IPowerNode {
    public float minOutput, maxOutput, rampRate;
    private float currentOutput;
    public float CurrentValue => currentOutput;
    public int Priority => 1;
    public string NodeName => "Microreactor";
    public void UpdateNode(float dt) {
        float target = GridManager.Instance.RequiredCriticalLoad();
        currentOutput = Mathf.MoveTowards(currentOutput, 
                                          Mathf.Clamp(target, minOutput, maxOutput),
                                          rampRate * dt);
    }
}

