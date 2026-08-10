using UnityEngine;

[System.Serializable]
public class FishData
{
    public string fishName = "Окунь";
    
    [Header("Диапазон веса (в кг)")]
    public float minWeight = 0.5f; 
    public float maxWeight = 1.5f; 

    [Header("Экономика")]
    public float pricePerKg = 50f; // Цена за 1 кг (чем реже рыба, тем выше число!)
    
    public float fishPullStrength = 20f;
    public float fightDuration = 6f;
    public string location = "Lake1";
    
    [Header("Новые параметры")]
    public float spawnChanceWeight = 100f; 
    public float erraticMultiplier = 0.5f; 
}