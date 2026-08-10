using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FishingController : MonoBehaviour
{
    [Header("Настройки рыбалки")]
    public bool isFishing = false;
    public bool hasBite = false; 
    public bool isFighting = false; 

    [Header("Параметры натяжения лески")]
    public float tension = 0f; 
    public float maxTension = 100f;
    public float playerPullSpeed = 40f; 

    [Header("Прогресс поимки")]
    public float catchProgress = 0f; 
    private float currentFightDuration = 6f;
    private float currentFishPull = 20f;
    private float currentErraticMultiplier = 0f;
    public float playerPullStrength = 25f; 

    [Header("UI Элементы")]
    public TextMeshProUGUI infoText; 
    public Slider tensionSlider;     
    public Slider progressSlider;    
    public bool isEnglish = false; 

    [Header("База данных всех рыб")]
    public FishDatabase fishDatabase; 
    
    [Header("Инвентарь")]
    public FishBag playerBag; // ССЫЛКА НА САДОК

    [Header("Текущая пойманная рыба")]
    public FishData currentHookedFish;
    private float currentFishWeight; // Сюда сохраняем рандомный вес клюнувшей рыбы

    private Coroutine waitBiteCoroutine;
    private Coroutine biteWindowCoroutine;

    void Start()
    {
        if (tensionSlider != null) tensionSlider.gameObject.SetActive(false);
        if (progressSlider != null) progressSlider.gameObject.SetActive(false);
        
        UpdateText(isEnglish ? "Press LMB to cast." : "Нажми ЛКМ, чтобы забросить удочку.");
    }

    void Update()
    {
        if (isFighting)
        {
            float basePull = currentFishPull * Mathf.Sin(Time.time * 4f) + currentFishPull; 
            float jerkNoise = (Mathf.PerlinNoise(Time.time * 3f, 0f) - 0.5f) * 2f;
            float erraticJerk = jerkNoise * (currentFishPull * currentErraticMultiplier);

            float fishAction = basePull + erraticJerk;
            
            if (Input.GetMouseButton(0))
            {
                tension += playerPullSpeed * Time.deltaTime;
                catchProgress += (120f / currentFightDuration) * Time.deltaTime; 
            }
            else
            {
                tension -= (playerPullStrength * 0.8f) * Time.deltaTime;
            }

            tension += fishAction * 0.12f * Time.deltaTime;

            tension = Mathf.Clamp(tension, 0f, maxTension);
            catchProgress = Mathf.Clamp(catchProgress, 0f, 100f);
            
            if (tensionSlider != null) tensionSlider.value = tension / maxTension;
            if (progressSlider != null) progressSlider.value = catchProgress / 100f;

            string fishNameStr = currentHookedFish != null ? currentHookedFish.fishName : "Fish";
            
            // Показываем сгенерированный вес
            UpdateText((isEnglish ? "FIGHT with " : "БОРЬБА с ") + fishNameStr + " (" + currentFishWeight + "kg)! " + Mathf.Round(catchProgress) + "%");

            if (tension >= maxTension)
            {
                FailFishing(isEnglish ? "Line broke! Too much tension." : "Леска порвалась! Рыба оказалась слишком сильной.");
            }
            else if (tension <= 0f)
            {
                FailFishing(isEnglish ? "Line went slack! Fish escaped." : "Леска провисла! Рыба сорвалась.");
            }
            else if (catchProgress >= 100f)
            {
                SuccessFishing();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isFishing)
            {
                StartFishing();
            }
            else if (hasBite)
            {
                TryHook();
            }
            else
            {
                CancelFishing();
            }
        }
    }

    void StartFishing()
    {
        isFishing = true;
        hasBite = false;
        isFighting = false;
        tension = 0f;
        catchProgress = 0f;
        
        UpdateText(isEnglish ? "Casting... Waiting for a bite..." : "Заброс... Ждем поклевку...");
        waitBiteCoroutine = StartCoroutine(WaitBiteRoutine());
    }

    IEnumerator WaitBiteRoutine()
    {
        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);

        if (isFishing && !hasBite && !isFighting)
        {
            hasBite = true;
            UpdateText(isEnglish ? "BITE! Press LMB quickly!" : "КЛЮЕТ! Быстро жми ЛКМ для подсечки!");
            biteWindowCoroutine = StartCoroutine(BiteWindowRoutine());
        }
    }

    IEnumerator BiteWindowRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (hasBite && !isFighting)
        {
            FailFishing(isEnglish ? "Fish got away! You waited too long." : "Эх, рыба уплыла! Ты слишком долго ждал.");
        }
    }

    public void TryHook()
    {
        if (hasBite && !isFighting)
        {
            if (biteWindowCoroutine != null) StopCoroutine(biteWindowCoroutine);
            
            hasBite = false;
            isFighting = true; 
            tension = 40f; 
            catchProgress = 0f;

            if (fishDatabase != null)
            {
                currentHookedFish = fishDatabase.GetRandomFish();
            }

            if (currentHookedFish != null)
            {
                currentFishPull = currentHookedFish.fishPullStrength;
                currentFightDuration = currentHookedFish.fightDuration;
                currentErraticMultiplier = currentHookedFish.erraticMultiplier;
                
                // ГЕНЕРИРУЕМ ВЕС РЫБЫ ИМЕННО ЗДЕСЬ (от min до max)
                currentFishWeight = Random.Range(currentHookedFish.minWeight, currentHookedFish.maxWeight);
                currentFishWeight = Mathf.Round(currentFishWeight * 100f) / 100f; // Округляем
            }
            else
            {
                currentFishPull = 20f;
                currentFightDuration = 6f;
                currentErraticMultiplier = 0.5f;
                currentFishWeight = 1.0f;
            }

            if (tensionSlider != null) tensionSlider.gameObject.SetActive(true);
            if (progressSlider != null) progressSlider.gameObject.SetActive(true);

            string fishNameStr = currentHookedFish != null ? currentHookedFish.fishName : "Fish";
            UpdateText(isEnglish ? "HOOKED a " + fishNameStr + "!" : "ПОДСЕЧКА! Клюнул: " + fishNameStr + "!");
        }
    }

    void CancelFishing()
    {
        if (waitBiteCoroutine != null) StopCoroutine(waitBiteCoroutine);

        isFishing = false;
        hasBite = false;
        UpdateText(isEnglish ? "Line reeled in. Click to cast again." : "Ты смотал леску. Нажми ЛКМ для нового заброса.");
    }

    void SuccessFishing()
    {
        ResetUIState();
        string fishNameStr = currentHookedFish != null ? currentHookedFish.fishName : "Fish";

        // Пытаемся положить рыбу в садок
        if (currentHookedFish != null && playerBag != null)
        {
            bool isSaved = playerBag.TryAddFish(currentHookedFish, currentFishWeight);
            
            if (isSaved)
            {
                UpdateText(isEnglish ? $"SUCCESS! Caught {fishNameStr} ({currentFishWeight}kg)!" : $"ПОБЕДА! {fishNameStr} ({currentFishWeight} кг) в садке!");
            }
            else
            {
                UpdateText(isEnglish ? $"Bag is full! {fishNameStr} released." : $"САДОК ПОЛОН! {fishNameStr} ({currentFishWeight} кг) не влезла.");
            }
        }
        else
        {
            UpdateText(isEnglish ? "SUCCESS! Caught " + fishNameStr + "!" : "ПОБЕДА! Ты поймал: " + fishNameStr + "!");
        }
    }

    void FailFishing(string reason)
    {
        ResetUIState();
        UpdateText(reason + (isEnglish ? "\nClick to try again." : "\nНажми ЛКМ, чтобы попробовать снова."));
    }

    void ResetUIState()
    {
        isFighting = false;
        isFishing = false;
        hasBite = false;
        if (tensionSlider != null) tensionSlider.gameObject.SetActive(false);
        if (progressSlider != null) progressSlider.gameObject.SetActive(false);
    }

    void UpdateText(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
        }
    }
}