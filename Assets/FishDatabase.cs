using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FishDatabase", menuName = "Fishing/Fish Database")]
public class FishDatabase : ScriptableObject
{
    public List<FishData> allFish = new List<FishData>();

    public FishData GetRandomFish()
    {
        if (allFish == null || allFish.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (var fish in allFish)
        {
            totalWeight += fish.spawnChanceWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var fish in allFish)
        {
            currentWeight += fish.spawnChanceWeight;
            if (randomValue <= currentWeight)
            {
                return fish;
            }
        }

        return allFish[0]; 
    }
}