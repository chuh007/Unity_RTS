using Code.CoreSystem;
using Code.Units.Combat;
using ObjectPool.RunTime;
using UnityEngine;

namespace Code.GameEvents
{
    public struct ProjectileLaunchEvent : IEvent
    {
        public PoolingItemSO Item { get; }
        public Vector3 StartPosition { get; }
        public Vector3 EndPosition { get; }
        public IDamageable Target { get; }
        public float Speed { get; }
        public int Damage { get; }
        
        public ProjectileLaunchEvent(PoolingItemSO item, Vector3 startPosition, Vector3 endPosition, IDamageable target, float speed, int damage)
        {
            Item = item;
            StartPosition = startPosition;
            EndPosition = endPosition;
            Target = target;
            Speed = speed;
            Damage = damage;
        }
    }
}