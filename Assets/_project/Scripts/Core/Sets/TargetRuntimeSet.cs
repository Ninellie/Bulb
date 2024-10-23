using EntityComponents.UnitComponents.EnemyComponents;
using UnityEngine;

namespace Core.Sets
{
    [CreateAssetMenu(fileName = "New Target Set", menuName = "Sets/Target", order = 51)]
    public class TargetRuntimeSet : RuntimeSet<Target>
    {
        public Target GetNearestToCenterInCircle(Vector2 center, float radius)
        {
            var distanceToNearestTarget = Mathf.Infinity;
            Target nearestTarget = null;
            foreach (var target in items)
            {
                var distance = Vector2.Distance(center, target.transform.position);
                if (!(distance < radius)) continue;
                if (!(distance < distanceToNearestTarget)) continue;
                distanceToNearestTarget = distance;
                nearestTarget = target;
            }
            return nearestTarget;
        }

        public Target GetNearestToPosition(Vector2 position)
        {
            return GetNearestToCenterInCircle(position, Mathf.Infinity);
        }
    }
}