using UnityEngine;
using System;
using System.Collections.Generic;

public class TimeSeriesRecorder : MonoBehaviour
{
    [Tooltip("Reference to your SolarPanel component")]
    public SolarPanel solarPanel;

    [Tooltip("Reference to your MicroReactor (nuclear) component")]
    public Microreactor nuclearReactor;

    [Tooltip("Reference to your GridManager that provides total power stats")]
    public GridManager gridManager;

    [Tooltip("How often (in Martian seconds) to sample and record data")]
    public double recordInterval = 5.0;

    public MartianTimeManager timeKeeper;
    private double _nextRecordTime;
    private List<PowerRecord> _records = new List<PowerRecord>();

    [Serializable]
    public struct PowerRecord
    {
        public double time;
        public float produced;
        public float used;
        public float net;
        public float storage;
        public float solar;
        public float nuclear;
    }

    void Awake()
    {
        Debug.Log("TimeSeriesRecorder is Awake.");
        timeKeeper = MartianTimeManager.Instance;
        Debug.Assert(solarPanel != null, "SolarPanel reference is missing!");
        Debug.Assert(solarPanel.powerEstimator != null, "SolarPanel.powerEstimator is missing!");
        Debug.Assert(nuclearReactor != null, "NuclearReactor is missing!");
        Debug.Assert(gridManager != null, "gridManager is missing!");
        Debug.Assert(timeKeeper != null, "timeKeeper is missing!");
    }

    void Start()
    {
        // schedule the first record one interval in the future
        _nextRecordTime = timeKeeper.GetMartianSeconds() + recordInterval;
        _nextRecordTime = 0.0;
        Debug.Assert(solarPanel != null, "SolarPanel reference is missing!");
        Debug.Assert(solarPanel.powerEstimator != null, "SolarPanel.powerEstimator is missing!");

    }

    void LateUpdate()
    {
        double secs = timeKeeper.GetMartianSeconds();
        /*Debug.Log("TSRUpdate: supply is " + gridManager.supply);
        Debug.Log("TSRUpdate: solar is " + solarPanel.CurrentValue);
        Debug.Log("TSRUpdate: nuclear is " + nuclearReactor.CurrentValue);*/
        Record(
            secs,
            gridManager.supply,
            gridManager.demand,
            gridManager.net,
            gridManager.storageSoC,
            solarPanel.CurrentValue,
            nuclearReactor.CurrentValue
        );

        _nextRecordTime += recordInterval;
    }

    void Record(double secs, float generated, float consumed, float netPower,
                float storageSoC, float solarGen, float nuclearGen)
    {
        _records.Add(new PowerRecord
        {
            time     = secs,
            produced = generated,
            used     = consumed,
            net      = netPower,
            storage  = storageSoC,
            solar    = solarGen,
            nuclear  = nuclearGen
        });

        Debug.Log("time: " + secs);
        Debug.Log("generated: " + generated);
        Debug.Log("consumed: " + consumed);
        Debug.Log("net power: " + netPower);
        Debug.Log("storage: " + storageSoC);
        Debug.Log("solar: " + solarGen);
        Debug.Log("nuclear: " + nuclearGen);
    }

    // expose your data read‐only
    public IReadOnlyList<PowerRecord> Records => _records;
}