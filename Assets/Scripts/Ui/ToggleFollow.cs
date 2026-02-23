using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace DermapathologieVR.Ui
{
    /// <summary>
    /// Attach this to a UI Button. Taps toggle LazyFollow's Position Follow Mode between None and Follow.
    /// Also updates the button background color to reflect the state.
    /// - None  -> Grey background
    /// - Follow -> #0CB7E1 background
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ToggleFollow : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("LazyFollow instance to control.")]
        [SerializeField] private LazyFollow lazyFollow;

        [Tooltip("Button to listen for clicks. If not assigned, will use the Button on this GameObject.")]
        [SerializeField] private Button button;

        [Header("Colors")]
        [Tooltip("Background color when PositionFollowMode is None.")]
        [SerializeField] private Color colorNone = new Color(0.5f, 0.5f, 0.5f, 1f); // Grey

        [Tooltip("Background color when PositionFollowMode is Follow.")]
        [SerializeField] private Color colorFollow = new Color(12f / 255f, 183f / 255f, 225f / 255f, 1f); // #0CB7E1

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (lazyFollow == null)
                lazyFollow = GetComponent<LazyFollow>();

            if (button != null)
                button.onClick.AddListener(OnButtonClicked);
        }

        private void OnEnable()
        {
            // Ensure visuals reflect current state whenever enabled
            InitializeState();
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(OnButtonClicked);
        }

        private void Start()
        {
            // Requirement: Start with PositionFollowMode.None
            SetMode(LazyFollow.PositionFollowMode.None);
        }

        // Public method to hook up via Inspector if preferred
        public void Toggle()
        {
            if (lazyFollow == null)
            {
                Debug.LogWarning("ToggleFollow: LazyFollow reference is not set.", this);
                return;
            }

            var current = lazyFollow.positionFollowMode;
            var next = current == LazyFollow.PositionFollowMode.Follow
                ? LazyFollow.PositionFollowMode.None
                : LazyFollow.PositionFollowMode.Follow;

            SetMode(next);
        }

        private void OnButtonClicked()
        {
            Toggle();
        }

        private void InitializeState()
        {
            if (lazyFollow == null)
            {
                UpdateVisuals(isFollow: false);
                return;
            }

            UpdateVisuals(lazyFollow.positionFollowMode == LazyFollow.PositionFollowMode.Follow);
        }

        private void SetMode(LazyFollow.PositionFollowMode mode)
        {
            if (lazyFollow != null)
                lazyFollow.positionFollowMode = mode;

            UpdateVisuals(mode == LazyFollow.PositionFollowMode.Follow);
        }

        private void UpdateVisuals(bool isFollow)
        {
            var targetColor = isFollow ? colorFollow : colorNone;

            if (button != null)
            {
                var colors = button.colors;
                colors.normalColor = targetColor;
                button.colors = colors; // Assign back to apply changes
            }
        }
    }
}

