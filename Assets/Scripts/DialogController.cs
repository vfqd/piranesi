using System;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class DialogController : MonoSingleton<DialogController>
{
    public Transform dialogBox;

    public TextMeshProUGUI dialogPrefab;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MapController.Instance.AddNewAcolytes(2);
        }
        
        dialogBox.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        dialogBox.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
    }

    public void PlayDialog(string dialog)
    {
        var text = Instantiate(dialogPrefab, dialogBox);
        text.rectTransform.localRotation = Quaternion.Euler(0,0,180);
        text.GetComponent<TextAnimatorPlayer>().ShowText(dialog);
    }
}