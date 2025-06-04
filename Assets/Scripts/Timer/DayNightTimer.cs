using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering.Universal; // For 2D lighting

public class DayNightTimer : MonoBehaviour
{
    // Total time for the full day cycle
    [SerializeField] private float totalDayTime;
    private float timePassed = 0f;

    // UI to show time left
    public TMP_Text timerText;

    // Lighting
    public Light2D globalLight;
    public Gradient skyColors;
    public AnimationCurve brightnessOverTime;

    // Sound for night
    public AudioSource audioSource;
    public AudioClip nightAmbientClip;
    private bool hasPlayedNightAudio = false;

    // Show progress in console
    public bool showDebugTime = true;

    private bool dayIsOver = false;

    void Start()
    {
        timePassed = 0f;

        // Set starting light color and brightness
        if (globalLight != null)
        {
            globalLight.color = skyColors.Evaluate(0f);
            globalLight.intensity = brightnessOverTime.Evaluate(0f);
        }

        UpdateCountdownUI();
    }

    void Update()
    {
        if (timePassed < totalDayTime)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / totalDayTime;

            UpdateLighting(progress);
            UpdateCountdownUI();

            if (progress > 0.5f && !hasPlayedNightAudio)
            {
                PlayNightAmbientSound();
            }

            if (showDebugTime)
            {
                //Debug.Log("Progress: " + progress.ToString("F2"));
            }
        }
        else if (!dayIsOver)
        {
            dayIsOver = true;
            PlayNightAmbientSound();
            timerText.text = "Time Left: 00:00";
            timerText.color = Color.red;
        }
    }

    void UpdateLighting(float progress)
    {
        if (globalLight != null)
        {
            globalLight.color = skyColors.Evaluate(progress);
            globalLight.intensity = Mathf.Clamp(brightnessOverTime.Evaluate(progress), 0.2f, 1f);
        }
    }

    void UpdateCountdownUI()
    {
        float timeLeft = Mathf.Max(0f, totalDayTime - timePassed);
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = "Time Left: " + minutes.ToString("00") + ":" + seconds.ToString("00");

        if (timeLeft <= 60f)
            timerText.color = Color.red;
        else if (timeLeft <= 180f)
            timerText.color = Color.yellow;
        else
            timerText.color = Color.white;
    }

    void PlayNightAmbientSound()
    {
        if (audioSource != null && nightAmbientClip != null)
        {
            audioSource.clip = nightAmbientClip;
            audioSource.loop = true;
            StartCoroutine(FadeInAudio(3f, 1f));
            hasPlayedNightAudio = true;
        }
    }

    IEnumerator FadeInAudio(float duration, float targetVolume)
    {
        audioSource.volume = 0f;
        audioSource.Play();

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    public float GetProgress()
    {
        return Mathf.Clamp01(timePassed / totalDayTime);
    }
    
    public float GetTimeRemaining()
    {
        return Mathf.Max(0f, totalDayTime - timePassed);
    }

    public float GetTotalTime()
    {
        return totalDayTime;
    }


}
