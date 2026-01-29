using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ali.UI
{
    public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Settings")]
        public float hoverScale = 1.1f;
        public float pressScale = 0.95f;
        public float animationSpeed = 10f;
        
        [Header("Visuals")]
        public Image glowImage; // Optional: Assign an overlay image to fade in/out
        public Color hoverColor = Color.cyan;
        public Color normalColor = Color.white;

        private Vector3 originalScale;
        private Vector3 targetScale;
        private Image buttonImage;
        private Color originalColor;

        void Start()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
            
            buttonImage = GetComponent<Image>();
            if (buttonImage) originalColor = buttonImage.color;
            
            if (glowImage) glowImage.canvasRenderer.SetAlpha(0f);
        }

        void Update()
        {
            // Smoothly animate scale
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);

            // Smoothly animate color if we don't use a separate glow image
            if (buttonImage && glowImage == null)
            {
                buttonImage.color = Color.Lerp(buttonImage.color, (targetScale.x > originalScale.x) ? hoverColor : originalColor, Time.deltaTime * animationSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
            if (glowImage) glowImage.CrossFadeAlpha(1f, 0.2f, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
            if (glowImage) glowImage.CrossFadeAlpha(0f, 0.2f, true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = originalScale * pressScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale; // Return to hover state
        }
    }
}
