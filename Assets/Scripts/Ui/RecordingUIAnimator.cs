using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecordingUIAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private RawImage recordingIndicator;
    
    [Header("Animation Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;
    
    private float elapsedTime = 0f;
    private float pulseTime = 0f;

    void OnEnable()
    {
        // Reset timer when the GameObject becomes active
        elapsedTime = 0f;
        pulseTime = 0f;
        UpdateTimerText();
    }

    void Update()
    {
        // Update timer
        elapsedTime += Time.deltaTime;
        UpdateTimerText();
        
        // Pulse the recording indicator
        if (recordingIndicator != null)
        {
            pulseTime += Time.deltaTime * pulseSpeed;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(pulseTime) + 1f) / 2f);
            
            Color color = recordingIndicator.color;
            color.a = alpha;
            recordingIndicator.color = color;
        }
    }
    
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
    }
}
