using EntityComponents.UnitComponents.EnemyComponents;
using UnityEngine;

namespace EntityComponents.UnitComponents.PlayerComponents
{
    /// <summary>
    /// This class must be attached to root enemy's gameobject
    /// </summary>
    public class EnemyData : MonoBehaviour
    {
        [SerializeField] private Transform enemyTransform;
        [SerializeField] private Rigidbody2D enemyRigidbody2D;
        [SerializeField] private SpriteType enemySpriteType;
        
        public Transform EnemyTransform => enemyTransform;
        public Rigidbody2D EnemyRigidbody2D => enemyRigidbody2D;
        public SpriteType EnemySpriteType => enemySpriteType;
        
        private void Awake() => GetDependencies();

        private void OnValidate() => GetDependencies();

        private void GetDependencies()
        {
            if (enemyTransform == null) enemyTransform = GetComponentTypeInChildren<Transform>();
            if (enemyRigidbody2D == null) enemyRigidbody2D = GetComponentTypeInChildren<Rigidbody2D>();
            if (enemySpriteType == null) enemySpriteType = GetComponentTypeInChildren<SpriteType>();
        }
        
        private T GetComponentTypeInChildren<T>()
        {
            if (!TryGetComponent(out T componentType))
            {
                componentType = GetComponentInChildren<T>();
            }
            
            return componentType;
        }
    }
}