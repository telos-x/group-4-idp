using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(MartianTimeManager))]
public class DustStormSimulator : MonoBehaviour
{
    [Header("Storm Probability by Month (0–1)")]
    [Tooltip("Index 0→Ls0–30, 1→30–60, …, 11→330–360")]
    public float[] stormChanceByMonth = new float[12]
    {
        0.05f, 0.05f, 0.10f, 0.15f,
        0.20f, 0.20f, 0.25f, 0.30f,
        0.25f, 0.15f, 0.10f, 0.05f
    };

    [Header("Storm Duration (sols)")]
    public int minDuration = 5;
    public int maxDuration = 30;

    [Header("Optical Depth (τ) Range")]
    [Tooltip("Higher τ → darker/dustier")]
    public float minTau = 0.5f;
    public float maxTau = 2.5f;

    // Current storm state
    public bool StormActive    { get; private set; }
    public int  SolsRemaining  { get; private set; }
    public float CurrentTau    { get; private set; }

    private MartianTimeManager timeMgr;

    void Awake()
    {
        timeMgr = MartianTimeManager.Instance;
    }

    void OnEnable()
    {
        timeMgr.OnTimeChanged += OnTimeChanged;
    }

    void OnDisable()
    {
        timeMgr.OnTimeChanged -= OnTimeChanged;
    }

    private void OnTimeChanged(float lmst)
    {
        // only trigger logic once per sol, at LMST near 0
        if (lmst > 0.01f) return;

        if (StormActive)
        {
            // storm is ongoing
            SolsRemaining--;
            if (SolsRemaining <= 0)
            {
                StormActive = false;
                Debug.Log("Dust storm ended.");
            }
        }
        else
        {
            // maybe start a new storm?
            int month = timeMgr.CurrentMonth;
            if (Random.value < stormChanceByMonth[month])
            {
                StormActive   = true;
                SolsRemaining = Random.Range(minDuration, maxDuration+1);
                CurrentTau    = Random.Range(minTau, maxTau);
                Debug.Log($"Dust storm started: {SolsRemaining} sols, τ={CurrentTau:F2}");
            }
        }
    }

    /// <summary>
    /// Returns an attenuation multiplier [0–1] for direct sunlight:
    /// I_actual = I_clear × AttenuationFactor.
    /// Approximate with Beer-Lambert: exp(-τ / cos(zenithAngle))
    /// Here we simplify: exp(-CurrentTau).
    /// </summary>
    public float GetAttenuation()
    {
        if (!StormActive) return 1f;
        return Mathf.Exp(-CurrentTau);
    }
}
