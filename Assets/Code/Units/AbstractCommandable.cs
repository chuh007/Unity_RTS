using System;
using Code.Commands;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units.Combat;
using Code.Units.Data;
using Code.Util;
using ObjectPool.RunTime;
using UnityEngine;

namespace Code.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable, IDamageable
    {
        [SerializeField] protected DecalProjector decalProjector;
        [SerializeField] protected PoolingItemSO impactEffect;
        
        [field: SerializeField] public int CurrentHealth { get; protected set; }
        public Transform Transform => gameObject == null ? null : transform;

        [field: SerializeField] public int MaxHealth { get; protected set; }
        [field: SerializeField] public AbstractUnitSO UnitSo { get; private set; } 
        [field: SerializeField] public BaseCommandSO[] AvailableCommands { get; private set; }
        
        
        [field: SerializeField] public Owner Owner { get; set; }
        public bool IsSelected { get; protected set; }
        public bool IsDead { get; protected set; }
        
        private BaseCommandSO[] _initialCommands;
        private Collider _collider;

        public delegate void HealthUpdatedEvent(AbstractCommandable commandable, int lastHealth, int newHealth);
        public event HealthUpdatedEvent OnHealthUpdated;

        protected virtual void Awake()
        {
            Debug.Assert(UnitSo != null, $"UnitSo is not assigned in {gameObject.name}");
            Debug.Assert(decalProjector != null, $"Decal projector is not assigned {gameObject.name}");
            decalProjector.SetActiveDecal(false);
            _initialCommands = AvailableCommands; //초기 명령어 셋을 저장하기.
            _collider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {
            CurrentHealth = MaxHealth = UnitSo.Health;
            OnHealthUpdated?.Invoke(this, 0, CurrentHealth);
        }

        protected virtual void OnDestroy()
        {
            DeSelect();
        }

        public void Select()
        {
            IsSelected = true;
            decalProjector.SetActiveDecal(true);
            Bus<UnitSelectEvent>.Raise(Owner, new UnitSelectEvent(this));
        }

        public virtual void DeSelect()
        {
            IsSelected = false;
            decalProjector.SetActiveDecal(false);
            SetCommandOverrides(null);
            Bus<UnitDeselectEvent>.Raise(Owner, new UnitDeselectEvent(this));
        }

        public void SetCommandOverrides(BaseCommandSO[] commands)
        {
            if (commands == null || commands.Length == 0)
            {
                AvailableCommands = _initialCommands;
            }
            else
            {
                AvailableCommands = commands;
            }

            if (IsSelected)  //선택된 유닛이 커맨드가 변경되었다면 UI갱신을 위해서 다시 선택되었음을 알려준다.
            {
                Bus<UnitSelectEvent>.Raise(Owner, new UnitSelectEvent(this));
            }
        }

        public void Heal(int amount)
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            OnHealthUpdated?.Invoke(this, lastHealth, CurrentHealth);
        }
        
        public void TakeDamage(int damage, Vector3 position, Vector3 normal)
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
            
            OnHealthUpdated?.Invoke(this, lastHealth, CurrentHealth);

            if (impactEffect != null)
            {
                Vector3 colliderPosition = DistanceUtil.GetTargetPosition(_collider, 
                    position + normal * 2f);
                Quaternion rotation = Quaternion.LookRotation(normal);
                Bus<PoolEffectPlayEvent>.Raise(Owner,
                    new PoolEffectPlayEvent(impactEffect, colliderPosition, rotation));
            }

            if (CurrentHealth <= 0 && !IsDead)
            {
                IsDead = true;
                Die();
            }
        }

        public virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}