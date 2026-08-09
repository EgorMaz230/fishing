using UnityEngine;

[System.Serializable]
public class FishData
{
    public string fishName = "Окунь";
    public float weight = 1.0f;
    public float fishPullStrength = 20f;
    public float fightDuration = 6f;
    public string location = "Lake1";
    
    [Header("Новые параметры")]
    public float spawnChanceWeight = 100f; 
    public float erraticMultiplier = 0.5f; 
}