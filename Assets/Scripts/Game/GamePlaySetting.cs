using UnityEngine;

namespace Game
{
    public class GamePlaySetting : ScriptableObject
    {
        [SerializeField] float _levelTime;
        [SerializeField] float _safeForRun;
        [SerializeField] uint _startFromUnit;
        [SerializeField] uint _secondForReady;
        [SerializeField] float _speedReplaceUnit;
        public float LevelTime => _levelTime;
        public float SafeForRun => _safeForRun;
        public uint StartFromUnit => _startFromUnit;
        public uint SecondForReady => _secondForReady;
        public float SpeedReplaceUnit => this._speedReplaceUnit;
    }
}
