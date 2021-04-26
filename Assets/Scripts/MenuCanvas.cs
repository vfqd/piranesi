using System;
using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    public Canvas mainCanvas;

    private void Update()
    {
        if (Input.anyKey)
        {
            mainCanvas.gameObject.SetActive(true);
            GameController.Instance.Commence();
            Destroy(gameObject);
        }
    }
}