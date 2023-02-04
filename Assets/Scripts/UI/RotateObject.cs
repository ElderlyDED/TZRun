using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] Vector3 rotateDirection;
        void Awake()
        {
            var tween = transform.DORotate(rotateDirection.normalized * 360, 7f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
        void OnDestroy()
        {
            DOTween.Kill(transform);
        }
    }
}