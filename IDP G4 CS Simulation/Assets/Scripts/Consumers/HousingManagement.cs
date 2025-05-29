using UnityEngine;

public class HousingManagement : MonoBehaviour, IPowerNode
{
    //IPowerNode
    public float CurrentValue { get; set; }
    public int Priority => 6;
    public string NodeName => "Housing Management";
    public bool isEnabled { get; set; } = true;

    public float HeatingValue;
    public float heatPowerPerUnit = 5f; // kW power needed per housing unit (5 people) for heating
    public float latitudeDeg = 46.7f;

    public float LightingValue;
    public float lightPowerPerUnit = 0.3f; // kW power needed per housing unit for lighting 

    public float PressurizationValue;
    public float pressurizationPowerPerUnit = 2f; //kW power needed per housing unit for pressurization

    public MartianTimeManager timeMgr;
    public ColonyManager colonyMgr;
    void Awake()
    {
        timeMgr   = MartianTimeManager.Instance;
        colonyMgr = ColonyManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNode(Time.deltaTime);
    }
    public void UpdateNode(float dt)
    {
        // if (!isEnabled)
        // {
        //     CurrentValue = 0f;
        //     return;
        // }
        // 1) get local mean solar time and month index
        float lmst = timeMgr.GetCurrentHour();              // 0–24.6597
        int mIdx = timeMgr.CurrentMonth - 1;              // 0–11
        // 2) approximate solar declination mid-month
        float midLs = mIdx * 30f + 15f;
        float declDeg = 25.19f * Mathf.Sin(midLs * Mathf.Deg2Rad);
        // 3) compute exposure (0→1) just like InstantSolarPower does
        float exposure = InstantSolarPower.ComputeExposure(lmst, latitudeDeg, declDeg);
        // 4) determine how many housing units we need 
        int colonists = colonyMgr.ColonistCount;
        int housingUnits = Mathf.CeilToInt(colonists / 5f);

        // HEATING is max at night (exposure=0), zero at peak sun (exposure=1)
        HeatingValue = housingUnits * heatPowerPerUnit * (1f - exposure);
        // LIGHTING is also max at night when darkest
        LightingValue = housingUnits * lightPowerPerUnit * (1f - exposure);
        // PRESSURIZATION only depends on number of housing units and has a constant demand
        PressurizationValue = housingUnits * pressurizationPowerPerUnit;

        // Sum all for CurrentValue
        CurrentValue = -(HeatingValue + LightingValue + PressurizationValue);
    }
}
