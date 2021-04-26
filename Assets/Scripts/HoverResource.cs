using UnityEngine;
using UnityEngine.EventSystems;

public class HoverResource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        string s = text;
        if (text.Equals("Knowledge"))
        {
            s += " per day";
        }
        HoverPanel.Instance.ShowText(s);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverPanel.Instance.HideText();
    }
}