using System;
using System.Collections;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utils;

namespace UI
{
    public class CurveDrawer : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
    {
        RectTransform _rectTransform;
        [SerializeField] SplineComputer _splineComputer;
        [SerializeField] SplineRenderer _splineRenderer;
        [SerializeField] Camera _canvasCamera;
        [SerializeField] UnityEvent<Vector3[]> _splineComplete;
        int _indexPoint;
        Coroutine _trackPointer;
        private Vector3[] GetOffsetFromTarget()
        {
            var points = _splineComputer.GetPoints().ToSimplify(0.9f);
            var offsets = new Vector3[points.Length];
            var worldCenterPointDrawPanel = _rectTransform.TransformPoint(_rectTransform.rect.center);
            for (var i = 0; i < points.Length; ++i)
            {
                var vectorToPoint = worldCenterPointDrawPanel - points[i].position;
                offsets[i] = new Vector3(-vectorToPoint.x, 0, -vectorToPoint.y);
            }
            return offsets;
        }
        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        IEnumerator OnMove()
        {
            Vector2 pointPos = Vector2.zero;
            while (Application.isPlaying)
            {
                Vector2 currentPosition =
                    Input.touchCount < 1 ? (Vector2) Input.mousePosition : Input.GetTouch(0).position;
                if (currentPosition != pointPos)
                {
                    pointPos = currentPosition;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, pointPos, _canvasCamera,
                        out Vector3 worldPoint);
                    _splineComputer.SetPoint(_indexPoint++, new SplinePoint(worldPoint + Vector3.back * 0.05f));
                }
                yield return null;
            }
        }
        void StopTrack()
        {
            if (_trackPointer != null)
            {
                _splineComplete.Invoke(GetOffsetFromTarget());
                _indexPoint = 0;
                _splineComputer.SetPoints(new SplinePoint[0]);
                StopCoroutine(_trackPointer);
                _trackPointer = null;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            StopTrack();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _trackPointer = StartCoroutine(OnMove());
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            StopTrack();
        }
    }
}