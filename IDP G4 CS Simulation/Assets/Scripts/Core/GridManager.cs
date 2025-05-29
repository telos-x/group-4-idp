using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    List<IPowerNode> nodes = new List<IPowerNode>();
    public float storageSoC = 0f;
    public float storageCapacity = 80000f; // in kWh, so 80 MWh, or 80 MW/time

    public float supply;
    public float demand;
    public float net;
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
        var consumers = nodes.Where(n => n.CurrentValue < 0).OrderBy(n => n.Priority).ToList();

        supply = producers.Sum(n => n.CurrentValue);
        Debug.Log("supply: " + supply);
        demand = -consumers.Sum(n => n.CurrentValue);
        Debug.Log("demand: " + demand);
        net = supply - demand;
        Debug.Log("net: " + net);

        // dispatch into storage if excess or draw from storage if deficit
        if (net > 0)
        {
            storageSoC = Mathf.Min(storageCapacity, storageSoC + net * dt);
            if (storageSoC >= storageCapacity)
            {
                ReturnLoad(nodes, net);
            }
        }
        else
        {
            // draw from storage
            float needed = -net * dt;
            float drawn = Mathf.Min(needed, storageSoC);
            storageSoC -= drawn;
            net += drawn / dt;
        }

        // shed non‐critical loads if still in deficit
        if (net < 0)
        {
            ShedLoad(nodes, net);
        }

        void ShedLoad(List<IPowerNode> nodes, float targetReduction)
        {
            foreach (var c in consumers.Where(c => c.Priority > 1))
            {
                // shed until net >= 0
                if (c.isEnabled)
                {
                    float shedAmount = Mathf.Min(-c.CurrentValue, -net);
                    net += shedAmount;
                    c.isEnabled = false;
                }
                if (net >= 0) break;
            }
        }

        void ReturnLoad(List<IPowerNode> nodes, float recoverableAmount)
        {
            foreach (var c in consumers.Where(c => c.Priority < 11))
            {
                if (c.isEnabled == false)
                {
                    float returnAmount = Mathf.Min(c.CurrentValue, net);
                    net -= returnAmount;
                    c.isEnabled = true;
                }
                if (net <= 0) break;
            }
        }

    }

    public float RequiredCriticalLoad()
    {
        return demand;
    }
    public bool IsStorageFull()
    {
        if (storageSoC == storageCapacity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
