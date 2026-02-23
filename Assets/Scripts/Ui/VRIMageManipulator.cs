using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace DermapathologieVR.Ui
{
    /// <summary>
    /// Enables zoom, pan, and rotate manipulation of a RawImage in VR using XR ray interactors.
    
    
    /// Features:
    /// - Pan: Single trigger pressed and dragging
    /// - Zoom: Both triggers pressed and moving apart/together (pinch gesture)
    /// - Rotate: Both triggers pressed and rotating around image center
    /// 
    
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class VRImageManipulator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Manipulation Settings")]
        [SerializeField]

        float m_PanSensitivity = 0.5f;

        [SerializeField]
        
        float m_ZoomSensitivity = 2.0f;

        [SerializeField]
        
        float m_MinZoom = 1.0f;

        [SerializeField]
        
        float m_MaxZoom = 5.0f;

        [SerializeField]
        
        float m_RotationSensitivity = 100f;

        RawImage m_RawImage;
        RectTransform m_RectTransform;
        Canvas m_ParentCanvas;

        // Tracking pointer states
        struct PointerData
        {
            public int pointerId;
            public Vector2 currentPosition;
            public Vector2 previousPosition;
            public bool isActive;
        }

        PointerData m_Pointer1;
        PointerData m_Pointer2;

        // Image manipulation state
        float m_CurrentZoom = 1.0f;
        Vector2 m_CurrentPan = Vector2.zero;
        float m_CurrentRotation = 0f;

        // Cached values for two-finger gestures
        float m_LastTwoPointerDistance;
        float m_LastTwoPointerAngle;

        void Awake()
        {
            m_RawImage = GetComponent<RawImage>();
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentCanvas = GetComponentInParent<Canvas>();

            if (m_ParentCanvas == null)
            {
                Debug.LogError("VRImageManipulator requires a Canvas parent!", this);
                enabled = false;
            }
        }

        void Start()
        {
            // Initialize UV rect to show full image
            ResetImageTransform();
        }

        void Update()
        {
            // Handle manipulation based on active pointers
            // The pointer positions are updated via OnDrag events
            if (m_Pointer1.isActive && m_Pointer2.isActive)
            {
                HandleTwoPointerGesture();
            }
        }

        void HandleSinglePointerPan(ref PointerData pointer)
        {
            if (pointer.previousPosition == Vector2.zero)
            {
                pointer.previousPosition = pointer.currentPosition;
                return;
            }

            Vector2 delta = pointer.currentPosition - pointer.previousPosition;
            
            // Convert screen delta to UV delta
            Vector2 uvDelta = new Vector2(
                -delta.x / (m_RectTransform.rect.width * m_CurrentZoom),
                -delta.y / (m_RectTransform.rect.height * m_CurrentZoom)
            ) * m_PanSensitivity;

            m_CurrentPan += uvDelta;
            ApplyImageTransform();
        }

        void HandleTwoPointerGesture()
        {
            Vector2 currentMidpoint = (m_Pointer1.currentPosition + m_Pointer2.currentPosition) / 2f;
            Vector2 previousMidpoint = (m_Pointer1.previousPosition + m_Pointer2.previousPosition) / 2f;

            // Calculate current distance and angle between pointers
            float currentDistance = Vector2.Distance(m_Pointer1.currentPosition, m_Pointer2.currentPosition);
            Vector2 direction = m_Pointer2.currentPosition - m_Pointer1.currentPosition;
            float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Initialize on first frame of two-pointer gesture
            if (m_LastTwoPointerDistance == 0)
            {
                m_LastTwoPointerDistance = currentDistance;
                m_LastTwoPointerAngle = currentAngle;
                m_Pointer1.previousPosition = m_Pointer1.currentPosition;
                m_Pointer2.previousPosition = m_Pointer2.currentPosition;
                return;
            }

            // ZOOM: based on distance change
            float distanceDelta = currentDistance - m_LastTwoPointerDistance;
            if (Mathf.Abs(distanceDelta) > 0.01f)
            {
                float zoomDelta = distanceDelta * m_ZoomSensitivity * 0.01f;
                m_CurrentZoom = Mathf.Clamp(m_CurrentZoom + zoomDelta, m_MinZoom, m_MaxZoom);
            }

            // ROTATION: based on angle change
            float angleDelta = Mathf.DeltaAngle(m_LastTwoPointerAngle, currentAngle);
            if (Mathf.Abs(angleDelta) > 0.1f)
            {
                m_CurrentRotation += angleDelta * m_RotationSensitivity * 0.01f;
            }

            // PAN: based on midpoint movement (scaled by zoom for consistent feel)
            Vector2 midpointDelta = currentMidpoint - previousMidpoint;
            if (midpointDelta.sqrMagnitude > 0.001f)
            {
                Vector2 uvDelta = new Vector2(
                    -midpointDelta.x / (m_RectTransform.rect.width * m_CurrentZoom),
                    -midpointDelta.y / (m_RectTransform.rect.height * m_CurrentZoom)
                ) * m_PanSensitivity;
                m_CurrentPan += uvDelta;
            }

            m_LastTwoPointerDistance = currentDistance;
            m_LastTwoPointerAngle = currentAngle;

            ApplyImageTransform();
        }

        void ApplyImageTransform()
        {
            // Calculate UV rect size based on zoom
            float uvWidth = 1f / m_CurrentZoom;
            float uvHeight = 1f / m_CurrentZoom;

            // Clamp pan to keep image within bounds
            float maxPanX = Mathf.Max(0, (1f - uvWidth) / 2f);
            float maxPanY = Mathf.Max(0, (1f - uvHeight) / 2f);
            
            m_CurrentPan.x = Mathf.Clamp(m_CurrentPan.x, -maxPanX, maxPanX);
            m_CurrentPan.y = Mathf.Clamp(m_CurrentPan.y, -maxPanY, maxPanY);

            // Calculate UV rect center
            float centerX = 0.5f + m_CurrentPan.x;
            float centerY = 0.5f + m_CurrentPan.y;

            // Apply UV rect (zoom and pan)
            m_RawImage.uvRect = new Rect(
                centerX - uvWidth / 2f,
                centerY - uvHeight / 2f,
                uvWidth,
                uvHeight
            );

            // Apply rotation to the RectTransform
            m_RectTransform.localRotation = Quaternion.Euler(0, 0, m_CurrentRotation);
        }

        /// <summary>
        /// Reset the image to its original state
        /// </summary>
        public void ResetImageTransform()
        {
            m_CurrentZoom = 1.0f;
            m_CurrentPan = Vector2.zero;
            m_CurrentRotation = 0f;
            ApplyImageTransform();
        }

        #region Pointer Event Handlers

        public void OnPointerDown(PointerEventData eventData)
        {
            // Check if this is an XR UI pointer (from a controller)
            if (!(eventData is TrackedDeviceEventData))
                return;

            Vector2 localPoint;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_RectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
                return;

            // Assign to first available pointer slot
            if (!m_Pointer1.isActive)
            {
                m_Pointer1 = new PointerData
                {
                    pointerId = eventData.pointerId,
                    currentPosition = eventData.position,
                    previousPosition = eventData.position,
                    isActive = true
                };
            }
            else if (!m_Pointer2.isActive)
            {
                m_Pointer2 = new PointerData
                {
                    pointerId = eventData.pointerId,
                    currentPosition = eventData.position,
                    previousPosition = eventData.position,
                    isActive = true
                };

                // Reset two-pointer tracking when second pointer joins
                m_LastTwoPointerDistance = 0;
                m_LastTwoPointerAngle = 0;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Remove the pointer that was released
            if (m_Pointer1.isActive && m_Pointer1.pointerId == eventData.pointerId)
            {
                m_Pointer1.isActive = false;
                m_LastTwoPointerDistance = 0;
            }
            else if (m_Pointer2.isActive && m_Pointer2.pointerId == eventData.pointerId)
            {
                m_Pointer2.isActive = false;
                m_LastTwoPointerDistance = 0;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Update position if pointer is already active
            if (m_Pointer1.isActive && m_Pointer1.pointerId == eventData.pointerId)
            {
                m_Pointer1.currentPosition = eventData.position;
            }
            else if (m_Pointer2.isActive && m_Pointer2.pointerId == eventData.pointerId)
            {
                m_Pointer2.currentPosition = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Update the current position of the dragging pointer
            if (m_Pointer1.isActive && m_Pointer1.pointerId == eventData.pointerId)
            {
                m_Pointer1.previousPosition = m_Pointer1.currentPosition;
                m_Pointer1.currentPosition = eventData.position;
                
                // If only one pointer is active, handle pan immediately
                if (!m_Pointer2.isActive)
                {
                    HandleSinglePointerPan(ref m_Pointer1);
                }
            }
            else if (m_Pointer2.isActive && m_Pointer2.pointerId == eventData.pointerId)
            {
                m_Pointer2.previousPosition = m_Pointer2.currentPosition;
                m_Pointer2.currentPosition = eventData.position;
                
                // If only one pointer is active, handle pan immediately
                if (!m_Pointer1.isActive)
                {
                    HandleSinglePointerPan(ref m_Pointer2);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        #endregion

        #region Public Properties for Inspector

        public float PanSensitivity
        {
            get => m_PanSensitivity;
            set => m_PanSensitivity = value;
        }

        public float ZoomSensitivity
        {
            get => m_ZoomSensitivity;
            set => m_ZoomSensitivity = value;
        }

        public float RotationSensitivity
        {
            get => m_RotationSensitivity;
            set => m_RotationSensitivity = value;
        }

        public float MinZoom
        {
            get => m_MinZoom;
            set => m_MinZoom = Mathf.Max(0.1f, value);
        }

        public float MaxZoom
        {
            get => m_MaxZoom;
            set => m_MaxZoom = Mathf.Max(m_MinZoom, value);
        }

        #endregion
    }
}
