using System;
using System.Collections.Generic;
using System.Linq;
using Code.CoreSystem;
using Code.GameEvents;
using UnityEngine;

namespace Code.Units.Combat
{
    [RequireComponent(typeof(SphereCollider))]
    public class DamageableSensor : MonoBehaviour
    {
        public delegate void UnitDetectEvent(IDamageable damageable);

        public event UnitDetectEvent OnUnitEnter;
        public event UnitDetectEvent OnUnitExit;

        private SphereCollider _collider;
        private HashSet<IDamageable> _damageables = new HashSet<IDamageable>();
        public List<IDamageable> Damageables => _damageables.ToList();
        
        [field: SerializeField] public Owner Owner { get; private set; }
        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
        }

        public void SetUpFrom(Owner owner, AttackConfigSO attackConfig)
        {
            Owner = owner;
            _collider.radius = attackConfig.DetectRange;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable) && damageable.Owner != Owner)
            {
                _damageables.Add(damageable);
                OnUnitEnter?.Invoke(damageable);
            }

            if (_damageables.Count == 1)
            {
                Bus<UnitDeathEvent>.RegisterForAll( HandleUnitDeath);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable)
                && _damageables.Remove(damageable))
            {
                OnUnitExit?.Invoke(damageable);
            }

            if (_damageables.Count == 0)
            {
                Bus<UnitDeathEvent>.UnRegisterForAll(HandleUnitDeath);
            }
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            if (_damageables.Contains(evt.Unit))
            {
                OnTriggerExit(evt.Unit.GetComponent<Collider>());
            }
        }
    }
}