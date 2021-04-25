using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    public Sprite[] possibleSprites;
    [FormerlySerializedAs("moveSpeed")] public float moveTime;
    public float maxDist;

    private SpriteRenderer _spriteRenderer;
    private Vector2 _startPos;
    private float _moveTimer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
        _startPos = transform.position;
        _moveTimer = Random.Range(1,5);
    }

    private void Update()
    {
        _moveTimer -= Time.deltaTime;

        if (_moveTimer < 0)
        {
            _moveTimer = Random.Range(3, 9);
            int angle = Random.Range(0, 360);
            Vector2 targetPos = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) *
                                maxDist;
            transform.DOMove(_startPos + targetPos, moveTime).SetEase(Ease.Linear);
        }
    }
}