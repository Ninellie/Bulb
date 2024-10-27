using System.Collections;
using Core.Sets;
using Core.Variables.References;
using UnityEngine;
using UnityEngine.Events;

namespace EntityComponents.UnitComponents.PlayerComponents
{
    public class TurretShootDelaySetter : MonoBehaviour
    {
        [field:SerializeField] public bool Suspended { get; set; }
        
        [SerializeField] private FloatReference turretAttackSpeed;
        [SerializeField] private ShooterRuntimeSet turretShooters;
        [SerializeField] private float timeBetweenShoots;
        [SerializeField] private UnityEvent onShoot;

        private void OnEnable()
        {
            Shoot();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void ReShoot()
        {
            StopAllCoroutines();
            StartCoroutine(Co_Shoot());
        }

        public void Shoot()
        {
            StartCoroutine(Co_Shoot());
        }

        private IEnumerator Co_Shoot()
        {
            foreach (var shooter in turretShooters.items)
            {
                if (Suspended)
                {
                    yield return new WaitWhile(() => Suspended);
                }
                timeBetweenShoots = 1f / turretAttackSpeed / turretShooters.items.Count;
                yield return new WaitForSeconds(timeBetweenShoots);
                shooter.Shoot();
            }
        }
    }
}