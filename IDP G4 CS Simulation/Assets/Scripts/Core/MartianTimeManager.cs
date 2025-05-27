using UnityEngine;
using System;

public class MartianTimeManager : MonoBehaviour
{
    public static MartianTimeManager Instance { get; private set; }

    // ——— Constants ———
    public const double MartianSolSeconds = 88775.244;   // seconds per sol
    public const double MartianYearSols    = 668.6;      // sols per martian year

    // Month durations (in sols) from table:
    //   Month 1 → 61.2 sols, 2 → 65.4, 3 → 66.7, …, 12 → 55.7
    private static readonly double[] MonthDurations = {
        61.2, 65.4, 66.7, 64.5, 59.7, 54.4,
        49.7, 46.9, 46.1, 47.4, 50.9, 55.7
    };

    [Header("Simulation Settings")]
    [Tooltip("Sol number at which simulation starts (e.g. landing = Sol 0)")]
    public int startSol = 0;
    [Tooltip("Martian year at simulation start (e.g. landing = Year 0)")]
    public int startYear = 0;
    [Tooltip("How many simulated Martian seconds pass per real second")]
    public float timeScale = 60f;

    // internal clock (Martian seconds since start)
    private double _elapsedMartianSeconds = 0.0;

    public float GetCurrentHour()
    {
        return (float)_elapsedMartianSeconds / 3600f;
    }
    // events
    public event Action<double> OnTimeTick;  // raw elapsed seconds
    public event Action         OnDateTick;  // sol/month/year/season changed

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        _elapsedMartianSeconds += Time.deltaTime * timeScale;
        OnTimeTick?.Invoke(_elapsedMartianSeconds);
        OnDateTick?.Invoke();
    }

    // ——— Public read-only properties ———

    // whole sols since simulation start
    public int SolsSinceStart => (int)(_elapsedMartianSeconds / MartianSolSeconds);

    // absolute sol index
    public int CurrentSol => startSol + SolsSinceStart;

    // time‐of‐sol (hh:mm:ss)
    public TimeSpan TimeOfSol 
        => TimeSpan.FromSeconds(_elapsedMartianSeconds % MartianSolSeconds);

    // martian year count
    public int CurrentYear 
        => startYear + (int)(SolsSinceStart / MartianYearSols);

    // sol‐index within current martian year [0, 668.6)
    private double SolInYear => (SolsSinceStart % MartianYearSols);

    // month 1…12 according to the variable‐length sol‐intervals
    public int CurrentMonth
    {
        get
        {
            double acc = 0;
            double sol = SolInYear;
            for (int i = 0; i < MonthDurations.Length; i++)
            {
                acc += MonthDurations[i];
                if (sol < acc) return i + 1;
            }
            return 12;  // fallback
        }
    }

    // season by month:
    //   months 1–3  → Northern Spring
    //   months 4–6  → Northern Summer
    //   months 7–9  → Northern Autumn
    //   months 10–12 → Northern Winter
    public string CurrentSeason
    {
        get
        {
            int m = CurrentMonth;
            if (m <= 3)  return "Northern Spring";
            if (m <= 6)  return "Northern Summer";
            if (m <= 9)  return "Northern Autumn";
            return           "Northern Winter";
        }
    }

    // jump the clock
    public void SetMartianTime(int sol, TimeSpan timeIntoSol, int year = 0)
    {
        startSol = sol;
        startYear = year;
        _elapsedMartianSeconds = sol * MartianSolSeconds + timeIntoSol.TotalSeconds;
        OnTimeTick?.Invoke(_elapsedMartianSeconds);
        OnDateTick?.Invoke();
    }
}
