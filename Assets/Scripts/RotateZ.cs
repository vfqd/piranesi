using System;
using UnityEngine;

public class RotateZ : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Update()
    {
        var rot = transform.localRotation.ToEulerAngles();
        rot.z += speed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rot);
    }
}