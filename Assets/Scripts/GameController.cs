using System;
using System.Collections;
using Researches;
using SoundManager;
using UnityEngine;
using Utils;
using Utils.MonoBehaviourMethodExtensions;

public class GameController : MonoSingleton<GameController>
{
    public _2dxFX_DesintegrationFX londonBg;
    
    public bool hasLeftLondon;
    public bool createHierophants;

    public EffectSoundBank rain, burn, waves;

    private EffectSoundInstance _rainInstance, _wavesInstance;

    private void Start()
    {
        CameraController.Instance.pixelCamera.upscaleRT = true;
        _rainInstance = rain.Play();
        _rainInstance.SetLooping(true);
    }

    public void Commence()
    {
        // LeaveLondon();
        DialogController.Instance.PlayDialog("London is awash with the rains of winter. The water sluices off the wet cobblestones. The unbelievers slumber.");
        Invoke(nameof(StartDialog1),5);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot("LondonBackground.png");
        }
    }

    private void StartDialog1()
    {
        DialogController.Instance.PlayDialog("The time has come for me to undertake the Great and Necessary Work. I must start by raising an altar.");
    }

    private bool _altarDialogPlayed = false;
    public void PlacedAltarDialog()
    {
        if (!_altarDialogPlayed)
        {
            _altarDialogPlayed = true;
            DialogController.Instance.PlayDialog("This will serve as a place to begin the rites. I should begin my fervour by picking one.");
        }
    }

    public void LeaveLondon()
    {
        hasLeftLondon = true;
        MapController.Instance.tilemaps[1].Explore();
        CameraController.Instance.transform.position = new Vector3(-1.1f, -0.2f, -10f);
        CameraController.Instance.pixelCamera.upscaleRT = false;
        StartCoroutine(DisintegrateScreen());
        _rainInstance.FadeOutAndDestroy(.5f,false);
        burn.Play();
        
        _wavesInstance = waves.Play();
        _wavesInstance.FadeIn(3, true);
        _wavesInstance.SetLooping(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private IEnumerator DisintegrateScreen()
    {
        londonBg.gameObject.SetActive(true);
        float dT = 0;

        while (dT < 1)
        {
            dT += Time.deltaTime;
            londonBg.Desintegration = dT;
            yield return null;
        }
        londonBg.gameObject.SetActive(false);
    }
}