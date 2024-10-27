using Core.Sets;
using Core.Variables.References;
using EntityComponents.UnitComponents.EnemyComponents;
using UnityEngine;

namespace FirearmComponents
{
    public class Aim : MonoBehaviour
    {
        [SerializeField] private TargetRuntimeSet targets;
        [SerializeField] private bool displayTarget;
        [SerializeField] private AimMode mode;
        [SerializeField] private Vector2Reference selfAimDirection;
        [SerializeField] private bool zeroDirectionIfNoTargets; // previously "inCamBounds"
        [Space]
        [Header("Stats")]
        [SerializeField] private FloatReference radius;
        [SerializeField] private bool infiniteRadius; // previously "inCamBounds"
        
        private Transform Transform
        {
            get
            {
                if (_transform != null) return _transform;
                _transform = transform;
                return _transform;
            }
        }
        private Transform _transform;
        private Vector2 _direction;
        private Target _target;

        private void FixedUpdate()
        {
            UpdateTarget();
        }

        private void OnDrawGizmos()
        {
            if (!radius.useConstant && radius.variable == null)
            {
                return;
            }
            
            Gizmos.DrawRay(Transform.position, _direction);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_transform.position, Vector2.up * radius);
            Gizmos.DrawRay(_transform.position, Vector2.left * radius);
            Gizmos.DrawRay(_transform.position, Vector2.right * radius);
            Gizmos.DrawRay(_transform.position, Vector2.down * radius);
        }

        public Vector2 GetDirection()
        {
            if (mode == AimMode.SelfAim) return selfAimDirection.Value;
            if (_target == null)
            {
                if (zeroDirectionIfNoTargets)
                {
                    return Vector2.zero;
                }
                _direction = Random.onUnitSphere;
                return _direction.normalized;
            }
            _direction = _target!.Transform.position - Transform.position;
            return _direction.normalized;
        }

        private void UpdateTarget()
        {
            if (mode == AimMode.SelfAim)
            {
                if (_target == null) return;
                
                if (displayTarget) _target.RemoveFromCurrent();
                _target = null;
                return;
            }

            var nearestTarget = infiniteRadius ?
                targets.GetNearestToPosition(Transform.position) :
                targets.GetNearestToCenterInCircle(Transform.position, radius);

            if (nearestTarget == null)
            {
                _target = null;
                return;
            }
            if (_target != null)
            {
                if (_target == nearestTarget) return;
                if (displayTarget) _target.RemoveFromCurrent();
            }
            _target = nearestTarget;
            if (displayTarget) _target.TakeAsCurrent();
        }


        public void ToggleMode()
        {
            mode = mode == AimMode.AutoAim ? AimMode.SelfAim : AimMode.AutoAim;
        }

        public void SetAutoMode(bool autoAim)
        {
            mode = autoAim ? AimMode.AutoAim : AimMode.SelfAim;
        }
    }
}