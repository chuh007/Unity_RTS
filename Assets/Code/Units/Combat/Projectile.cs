using System;
using ObjectPool.RunTime;
using UnityEngine;

namespace Code.Units.Combat
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolingItemSO PoolType { get; private set; }
        public GameObject GameObject => gameObject;

        private Pool _myPool;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private IDamageable _targetDamageable;
        private float _duration;
        private float _launchTime;
        private int _damage;

        public void LaunchProjectile(Vector3 startPosition, Vector3 endPosition, IDamageable targetDamageable,
            float speed, int damage)
        {
            Vector3 distance = endPosition - startPosition;
            Quaternion rotation = Quaternion.LookRotation(distance.normalized);
            transform.SetPositionAndRotation(startPosition, rotation);

            _duration = distance.magnitude / speed;
            _launchTime = Time.time;

            _startPosition = startPosition;
            _endPosition = endPosition;
            _targetDamageable = targetDamageable;
            _damage = damage;
        }

        private void Update()
        {
            float deltaTime = Time.time - _launchTime;
            float lerpTime = deltaTime / _duration;
            
            transform.position = Vector3.Lerp(_startPosition, _endPosition, lerpTime);
            if (lerpTime >= 1)
            {
                if (_targetDamageable != null && _targetDamageable.Transform != null)
                {
                    _targetDamageable.TakeDamage(_damage, _endPosition, transform.forward * -1);   
                }
                _myPool.Push(this);
            }
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        public void ResetItem() { }
    }
}