using UnityEngine;
using UnityEngine.EventSystems;

// Attach to each button GameObject
public class UIButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float hoverScale = 1.06f;
    [SerializeField] float pressScale = 0.98f;
    [SerializeField] float lerpSpeed = 12f;

    Vector3 baseScale;
    Vector3 targetScale;

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * lerpSpeed);
    }

    public void OnPointerEnter(PointerEventData _) => targetScale = baseScale * hoverScale;
    public void OnPointerExit(PointerEventData _)  => targetScale = baseScale;
    public void OnPointerDown(PointerEventData _)  => targetScale = baseScale * pressScale;
    public void OnPointerUp(PointerEventData _)    => targetScale = baseScale * hoverScale;
}