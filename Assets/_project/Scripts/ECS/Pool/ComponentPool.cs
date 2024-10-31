using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace _project.Scripts.ECS.Pool
{
    public class ComponentPool<T> where T : Component
    {
        public int CountActive => _pool.CountActive;
        public int CountAll => _pool.CountAll;
        public int CountInactive => _pool.CountInactive;
        
        private readonly GameObject _itemPrefab;
        private readonly GameObject _root;
        private readonly uint _size;
        private readonly uint _maxSize;
        private ObjectPool<T> _pool;
        private readonly bool _collectionCheck;
        private Transform _transform;

        public ComponentPool(bool collectionCheck, uint maxSize, uint size, GameObject root, GameObject itemPrefab)
        {
            _collectionCheck = collectionCheck;
            _maxSize = maxSize;
            _size = size;
            _root = root;
            _itemPrefab = itemPrefab;
        }

        public void Init()
        {
            _transform = _root.transform;
            _pool = new ObjectPool<T>(
                CreateItem,
                OnGetPool,
                OnReleasePool,
                OnDestroy,
                _collectionCheck, (int)_size, (int)_maxSize);
        }

        public T Get()
        {
            return _pool.Get();
        }

        public void Release(T item)
        {
            _pool.Release(item);
        }

        public void Release(GameObject item)
        {
            _pool.Release(item.GetComponent<T>());
        }

        private T CreateItem()
        {
            return Object.Instantiate(_itemPrefab, _transform).GetComponent<T>();
        }

        private static void OnDestroy(T item)
        {
            Object.Destroy(item);
        }

        private static void OnGetPool(T item)
        {
            item.gameObject.SetActive(true);
        }

        private static void OnReleasePool(T item)
        {
            item.gameObject.SetActive(false);
        }
    }
}