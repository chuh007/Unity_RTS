using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool.RunTime
{
    [CreateAssetMenu(fileName = "PoolManager", menuName = "SO/Pool/Manager", order = 0)]
    public class PoolManagerSO : ScriptableObject
    {
        public List<PoolingItemSO> itemList = new ();
        
        private Dictionary<PoolingItemSO, Pool> _pools;
        private Transform _rootTrm;

        public void InitializePool(Transform root) {
            _rootTrm = root;
            _pools = new Dictionary<PoolingItemSO, Pool>();

            foreach(var item in itemList)
            {
                IPoolable poolable = item.prefab.GetComponent<IPoolable>();
                Debug.Assert(poolable != null, $"PoolItem does not have IPoolable {item.prefab.name}");
            
                var pool = new Pool(poolable, _rootTrm, item.initCount);

                _pools.Add(item, pool);
            }
        }

        public IPoolable Pop(PoolingItemSO type)
        {
            if(_pools.TryGetValue(type, out Pool pool))
            {
                return pool.Pop();
            }
            return null;
        }

        public void Push(IPoolable item)
        {
            if(_pools.TryGetValue(item.PoolType, out Pool pool))
            {
                pool.Push(item);
            }
        }
    }
}