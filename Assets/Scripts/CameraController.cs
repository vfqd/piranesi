using System;
using EZCameraShake;
using Framework;
using UnityEngine;
using UnityEngine.U2D;
using Utils;

public class CameraController : MonoSingleton<CameraController>
{
    public CameraShaker shaker;
    public Transform parent;
    public PixelPerfectCamera pixelCamera;

    public Vector2 xBounds;
    public Vector2 yBounds;

    public float panSpeed;

    public float panZone;

    public bool lockPos;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        QualitySettings.vSyncCount = 1;
    }

    private void Update()
    {
        PanAtScreenEdge();
        Zoom();
    }

    public void SmallCameraShake()
    {
        shaker.ShakeOnce(3,2,0,.33f);
    }
    
    private void Zoom()
    {
        if (!GameController.Instance.hasLeftLondon) return;

        var mouseWheel = Input.mouseScrollDelta.y;

        if (mouseWheel > 0)
        {
            if (pixelCamera.assetsPPU < 24)
            {
                pixelCamera.assetsPPU += 2;
            }
        }
        else if (mouseWheel < 0)
        {
            if (pixelCamera.assetsPPU > 12)
            {
                pixelCamera.assetsPPU -= 2;
            }
        }
    }

    private void PanAtScreenEdge()
    {
        if (Cursor.lockState != CursorLockMode.Confined) return;
        if (!GameController.Instance.hasLeftLondon) return;
        if (lockPos) return;

        var pos = parent.transform.position;
        var p = Input.mousePosition;
        
        Vector2 delta = Vector2.zero;
        if (!(pos.x < xBounds.x) && p.x > -panZone && p.x < panZone) delta += new Vector2(-1, 0);
        else if (!(pos.x > xBounds.y) && p.x < Screen.width+panZone && p.x > Screen.width - panZone) delta += new Vector2(1, 0);
        
        if (!(pos.y < yBounds.x) && p.y > -panZone && p.y < panZone) delta += new Vector2(0, -1);
        else if (!(pos.y > yBounds.y) && p.y < Screen.height+panZone && p.y > Screen.height - panZone) delta += new Vector2(0, 1);

        delta = delta.normalized;
        parent.transform.position += (delta * (panSpeed * Time.deltaTime)).ToVector3();
    }
}