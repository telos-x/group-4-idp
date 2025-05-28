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
    public double recordInterval = 1.0;
    public MartianTimeManager timeKeeper;
    public double martianSec = 0.0;

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

    private List<PowerRecord> _records = new List<PowerRecord>();
    private double _nextRecordTime;

    void Awake()
    {
        if (timeKeeper == null)
        {
            timeKeeper = GetComponent<MartianTimeManager>();
        }
    }
    
    /*
    void OnEnable() => MartianTimeManager.Instance.OnTimeTick += OnTimeTick;
    void OnDisable() => MartianTimeManager.Instance.OnTimeTick -= OnTimeTick;
    */

    private void OnTimeTick()
    {
        martianSec = timeKeeper.GetMartianSeconds();
        if (martianSec < _nextRecordTime) return;
        Record(martianSec, gridManager.supply, gridManager.demand, gridManager.net, gridManager.storageSoC, solarPanel.CurrentValue,
            nuclearReactor.CurrentValue);
        _nextRecordTime += recordInterval;
    }

    void Record(double secs, float generated, float consumed, float netPower, float storageSoC, float solarGen, float nuclearGen)
    {
        _records.Add(new PowerRecord
        {
            time      = secs,
            produced  = generated,
            used      = consumed,
            net       = netPower,
            storage   = storageSoC,
            solar     = solarGen,
            nuclear   = nuclearGen
        });
    }

    /// <summary>Read-only access to the time-series data.</summary>
    public IReadOnlyList<PowerRecord> Records => _records;
}
