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
            var text = Instantiate(dialogPrefab, dialogBox);
            text.rectTransform.localRotation = Quaternion.Euler(0,0,180);
            text.GetComponent<TextAnimatorPlayer>().ShowText(" - " + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " "
                        + Random.Range(0, 100000) + " ");
        }
        dialogBox.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        dialogBox.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
    }
}