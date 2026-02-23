using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class StudyControlManager : MonoBehaviour
{
    [Header("Patient Image")]
    [SerializeField] private Transform patientImage;

    [Header("AI Suggestion GameObjects")]
    [SerializeField] private GameObject aiSuggestionAll;
    [SerializeField] private GameObject aiSuggestion1;
    [SerializeField] private GameObject aiSuggestion2;
    [SerializeField] private GameObject aiSuggestionNone;
    [SerializeField] private GameObject imageAll;
    [SerializeField] private GameObject image1;
    [SerializeField] private GameObject image2;
    [SerializeField] private GameObject imageNone;
    [SerializeField] private GameObject imageAlt;
    [SerializeField] private GameObject imageNext;

    [Header("Tools")]
    [SerializeField] private GameObject measureTool;
    [SerializeField] private GameObject markerTool;
    [SerializeField] private RawImage measureImage;
    [SerializeField] private RawImage markerImage;
    [SerializeField] private GameObject keyboard;


    [Header("Mic Indicators")]
    [SerializeField] private GameObject micIndicator1;
    [SerializeField] private GameObject micIndicator2;

    [Header("Second Opinion")]
    [SerializeField] private GameObject secondOpinion;
    [SerializeField] private RawImage secondOpinionImage;

    [Header("Send")]
    [SerializeField] private GameObject send;
    [SerializeField] private RawImage mail;

    [Header("Patient")]
    [SerializeField] private TextMeshProUGUI patientIndicator;
    [SerializeField] private GameObject patientOverview;

    [Header("Undo/Redo")]
    [SerializeField] private GameObject redo;

    [Header("Gesture Indicator")]
    [SerializeField] private RawImage gestureIndicator;
    [SerializeField] private GameObject gestureFrame;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip confirmSound;
    [SerializeField] private AudioClip wooshSound;

    // State variables
    private bool isSecondOpinionActive = false;
    private bool isSendActive = false;
    private bool isMicIndicator1Active = true;
    private bool isGestureIndicatorActive = false;
    private bool isKeyboardActive = false;
    private int currentPatientIndex = 0;
    private string[] patientNames = { "Kitchen1", "Kitchen2", "Kitchen3", "Kitchen4" };

    // Color constants
    private Color activeColor = new Color(0.047f, 0.718f, 0.882f, 1f); // #0CB7E1
    private Color inactiveColor = Color.white;

    // Coroutine references
    private Coroutine rotationCoroutine;
    private Coroutine zoomCoroutine;

    // Helper method to play sound effects
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void Update()
    {
        // Keyboard mappings using new Input System
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        if (keyboard.rKey.wasPressedThisFrame) RotateImage();
        if (keyboard.cKey.wasPressedThisFrame) ZoomInImage();
        if (keyboard.xKey.wasPressedThisFrame) ZoomOutImage();
        
        if (keyboard.digit1Key.wasPressedThisFrame) ShowAllAiSuggestion();
        if (keyboard.digit2Key.wasPressedThisFrame) ShowFirstAiSuggestion();
        if (keyboard.digit3Key.wasPressedThisFrame) ShowSecondAiSuggestion();
        if (keyboard.digit4Key.wasPressedThisFrame) HideAllAiSuggestion();
        if (keyboard.digit5Key.wasPressedThisFrame) SwitchToAltImage();
        
        if (keyboard.mKey.wasPressedThisFrame) ShowMeasureTool();
        if (keyboard.kKey.wasPressedThisFrame) ShowMarkerTool();
        if (keyboard.hKey.wasPressedThisFrame) HideAllTools();
        
        if (keyboard.vKey.wasPressedThisFrame) ToggleRecorder();
        if (keyboard.tKey.wasPressedThisFrame) ToggleKeyboard();
        if (keyboard.oKey.wasPressedThisFrame) ToggleSecondOpinion();
        if (keyboard.sKey.wasPressedThisFrame) ToggleSend();
        if (keyboard.nKey.wasPressedThisFrame) ToggleNextPatient();
        if (keyboard.pKey.wasPressedThisFrame) TogglePatientOverview();
        if (keyboard.uKey.wasPressedThisFrame) ToggleUndo();
        if (keyboard.gKey.wasPressedThisFrame) ToggleGestureIndicator();
    }

    // Rotate image 90 degrees to the left over 1 second
    public void RotateImage()
    {
        if (patientImage == null) return;

        PlaySound(wooshSound);
        
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }
        rotationCoroutine = StartCoroutine(RotateImageCoroutine());
    }

    private IEnumerator RotateImageCoroutine()
    {
        float duration = 1f;
        float elapsed = 0f;
        Quaternion startRotation = patientImage.localRotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, 0f, -90f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            patientImage.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        patientImage.localRotation = targetRotation;
        rotationCoroutine = null;
    }

    // Zoom in by 50%
    public void ZoomInImage()
    {
        if (patientImage == null) return;

        PlaySound(wooshSound);
        
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }
        zoomCoroutine = StartCoroutine(ZoomImageCoroutine(1.5f));
    }

    // Zoom out by 50%
    public void ZoomOutImage()
    {
        if (patientImage == null) return;

        PlaySound(wooshSound);
        
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }
        zoomCoroutine = StartCoroutine(ZoomImageCoroutine(1f / 1.5f));
    }

    private IEnumerator ZoomImageCoroutine(float zoomFactor)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startScale = patientImage.localScale;
        Vector3 targetScale = startScale * zoomFactor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            patientImage.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        patientImage.localScale = targetScale;
        zoomCoroutine = null;
    }

    // Switch to alternate image
    public void SwitchToAltImage()
    {
        PlaySound(clickSound);
        
        if (imageAll != null) imageAll.SetActive(false);
        if (image1 != null) image1.SetActive(false);
        if (image2 != null) image2.SetActive(false);
        if (imageNone != null) imageNone.SetActive(false);
        if (imageAlt != null) imageAlt.SetActive(true);
        if (imageNext != null) imageNext.SetActive(false);
    }

    // Show all AI suggestions
    public void ShowAllAiSuggestion()
    {
        PlaySound(clickSound);
        
        if (aiSuggestionAll != null) aiSuggestionAll.SetActive(true);
        if (aiSuggestion1 != null) aiSuggestion1.SetActive(false);
        if (aiSuggestion2 != null) aiSuggestion2.SetActive(false);
        if (aiSuggestionNone != null) aiSuggestionNone.SetActive(false);

        if (imageAll != null) imageAll.SetActive(true);
        if (image1 != null) image1.SetActive(false);
        if (image2 != null) image2.SetActive(false);
        if (imageNone != null) imageNone.SetActive(false);
        if (imageAlt != null) imageAlt.SetActive(false);
        if (imageNext != null) imageNext.SetActive(false);

    }

    // Show first AI suggestion
    public void ShowFirstAiSuggestion()
    {
        PlaySound(clickSound);
        
        if (aiSuggestionAll != null) aiSuggestionAll.SetActive(false);
        if (aiSuggestion1 != null) aiSuggestion1.SetActive(true);
        if (aiSuggestion2 != null) aiSuggestion2.SetActive(false);
        if (aiSuggestionNone != null) aiSuggestionNone.SetActive(false);

        if (imageAll != null) imageAll.SetActive(false);
        if (image1 != null) image1.SetActive(true);
        if (image2 != null) image2.SetActive(false);
        if (imageNone != null) imageNone.SetActive(false);
        if (imageAlt != null) imageAlt.SetActive(false);
        if (imageNext != null) imageNext.SetActive(false);
    }

    // Show second AI suggestion
    public void ShowSecondAiSuggestion()
    {
        PlaySound(clickSound);
        
        if (aiSuggestionAll != null) aiSuggestionAll.SetActive(false);
        if (aiSuggestion1 != null) aiSuggestion1.SetActive(false);
        if (aiSuggestion2 != null) aiSuggestion2.SetActive(true);
        if (aiSuggestionNone != null) aiSuggestionNone.SetActive(false);

        if (imageAll != null) imageAll.SetActive(false);
        if (image1 != null) image1.SetActive(false);
        if (image2 != null) image2.SetActive(true);
        if (imageNone != null) imageNone.SetActive(false);
        if (imageAlt != null) imageAlt.SetActive(false);
        if (imageNext != null) imageNext.SetActive(false);
    }

    // Hide all AI suggestions
    public void HideAllAiSuggestion()
    {
        PlaySound(clickSound);
        
        if (aiSuggestionAll != null) aiSuggestionAll.SetActive(false);
        if (aiSuggestion1 != null) aiSuggestion1.SetActive(false);
        if (aiSuggestion2 != null) aiSuggestion2.SetActive(false);
        if (aiSuggestionNone != null) aiSuggestionNone.SetActive(true);

        if (imageAll != null) imageAll.SetActive(false);
        if (image1 != null) image1.SetActive(false);
        if (image2 != null) image2.SetActive(false);
        if (imageNone != null) imageNone.SetActive(true);
        if (imageAlt != null) imageAlt.SetActive(false);
        if (imageNext != null) imageNext.SetActive(false);
    }

    // Show measure tool
    public void ShowMeasureTool()
    {
        PlaySound(clickSound);
        
        if (measureTool != null) measureTool.SetActive(true);
        if (measureImage != null) measureImage.color = activeColor;
    }

    // Show marker tool
    public void ShowMarkerTool()
    {
        PlaySound(clickSound);
        
        if (markerTool != null) markerTool.SetActive(true);
        if (markerImage != null) markerImage.color = activeColor;
    }

    // Hide all tools
    public void HideAllTools()
    {
        PlaySound(clickSound);
        
        if (measureTool != null) measureTool.SetActive(false);
        if (markerTool != null) markerTool.SetActive(false);
        if (measureImage != null) measureImage.color = inactiveColor;
        if (markerImage != null) markerImage.color = inactiveColor;
    }

    // Toggle recorder indicators
    public void ToggleRecorder()
    {
        PlaySound(clickSound);
        
        isMicIndicator1Active = !isMicIndicator1Active;
        
        if (micIndicator1 != null) micIndicator1.SetActive(isMicIndicator1Active);
        if (micIndicator2 != null) micIndicator2.SetActive(!isMicIndicator1Active);
    }

        // Toggle recorder indicators
    public void ToggleKeyboard()
    {
        PlaySound(clickSound);
        
        isKeyboardActive = !isKeyboardActive;
        
        if (keyboard != null) keyboard.SetActive(isKeyboardActive);
    }

    // Toggle second opinion
    public void ToggleSecondOpinion()
    {
        PlaySound(confirmSound);
        
        isSecondOpinionActive = !isSecondOpinionActive;
        
        if (secondOpinion != null) secondOpinion.SetActive(isSecondOpinionActive);
        if (secondOpinionImage != null)
        {
            secondOpinionImage.color = isSecondOpinionActive ? activeColor : inactiveColor;
        }
    }

    // Toggle send
    public void ToggleSend()
    {
        PlaySound(confirmSound);
        
        isSendActive = !isSendActive;
        
        if (send != null) send.SetActive(isSendActive);
        if (mail != null)
        {
            mail.color = isSendActive ? activeColor : inactiveColor;
        }
    }

    // Toggle through patients
    public void ToggleNextPatient()
    {
        PlaySound(clickSound);
        
        currentPatientIndex = (currentPatientIndex + 1) % patientNames.Length;
        
        if (patientIndicator != null)
        {
            patientIndicator.text = patientNames[currentPatientIndex];
        }
        if (imageAll != null) imageAll.SetActive(false);
        if (image1 != null) image1.SetActive(false);
        if (image2 != null) image2.SetActive(false);
        if (imageNone != null) imageNone.SetActive(false);
        if (imageAlt != null) imageAlt.SetActive(false);
        if (imageNext != null) imageNext.SetActive(true);
    }

    // Toggle patient overview
    public void TogglePatientOverview()
    {
        PlaySound(clickSound);
        
        if (patientOverview != null)
        {
            patientOverview.SetActive(!patientOverview.activeSelf);
        }
    }

    // Toggle undo/redo
    public void ToggleUndo()
    {
        PlaySound(confirmSound);
        
        if (redo != null)
        {
            redo.SetActive(!redo.activeSelf);
        }
    }

    // Toggle gesture indicator
    public void ToggleGestureIndicator()
    {
        PlaySound(clickSound);
        
        isGestureIndicatorActive = !isGestureIndicatorActive;
        
        if (gestureIndicator != null)
        {
            gestureIndicator.color = isGestureIndicatorActive ? activeColor : inactiveColor;
        }
        
        if (gestureFrame != null)
        {
            gestureFrame.SetActive(isGestureIndicatorActive);
        }
    }
}
