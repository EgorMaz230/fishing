using UnityEngine;

public class SellZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Ищем скрипт FishBag у объекта, у его родителей или в его детях
        FishBag bag = other.GetComponentInParent<FishBag>();
        if (bag == null) bag = other.GetComponentInChildren<FishBag>();

        if (bag != null)
        {
            bag.SellAllFish();
        }
        else
        {
            Debug.Log($"[SellZone] Кто-то наступил ({other.name}), но скрипт FishBag на нем не найден!");
        }
    }
}