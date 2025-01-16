using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Aiming
{
    public sealed class AimingProvider : MonoProvider<Aiming>
    {
        private void OnDrawGizmos()
        {
            ref var aiming = ref cachedEntity.GetComponent<Aiming>();
            var radius = aiming.AimingRadius;
            var selfPos = aiming.SelfTransform.position;
            
           Gizmos.color = Color.green;
           Gizmos.DrawWireSphere(selfPos, radius);
           
            ref var aimed = ref cachedEntity.GetComponent<Aimed>(out var exist);
            if (!exist) return;
            if (aimed.Target is null) return;
            var targetPos = aimed.Target.position;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(selfPos, targetPos);
            
        }
    }
}