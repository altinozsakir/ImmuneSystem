using UnityEngine;
using Core.TimeSystem;
using Core.Pathing;
using Core.Combat; // CrowdControl


namespace Core.Enemies
{
    [RequireComponent(typeof(EnemyMoverSpline))]
    public class EnemySpeedPhaseAdapter : MonoBehaviour
    {
        public BodyClockDirector clock;
        EnemyMoverSpline _mover; CrowdControl _cc;
        float _baseSpeed; float _phaseScale = 1f;


        void Awake()
        {
            _mover = GetComponent<EnemyMoverSpline>();
            _cc = GetComponent<CrowdControl>();
            _baseSpeed = _mover ? _mover.speed : 0f;
        }


        void OnEnable()
        {
            if (!clock) clock = FindAnyObjectByType<BodyClockDirector>();
            if (clock)
            {
                clock.OnMultipliersChanged += OnMult;
                _phaseScale = clock.Multipliers.enemySpeed;
            }
        }
        void OnDisable() { if (clock) clock.OnMultipliersChanged -= OnMult; }


        void Update()
        {
            if (!_mover) return;
            float cc = (_cc ? _cc.SpeedMultiplier : 1f);
            _mover.speed = _baseSpeed * _phaseScale * cc;
        }


        void OnMult(PhaseMultipliers m) { _phaseScale = Mathf.Max(0f, m.enemySpeed); }
    }
}