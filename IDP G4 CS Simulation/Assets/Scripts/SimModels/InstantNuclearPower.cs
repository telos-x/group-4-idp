using UnityEngine;

public class InstantNuclearPower : MonoBehaviour
{
    public float minOutput = 0f;
    public float maxOutput = 20000f; // in kW, so 20 MW
    public float rampRate = 0.05f; // ~5%/min ramp rate to reach full capacity in about 20 minutes (standard estimate)
    public float InstantNP;

    void Update()
    {
        float target = GridManager.Instance.RequiredCriticalLoad();
        bool storageFull = GridManager.Instance.IsStorageFull();
        InstantNP = Mathf.MoveTowards(InstantNP, Mathf.Clamp(target, minOutput, maxOutput), rampRate * 10000 * Time.deltaTime);
        if (InstantNP >= target && storageFull)
        {
            rampRate = 0f;
            InstantNP = target;
        }
        else
        {
            rampRate = 0.05f;
        }
    }
}
