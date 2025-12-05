using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Objects to show on hover")]
    public GameObject[] hoverObjects;   // assign objects that appear around the button

    // Called when mouse starts hovering
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var obj in hoverObjects)
        {
            if (obj != null) obj.SetActive(true);
        }
    }

    // Called when mouse stops hovering
    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var obj in hoverObjects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}
