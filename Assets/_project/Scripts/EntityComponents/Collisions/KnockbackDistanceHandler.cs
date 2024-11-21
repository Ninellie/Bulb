using _project.Scripts.Core.Variables.References;
using UnityEngine;

namespace EntityComponents.Collisions
{
    public class KnockbackDistanceHandler : MonoBehaviour
    {
        [SerializeField] private bool useMultiplier;
        [SerializeField] private FloatReference directionMultiplier;
        [SerializeField] private bool useSeparatePosition;
        [SerializeField] private Vector2Reference position;
        [SerializeField] private bool inverseDirection;
        
        public Vector3 GetKnockBackVector(Vector3 otherPosition)
        {
            Vector3 collisionObjectPosition = useSeparatePosition ? position.Value : otherPosition;
            var direction = (transform.position - collisionObjectPosition).normalized;
            if (inverseDirection) direction = -direction;
            var force = useMultiplier ? direction * directionMultiplier : direction;
            return force;
        }
    }
}