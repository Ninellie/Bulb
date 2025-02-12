﻿using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.ECS.Pool
{
    [CreateAssetMenu(menuName = "ECS/Pool/" + nameof(ComponentPoolContainer))]
    public class ComponentPoolContainer : ScriptableObject
    {
        private readonly List<object> _pools = new();
        
        public ComponentPool<T> GetPool<T>() where T : Component
        {
            foreach (var pool in _pools)
            {
                if (pool is ComponentPool<T> componentPool)
                    return componentPool;
            }
            return null;
        }
        
        public ComponentPool<T> CreatePool<T>(string poolName, bool collectionCheck, uint maxSize, uint size, GameObject itemPrefab)
            where T : Component
        {
            var root = new GameObject(poolName)
            {
                transform =
                {
                    position = Vector3.zero
                }
            };
            var pool = AddPool<T>(collectionCheck, maxSize, size, root, itemPrefab);
            
            _pools.Add(pool);
            
            return pool;
        }

        private ComponentPool<T> AddPool<T>(bool collectionCheck, uint maxSize, uint size, GameObject root, GameObject itemPrefab)
            where T : Component
        {
            var pool = new ComponentPool<T>(collectionCheck, maxSize, size, root, itemPrefab);
            
            _pools.Add(pool);
            pool.Init();
            return pool;
        }
    }
}