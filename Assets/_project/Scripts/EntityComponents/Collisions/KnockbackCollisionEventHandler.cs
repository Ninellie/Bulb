using UnityEngine;
using UnityEngine.Events;

namespace EntityComponents.Collisions
{
    [RequireComponent(typeof(KnockbackDistanceHandler))]
    public class KnockbackCollisionEventHandler : MonoBehaviour
    {
        [Header("Collision object")]
        [SerializeField] private string otherTag;
        [SerializeField] private UnityEvent<Vector2> onCollisionEnter2D;
        [SerializeField] private UnityEvent<Vector3> onCollisionStay2D;

        private Transform _transform;
        private KnockbackDistanceHandler _knockbackDistanceHandler;
        
        private void Awake()
        {
            if (_transform == null) _transform = transform;
            if (_knockbackDistanceHandler == null)
            {
                _knockbackDistanceHandler = GetComponent<KnockbackDistanceHandler>();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (!collision2D.collider.CompareTag(otherTag)) return;
            var distance = _knockbackDistanceHandler.GetKnockBackVector(collision2D.transform.position);
            onCollisionEnter2D.Invoke(distance);
        }

        private void OnCollisionStay2D(Collision2D collision2D)
        {
            if (!collision2D.collider.CompareTag(otherTag)) return;
            var distance = _knockbackDistanceHandler.GetKnockBackVector(collision2D.transform.position);
            onCollisionStay2D.Invoke(distance);
        }
    }
}