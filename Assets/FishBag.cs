using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class CaughtFish
{
    public FishData data;
    public float actualWeight;

    public int GetPrice()
    {
        if (data == null) return 0;
        return Mathf.RoundToInt(actualWeight * data.pricePerKg);
    }
}

public class FishBag : MonoBehaviour
{
    [Header("Настройки садка")]
    public float maxCapacityKg = 10f; 
    public float currentWeight = 0f;  

    [Header("Экономика")]
    public int coins = 0; 

    [Header("UI Элементы")]
    public TextMeshProUGUI bagCapacityText; 
    public TextMeshProUGUI coinsText;       

    [Header("Содержимое")]
    public List<CaughtFish> caughtFishList = new List<CaughtFish>();

    void Start()
    {
        UpdateUI(); 
    }

    public bool TryAddFish(FishData fish, float weight)
    {
        weight = Mathf.Round(weight * 100f) / 100f;

        if (currentWeight + weight <= maxCapacityKg)
        {
            CaughtFish newFish = new CaughtFish();
            newFish.data = fish;
            newFish.actualWeight = weight;

            caughtFishList.Add(newFish);
            
            currentWeight += weight;
            currentWeight = Mathf.Round(currentWeight * 100f) / 100f;

            UpdateUI(); 

            return true;
        }
        else
        {
            return false;
        }
    }

    // Метод продажи всей рыбы (теперь его будет вызывать зона торговли!)
    public void SellAllFish()
    {
        if (caughtFishList.Count == 0)
        {
            return; // Если садок пуст — ничего не делаем
        }

        int totalEarned = 0;

        foreach (var fish in caughtFishList)
        {
            totalEarned += fish.GetPrice();
        }

        coins += totalEarned; 
        caughtFishList.Clear(); 
        currentWeight = 0f;    

        Debug.Log($"<color=yellow>[Рынок]</color> Вся рыба продана за {totalEarned} монет! Баланс: {coins}");

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (bagCapacityText != null)
        {
            bagCapacityText.text = $"Садок: {currentWeight} / {maxCapacityKg} кг";
        }

        if (coinsText != null)
        {
            coinsText.text = $"Монеты: {coins}";
        }
    }
}