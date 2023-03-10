using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }
        [SerializeField] Transform _targetFinish;
        [SerializeField] GamePlaySetting _gamePlaySetting;
        [SerializeField] Counter _pointCounter;
        [SerializeField] UnityEvent<uint> _eventRunPrepare;
        [SerializeField] UnityEvent _eventRunStart;
        [SerializeField] UnityEvent _eventFinishRun;
        [SerializeField] UnityEvent _eventGameOver;
        [SerializeField] UnityEvent<uint> _pointUpdate;
        public GamePlaySetting GamePlaySetting => _gamePlaySetting;
        public Counter PointCounter => _pointCounter;
        public Vector3 FinishPosition => _targetFinish.position;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            _pointCounter.EventUpdateCount += InvokeUpdateCounter;
            InvokeUpdateCounter(_pointCounter.Count);
            StartCoroutine(DelayRun());
        }
        void InvokeUpdateCounter(uint updateCount)
        {
            _pointUpdate.Invoke(updateCount);
        }
        IEnumerator DelayRun()
        {
            yield return new WaitForSeconds(_gamePlaySetting.SecondForReady);
            _eventRunStart?.Invoke();
        }
        public void FinishRun()
        {
            _eventFinishRun?.Invoke();
        }
        public void CheckGameOver(uint countUnits)
        {
            if(countUnits <= 0)
                _eventGameOver?.Invoke();
        }
    }
}