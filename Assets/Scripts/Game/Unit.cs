using System;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public class Unit : MonoBehaviour
    {
        static readonly int IdMovement = Animator.StringToHash("Movement");
        static readonly int IdDeath = Animator.StringToHash("Death");
        static readonly int IdFinalDance = Animator.StringToHash("Finish");
        public event Action<Unit> EventDeath;
        [SerializeField] Animator _animator;
        Rigidbody _rigidbody;
        SphereCollider _collider;
        Transform _center;
        Vector3 _moveVector = Vector3.zero;
        Vector3 _offsetMoveVector = Vector3.zero;

        public Vector3 MoveVector
        {
            get => _moveVector;
            set => _moveVector = value;
        }
        public Vector3 MoveOffset
        {
            get => _offsetMoveVector;
            set => _offsetMoveVector = value;
        }
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();
        }
        public void SetCenter(Transform centeredTransform)
        {
            _center = centeredTransform;
        }
        public void Walk()
        {
            _animator.SetFloat(IdMovement, 0.5f);
        }
        public void Run()
        {
            _animator.SetFloat(IdMovement, 1f);
        }
        public void Dance()
        {
            _animator.SetTrigger(IdFinalDance);
        }
        public void Kill()
        {
            _animator.SetTrigger(IdDeath);
            EventDeath?.Invoke(this);
        }
        void FixedUpdate()
        {
            var displacementFromOffset =
                (_center.position + _offsetMoveVector - transform.position) *
                Game.Instance.GamePlaySetting.SpeedReplaceUnit + _moveVector;
            _rigidbody.MovePosition(transform.position + displacementFromOffset * Time.deltaTime);
        }
        void OnDrawGizmos()
        {
            if (_center == null) return;
            Gizmos.DrawWireSphere(_center.position + _offsetMoveVector, .1f);
        }
    }
}