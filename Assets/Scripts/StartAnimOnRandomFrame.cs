using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartAnimOnRandomFrame : MonoBehaviour
{
    public Animator anim;
    private void Start()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        anim.GetComponent<SpriteRenderer>().flipX = Random.value < 0.25f;
        anim.Play(info.fullPathHash, -1, Random.Range(0f, 1f));
    }
}