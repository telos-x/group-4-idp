using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    List<IPowerNode> nodes = new List<IPowerNode>();
    public float storageSoC, storageCapacity;

    void Awake() => Instance = this;

    void Start()
    {
        nodes.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IPowerNode>());
    }

    void Update()
    {
        float dt = Time.deltaTime;
        nodes.ForEach(n => n.UpdateNode(dt));

        // sum producers and consumers
        var producers = nodes.Where(n => n.CurrentValue > 0).ToList();
        var consumers = nodes.Where(n => n.CurrentValue < 0)
                             .OrderBy(n => n.Priority).ToList();


        float supply = producers.Sum(n => n.CurrentValue);
        float demand = -consumers.Sum(n => n.CurrentValue);
        float net = supply - demand;

        // dispatch into storage if excess or draw from storage if deficit
        if (net > 0)
        {
            storageSoC = Mathf.Min(storageCapacity, storageSoC + net * dt);
        }
        else
        {
            float needed = -net * dt;
            float drawn = Mathf.Min(needed, storageSoC);
            storageSoC -= drawn;
            net += drawn / dt;
        }

        // shed non‐critical loads if still in deficit
        if (net < 0)
        {
            foreach (var c in consumers.Where(c => c.Priority > 1))
            {
                // shed until net >= 0
                float shedAmount = Mathf.Min(-c.CurrentValue, -net);
                c.ShedLoad(shedAmount);
                net += shedAmount;
                if (net >= 0) break;
            }
        }

        // Record for graph
        TimeSeriesRecorder.Record(supply, demand, storageSoC);
    }

    public float RequiredCriticalLoad()
    {
        return nodes.Where(n => n.CurrentValue < 0 && n.Priority == 1)
                    .Sum(n => -n.CurrentValue);
    }
}
