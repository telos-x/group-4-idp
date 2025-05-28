using UnityEngine;

public class Microreactor : MonoBehaviour, IPowerNode {
    public InstantNuclearPower nuclearEstimator;
    public float CurrentValue { get; set; }
    public int Priority => 1;
    public string NodeName => "Microreactor";
    public bool isEnabled{ get; set; } = true;
    void Update()
    {
        UpdateNode(Time.deltaTime);
    }
    public void UpdateNode(float dt)
    {
        CurrentValue = isEnabled ? nuclearEstimator.InstantNP : 0f;
    }
}

