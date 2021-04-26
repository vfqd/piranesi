using System;
using Febucci.UI;
using Researches;
using SoundManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class DialogController : MonoSingleton<DialogController>
{
    public Transform dialogBox;

    public TextMeshProUGUI dialogPrefab;
    
    public EffectSoundBank effectSoundBank;

    public TheGate theGate;
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                MapController.Instance.AddNewAcolytes(2);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                MapController.Instance.AddNewPrisoners(2);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                GameController.Instance.LeaveLondon();
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                theGate.Complete();
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (var mapState in MapController.Instance.tilemaps)
                {
                    mapState.Explore();
                }

                ResourcesController.Instance.woodCount += 99;
                ResourcesController.Instance.metalsCount += 99;
            }
        }

        dialogBox.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        dialogBox.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
    }

    public void PlayDialog(string dialog)
    {
        effectSoundBank.Play();
        var text = Instantiate(dialogPrefab, dialogBox);
        text.rectTransform.localRotation = Quaternion.Euler(0,0,180);
        text.GetComponent<TextAnimatorPlayer>().ShowText(dialog);
    }
}