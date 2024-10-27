using System;
using Core.Events;
using Core.Variables.References;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FirearmComponents
{
    /// <summary>
    /// Берёт из Magazine _magazine снаряды и направляет их по принципу определённому в Aim _aim
    /// </summary>
    public class Shooter : MonoBehaviour
    {
        [Header("Ammo")]
        [SerializeField] private Magazine magazine;

        [Header("Aim")]
        [SerializeField] private Aim aim;
        [SerializeField] private bool skipShootIfZeroDirection;

        [Space]
        [Header("Stats")]
        [SerializeField] private FloatReference projectilesPerAttack;
        [Tooltip("The distance between projectiles if there is more than one projectile")]
        [SerializeField] private FloatReference projectileSpread;
        [Tooltip("When fired, the projectiles will be distributed around the circle at equal intervals in degrees." +
                 " The first projectile will be aimed straight in the direction. The Spread stat no longer has any effect.")]
        [SerializeField] private bool roundShoot;
        [Tooltip("The direction of the shot will always be random. The Spread stat still has an effect.")]
        [SerializeField] private bool randomAimDirection;
        [Space]
        [SerializeField] private GameEvent onShoot;

        private Transform Transform
        {
            get
            {
                if (_transform != null) return _transform;
                _transform = transform;
                return _transform;
            }
        }
        private Aim Aim
        {
            get
            {
                if(aim != null) return aim;
                aim = GetComponent<Aim>();
                return aim;
            }
        }
        private Magazine Magazine
        {
            get
            {
                if(magazine != null) return magazine;
                magazine = GetComponent<Magazine>();
                return magazine;
            }
        }
        
        private Vector2 _shootDirection;
        private Transform _transform;

        public void Shoot(int projectileNumber)
        {
            _shootDirection = randomAimDirection ? Random.onUnitSphere : Aim.GetDirection();
            if (_shootDirection == Vector2.zero)
            {
                if (skipShootIfZeroDirection)
                {
                    return;
                }
            }
            var projSpread = roundShoot ? 360f : projectileSpread.Value;
            var fireAngle = projSpread * (projectileNumber - 1);
            var halfFireAngleRad = fireAngle * 0.5f * Mathf.Deg2Rad;
            var leftDirection = MathFirearm.Rotate(_shootDirection, -halfFireAngleRad);
            var actualShotDirection = leftDirection;
            var launchAngle = roundShoot ? 360f / projectileNumber : projectileSpread;
            launchAngle *= Mathf.Deg2Rad;

            for (int i = 0; i < projectileNumber; i++)
            {
                var projectile = Magazine.Get();
                if (projectile == null)
                {
                    throw new NullReferenceException("cannot get any projectile");
                }
                projectile.transform.SetPositionAndRotation(Transform.position, Transform.rotation);
                projectile.Trail.Clear();
                projectile.SetDirection(actualShotDirection);
                actualShotDirection = MathFirearm.Rotate(actualShotDirection, launchAngle);
                onShoot.Raise();
            }
        }

        public void Shoot()
        {
            Shoot(projectilesPerAttack);
        }
    }
}