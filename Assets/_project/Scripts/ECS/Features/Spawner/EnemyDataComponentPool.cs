using UnityEngine;

namespace _project.Scripts.ECS.Features.Spawner
{
    public class EnemyDataComponentPool : ComponentPool<EnemyDataProvider>
    {
        public EnemyDataComponentPool(bool collectionCheck, uint maxSize, uint size, GameObject root,
            GameObject itemPrefab) : base(collectionCheck, maxSize, size, root, itemPrefab)
        {
        }
    }
}