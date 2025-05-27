using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public int population = 50;

    public delegate void PopulationChanged(int newPopulation);
    public event PopulationChanged OnPopulationChanged;

    public void ChangePopulation(int amount)
    {
        population += amount;
        OnPopulationChanged?.Invoke(population);
    }
}
