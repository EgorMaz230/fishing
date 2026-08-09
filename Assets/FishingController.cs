using UnityEngine;
using UnityEngine.UI;
using TMPro; // Обязательно для работы с TextMeshPro
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
    public float playerPullSpeed = 35f; 
    public float fishEscapeSpeed = 25f; 

    [Header("UI Элементы")]
    public TextMeshProUGUI infoText; // Ссылка на текст подсказок
    public Slider tensionSlider;     // Ссылка на шкалу натяжения

    private Coroutine waitBiteCoroutine;
    private Coroutine biteWindowCoroutine;

    void Start()
    {
        // При старте прячем шкалу натяжения, так как мы еще не боремся с рыбой
        if (tensionSlider != null) tensionSlider.gameObject.SetActive(false);
        UpdateText("Нажми ЛКМ, чтобы забросить удочку.");
    }

    void Update()
    {
        if (isFighting)
        {
            if (Input.GetMouseButton(0))
            {
                tension += playerPullSpeed * Time.deltaTime;
            }
            else
            {
                tension -= fishEscapeSpeed * Time.deltaTime;
            }

            tension = Mathf.Clamp(tension, 0f, maxTension);
            
            // Обновляем шкалу на экране (переводим в диапазоне от 0 до 1 для слайдера Unity)
            if (tensionSlider != null)
            {
                tensionSlider.value = tension / maxTension;
            }

            UpdateText("БОРЬБА! Натяжение: " + Mathf.Round(tension) + "%");

            if (tension >= maxTension)
            {
                FailFishing("Леска порвалась! Ты слишком сильно тянул катушку.");
            }
            else if (tension <= 0f)
            {
                FailFishing("Леска провисла! Рыба сорвалась из-за слабины.");
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
        
        UpdateText("Заброс... Ждем поклевку...");
        waitBiteCoroutine = StartCoroutine(WaitBiteRoutine());
    }

    IEnumerator WaitBiteRoutine()
    {
        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);

        if (isFishing && !hasBite && !isFighting)
        {
            hasBite = true;
            UpdateText("КЛЮЕТ! Быстро жми ЛКМ для подсечки!");
            biteWindowCoroutine = StartCoroutine(BiteWindowRoutine());
        }
    }

    IEnumerator BiteWindowRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (hasBite && !isFighting)
        {
            FailFishing("Эх, рыба сорвалась! Ты слишком долго ждал.");
        }
    }

    public void TryHook()
    {
        if (hasBite && !isFighting)
        {
            if (biteWindowCoroutine != null) StopCoroutine(biteWindowCoroutine);
            
            hasBite = false;
            isFighting = true; 
            tension = 50f; 

            // Включаем отображение шкалы натяжения на экране
            if (tensionSlider != null) tensionSlider.gameObject.SetActive(true);

            UpdateText("НАЧАЛАСЬ БОРЬБА! Балансируй натяжение!");
            StartCoroutine(FightTimerRoutine());
        }
    }

    IEnumerator FightTimerRoutine()
    {
        yield return new WaitForSeconds(6f);

        if (isFighting)
        {
            SuccessFishing();
        }
    }

    void CancelFishing()
    {
        if (waitBiteCoroutine != null) StopCoroutine(waitBiteCoroutine);

        isFishing = false;
        hasBite = false;
        UpdateText("Ты смотал леску. Нажми ЛКМ для нового заброса.");
    }

    void SuccessFishing()
    {
        ResetUIState();
        UpdateText("ПОБЕДА! Ты поймал рыбу! Нажми ЛКМ для нового заброса.");
    }

    void FailFishing(string reason)
    {
        ResetUIState();
        UpdateText(reason + "\nНажми ЛКМ, чтобы попробовать снова.");
    }

    void ResetUIState()
    {
        isFighting = false;
        isFishing = false;
        hasBite = false;
        if (tensionSlider != null) tensionSlider.gameObject.SetActive(false);
    }

    void UpdateText(string message)
{
    if (infoText != null && infoText.font != null)
    {
        infoText.text = message;
    }
}
}