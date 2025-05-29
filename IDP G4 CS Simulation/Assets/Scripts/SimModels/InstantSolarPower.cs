using UnityEngine;

[RequireComponent(typeof(MartianTimeManager))]
public class InstantSolarPower : MonoBehaviour
{
    public DustStormSimulator dustSim;
    [Header("PV Array")]
    [Tooltip("Planar area of your PV array in m²")]
    public float arrayArea = 100f;

    [Header("Monthly Peak Irradiance (W/m²)")]
    [Tooltip("0→Ls0–30, 1→30–60, …, 11→330–360")]

    // peak irradiance each month in northern hemisphere of Mars
    public float[] peakIrradianceByMonth = new float[12]
    {
        405f, 430f, 375f, 550f, 520f, 480f,
        405f, 300f, 700f, 215f, 250f, 350f
    };

    public MartianTimeManager timeMgr;

    [Header("Output (read-only)")]
    [Tooltip("Instantaneous power output in Watts")]
    private float instantaneousPower;

    public float InstantaneousPower => instantaneousPower;

    void Awake()
    {
        timeMgr = MartianTimeManager.Instance;
    }

    void Update()
    {
        // 1) Current LMST (0–24.6597)
        float lmst = timeMgr.GetCurrentHour();

        // 2) Get month index 0–11
        int month = timeMgr.CurrentMonth - 1;

        // 3) Peak irradiance for this month
        float Ipeak = peakIrradianceByMonth[month];

        // 4) Compute exposure factor (0–1)
        //    Here we approximate declination as mid-month:
        float midLs = month * 30f + 15f;
        float declDeg = 25.19f * Mathf.Sin(midLs * Mathf.Deg2Rad);
        float exposure = ComputeExposure(lmst, 46.7f, declDeg);

        // 5) clear-sky power (W = W/m² × m²)
        float clearPower = exposure * Ipeak * arrayArea;

        // 6) instantaneous power with dust
        float atten = dustSim != null ? dustSim.GetAttenuation() : 1f;
        instantaneousPower = clearPower * atten;

        // 7) (Optional) Debug log once per simulated hour
        if (Mathf.Abs(lmst - Mathf.Round(lmst)) < 0.01f)
        {
            Debug.Log($"[{lmst:F1}h] Month {month + 1}, P_inst ≈ {instantaneousPower:F0} W");
        }
    }

    /// Returns relative exposure [0–1] given LMST (h), latitude (deg), and solar declination (deg).
    public static float ComputeExposure(float lmst, float latDeg, float declDeg)
    {
        const float solHours = 24.6597f;

        // convert to radians
        float phi = latDeg * Mathf.Deg2Rad;
        float delta = declDeg * Mathf.Deg2Rad;

        // hour angle H = (LMST – local noon) × (360°/solHours)
        float H = (lmst - solHours/2f) * (360f/solHours) * Mathf.Deg2Rad;

        // sin a = sin(phi) sin(delta) + cos(phi) cos(delta) cosH
        float sina = Mathf.Sin(phi)*Mathf.Sin(delta)
                   + Mathf.Cos(phi)*Mathf.Cos(delta)*Mathf.Cos(H);

        // exposure = max(0, sina)
        return Mathf.Clamp01(sina);
    }
}
