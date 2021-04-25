using UnityEngine;
using UnityEngine.EventSystems;

public class HoverResource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverPanel.Instance.ShowText(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverPanel.Instance.HideText();
    }
}