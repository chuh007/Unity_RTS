using Code.CoreSystem;
using ObjectPool.RunTime;
using UnityEngine;

namespace Code.GameEvents
{
    public struct PoolEffectPlayEvent : IEvent
    {
        public PoolingItemSO Item { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        
        public PoolEffectPlayEvent(PoolingItemSO item, Vector3 position, Quaternion rotation)
        {
            Item = item;
            Position = position;
            Rotation = rotation;
        }
    }
}