using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] Rigidbody _centerMove;
        [SerializeField] UnityEvent<uint> _unitsCountsUpdate;
        [SerializeField] ParticleSystem _dieParticle;
        Vector3 _runVelocity;
        List<Unit> _units;
        uint _countUnits;
        Vector3[] _offsetFromTarget;
        uint CountUnits
        {
            get => _countUnits;
            set
            {
                _countUnits = value;
                _unitsCountsUpdate.Invoke(_countUnits);
            }
        }
        Unit FirstInactive => _units.FirstOrDefault(unit => !unit.gameObject.activeSelf);
        Unit FirstActive => _units.FirstOrDefault(unit => unit.gameObject.activeSelf);
        List<Unit> AllActive => _units.FindAll(unit => unit.gameObject.activeSelf);
        Coroutine proccessChangeWalkToRun;
        public Rigidbody CenterMove => _centerMove;
        public Vector3 RunVelocity => _runVelocity;
        void Awake()
        {
            InitUnits();
        }
        void FixedUpdate()
        {
            _centerMove.MovePosition(_centerMove.position + _runVelocity * Time.deltaTime);
        }
        void InitUnits()
        {
            _units = GetComponentsInChildren<Unit>().ToList();
            _units.ForEach(unit =>
            {
                SubscribeOnEventsUnit(unit);
                unit.SetCenter(_centerMove.transform);
                unit.gameObject.SetActive(false);
            });
            CountUnits = Game.Instance.GamePlaySetting.StartFromUnit;
            for (var i = 0; i < Game.Instance.GamePlaySetting.StartFromUnit; i++)
            {
                _units[i].gameObject.SetActive(true);
            }
            SetRectangleOffset();
            RelocateUnits();
        }
        void SubscribeOnEventsUnit(Unit unit)
        {
            unit.EventDeath += RemoveUnit;
        }
        IEnumerator UpdateWalkToRun()
        {
            yield return new WaitForSeconds(Game.Instance.GamePlaySetting.SafeForRun);
            UpdateToRun();
        }
        void UpdateToWalk()
        {
            AllActive.ForEach(unit =>
            {
                unit.MoveVector = _runVelocity;
                unit.Walk();
            });
        }
        void UpdateToRun()
        {
            AllActive.ForEach(unit => { unit.Run(); });
        }
        void UpdateToFinish()
        {
            AllActive.ForEach(unit =>
            {
                _runVelocity = Vector3.zero;
                unit.MoveVector = Vector3.zero;
                unit.Dance();
            });
        }
        void RelocateUnits()
        {
            if (_countUnits > 0)
            {
                var units = AllActive;
                var pointsPerUnit = (float) _offsetFromTarget.Length / _countUnits;
                var iUnit = 0;
                for (var i = 0f; i < _offsetFromTarget.Length && iUnit < _countUnits; i += pointsPerUnit)
                {
                    units[iUnit++].MoveOffset = _offsetFromTarget[Mathf.FloorToInt(i)];
                }
            }
        }
        void SetRectangleOffset()
        {
            int row = (int) (CountUnits / 5), col = 5;
            float colSpace = .4f, rowSpace = .4f;
            var offsets = new Vector3[row * col];
            var startPoint = _centerMove.position.normalized + Vector3.forward * ((rowSpace / 2 * row)) +
                             Vector3.left * ((colSpace / 2 * col));
            for (var i = 0; i < row; ++i)
            {
                for (var j = 0; j < col; ++j)
                {
                    offsets[i * col + j] = startPoint + Vector3.back * ((i + 1) * rowSpace) +
                                           Vector3.right * ((j + 1) * colSpace);
                }
            }
            _offsetFromTarget = offsets;
        }
        public void CurveUpdate(Vector3[] offsetPoints)
        {
            _offsetFromTarget = offsetPoints;
            RelocateUnits();
        }
        public void StartRun()
        {
            var distance = Vector3.Distance(Game.Instance.FinishPosition, _centerMove.position);
            _runVelocity = Vector3.forward * (distance / Game.Instance.GamePlaySetting.LevelTime);
            UpdateToWalk();
            proccessChangeWalkToRun = StartCoroutine(UpdateWalkToRun());
        }
        public void EndRun()
        {
            RelocateUnits();
            SetRectangleOffset();
            UpdateToFinish();
            StopCoroutine(proccessChangeWalkToRun);
        }
        public void RemoveUnit(Unit unit)
        {
            --CountUnits;
            //unit.gameObject.SetActive(false);
            StartCoroutine(DieDelay(unit));
            UpdateToWalk();
            StopCoroutine(proccessChangeWalkToRun);
            proccessChangeWalkToRun = StartCoroutine(UpdateWalkToRun());
        }
        public void AddUnit()
        {
            var unit = FirstInactive;
            if (unit == null) return;
            unit.gameObject.SetActive(true);
            unit.transform.position = FirstActive.transform.position;
            unit.MoveVector = _runVelocity;
            ++CountUnits;
            RelocateUnits();
        }
        void OnDrawGizmos()
        {
            if (_offsetFromTarget?.Length > 0 && _centerMove != null)
            {
                var pastPoint = _offsetFromTarget[0];
                foreach (var point in _offsetFromTarget.Skip(1))
                {
                    Gizmos.color = Color.red;
                    
                    Gizmos.DrawLine(_centerMove.position + pastPoint, _centerMove.position + point);
                    pastPoint = point;
                }
            }
        }
        IEnumerator DieDelay(Unit _unit)
        {
            _unit.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(0.2f);
            _unit.gameObject.SetActive(false);
        }
    }
}