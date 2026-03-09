using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IGFTimeoutToMenu : MonoBehaviour
{
    public CanvasGroup mainGroup;
    public PauseManager pauseManager;
    public LevelLoader levelLoader;
    public TMP_Text labelText;
    public string labelPrefix, labelSuffix;
    public string mainMenuScene = "MainMenu";

    public float transTime = 0.5f;
    public float timeBeforeCountdown = 10f;
    public float countdownDuration = 60f;

    private EventSystem eventSystem;
    private bool countingDown = false;
    private bool alreadyChanging = false;
    [SerializeField] [ReadOnly] private float idleTime = 0f;
    [SerializeField] [ReadOnly] private float countdownRemaining = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (alreadyChanging) return;

        if (countingDown) {
            idleTime = 0;

            if (InputHandler.Instance && InputHandler.Instance.AnyDown) {
                ExitCountdown();
            }

            countdownRemaining -= Time.unscaledDeltaTime;

            labelText.text = $"{labelPrefix}{Mathf.RoundToInt(countdownRemaining)}{labelSuffix}";

            if (countdownRemaining <= 0) {
                ExitCountdown();
                levelLoader.LoadLevel(mainMenuScene);
            }

        } else { // !countingDown

            // Build idle time.
            idleTime += Time.unscaledDeltaTime;

            // If not idle, reset it.
            if (InputHandler.Instance && InputHandler.Instance.AnyDown) {
                idleTime = 0;
            }

            if (idleTime > timeBeforeCountdown) {
                EnterCountdown();
            }
        }
    }
    

    public void EnterCountdown()
    {
        pauseManager.Pause(true);

        countdownRemaining = countdownDuration;

        alreadyChanging = true;
        StopCoroutine(EnterCountdownRoutine());
        StartCoroutine(EnterCountdownRoutine());

        IEnumerator EnterCountdownRoutine()
        {
            eventSystem.enabled = false;
            float elapsed = 0;

            mainGroup.alpha = 0;
            
            while (elapsed < transTime) {
                mainGroup.alpha = LerpKit.EaseIn(elapsed/transTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
                // Debug.Log($"elapsed:{elapsed} | elapsed<transTime:{elapsed<transTime}");
            }

            mainGroup.alpha = 1;

            countingDown = true;
            alreadyChanging = false;
        }
    }


    public void ExitCountdown()
    {
        alreadyChanging = true;
        StopCoroutine(ExitAttractRoutine());
        StartCoroutine(ExitAttractRoutine());

        IEnumerator ExitAttractRoutine()
        {
            mainGroup.alpha = 1;
                        
            float elapsed = 0;
            while (elapsed < transTime) {
                mainGroup.alpha = LerpKit.EaseIn(LerpKit.Flip(elapsed/transTime));
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            mainGroup.alpha = 0;
            eventSystem.enabled = true;

            countingDown = false;
            alreadyChanging = false;
        }
    }
}
