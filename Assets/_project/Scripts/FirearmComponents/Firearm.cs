using _project.Scripts.Core.Variables.References;
using Core.Events;
using Core.Sets;
using EntityComponents.UnitComponents.EnemyComponents;
using EntityComponents.UnitComponents.Movement;
using EntityComponents.UnitComponents.PlayerComponents;
using UnityEngine;

namespace FirearmComponents
{
    public class Firearm : MonoBehaviour
    {
        [Space] [Header("Inner components")]
        [SerializeField] private MagazineReserve magazineReserve;
        [Space] [Header("Settings")]
        [SerializeField] private StackPool ammoPool;
        [SerializeField] private AimMode aimMode;
        [SerializeField] private Vector2Reference selfAimDirection;
        [SerializeField] private TargetRuntimeSet visibleEnemies;
        [Space] [Header("Stats")]
        [SerializeField] private FloatReference attackSpeed;
        [SerializeField] private FloatReference projectilesPerAttack;
        [SerializeField] private FloatReference maxShootDeflectionAngle;
        [Space] [Header("Events")]
        [SerializeField] private GameEvent onShoot;

        public bool OnCoolDown => _coolDownTimer > 0;
        public bool CanShoot => _coolDownTimer <= 0
                                && magazineReserve.Value > 0
                                && !magazineReserve.OnReload;
        private float _coolDownTimer;
        private Target _currentTarget;
        
        private Transform _transform;

        private void Awake()
        {
            if (_transform == null) _transform = transform;
            if (magazineReserve == null) magazineReserve = GetComponent<MagazineReserve>();
        }

        private void FixedUpdate()
        {
            if (_coolDownTimer > 0)
            {
                _coolDownTimer -= Time.fixedDeltaTime;
            }
            UpdateTarget();
        }

        public void DoAction()
        {
            if (!CanShoot) return;
            Shoot();
        }

        public void ChangeAimMode()
        {
            if (aimMode == AimMode.AutoAim)
            {
                aimMode = AimMode.SelfAim;
            }
            else
            {
                aimMode = AimMode.AutoAim;
            }
        }

        private void Shoot()
        {
            magazineReserve.Pop();

            var direction = GetShotDirection();
            var projSpread = maxShootDeflectionAngle;
            var projCount = (int)projectilesPerAttack;
            var fireAngle = projSpread * (projCount - 1);
            var halfFireAngleRad = fireAngle * 0.5f * Mathf.Deg2Rad;
            var leftDirection = MathFirearm.Rotate(direction, -halfFireAngleRad);
            var actualShotDirection = leftDirection;

            for (int i = 0; i < projCount; i++)
            {
                var projectile = ammoPool.Get();
                projectile.transform.SetPositionAndRotation(_transform.position, _transform.rotation);
                var projectileMovementController = projectile.GetComponent<ProjectileMovementController>();
                projectileMovementController.SetDirection(actualShotDirection);
                var launchAngle = maxShootDeflectionAngle * Mathf.Deg2Rad;
                actualShotDirection = MathFirearm.Rotate(actualShotDirection, launchAngle);
            }

            _coolDownTimer = 1f / attackSpeed;
        }

        private Vector2 GetShotDirection()
        {
            if (aimMode == AimMode.SelfAim)
            {
                return selfAimDirection.Value;
            }

            if (_currentTarget == null) return Random.insideUnitCircle;
            Vector2 direction = _currentTarget.Transform.position - _transform.position;
            direction.Normalize();
            return direction;
        }

        private void UpdateTarget()
        {
            if (aimMode == AimMode.SelfAim)
            {
                if (_currentTarget == null) return;
                _currentTarget.RemoveFromCurrent();
                _currentTarget = null;
                return;
            }
            var nearestTarget = visibleEnemies.GetNearestToPosition(_transform.position);
            if (nearestTarget == null)
            {
                _currentTarget = null;
                return;
            }
            if (_currentTarget != null)
            {
                if (_currentTarget == nearestTarget) return;
                _currentTarget.RemoveFromCurrent();
            }
            _currentTarget = nearestTarget;
            _currentTarget.TakeAsCurrent();
        }
    }
}