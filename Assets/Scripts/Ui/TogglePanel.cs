using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach this to a UI Button. Clicking toggles a target panel's active state and updates the button label.
/// - Default state on start: panel off, button text = "Show Report"
/// - When on: panel on, button text = "Hide Report"
/// </summary>
[RequireComponent(typeof(Button))]
public class TogglePanel : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The UI panel (GameObject) to show/hide.")]
    [SerializeField] private GameObject targetPanel;

    [Tooltip("TextMeshPro label to update on this button.")]
    [SerializeField] private TMP_Text buttonLabel;

    [Header("Labels")]
    [SerializeField] private string showText = "Show Report";
    [SerializeField] private string hideText = "Hide Report";

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnEnable()
    {
        // Default: panel off, label = Show Report
        SetPanelActive(false);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnButtonClicked);
    }

    // Public method for manual hookup if preferred
    public void Toggle()
    {
        bool nextState = !(targetPanel != null && targetPanel.activeSelf);
        SetPanelActive(nextState);
    }

    private void OnButtonClicked()
    {
        Toggle();
    }

    private void SetPanelActive(bool active)
    {
        if (targetPanel != null)
            targetPanel.SetActive(active);

        if (buttonLabel != null)
            buttonLabel.text = active ? hideText : showText;
    }
}

