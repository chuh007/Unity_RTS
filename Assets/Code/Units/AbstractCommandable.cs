using System;
using Code.Commands;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units.Data;
using UnityEngine;

namespace Code.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [SerializeField] protected DecalProjector decalProjector;
        [field: SerializeField] public int CurrentHealth { get; protected set; }
        [field: SerializeField] public int MaxHealth { get; protected set; }
        [field: SerializeField] public AbstractUnitSO UnitSo { get; private set; } 
        [field: SerializeField] public BaseCommandSO[] AvailableCommands { get; private set; }
        
        public bool IsSelected { get; protected set; }
        
        private BaseCommandSO[] _initialCommands;

        public delegate void HealthUpdatedEvent(AbstractCommandable commandable, int lastHealth, int newHealth);
        public event HealthUpdatedEvent OnHealthUpdated;
        
        protected virtual void Awake()
        {
            Debug.Assert(UnitSo != null, $"UnitSo is not assigned in {gameObject.name}");
            Debug.Assert(decalProjector != null, $"Decal projector is not assigned {gameObject.name}");
            decalProjector.SetActiveDecal(false);
            _initialCommands = AvailableCommands;
        }

        protected virtual void Start()
        {
            CurrentHealth = MaxHealth = UnitSo.Health;
            OnHealthUpdated?.Invoke(this, 0, CurrentHealth);
        }

        protected virtual void OnDestroy()
        {
        }

        public void Select()
        {
            IsSelected = true;
            decalProjector.SetActiveDecal(true);
            Bus<UnitSelectEvent>.Raise(new UnitSelectEvent(this));
        }

        public virtual void DeSelect()
        {
            IsSelected = false;
            decalProjector.SetActiveDecal(false);
            SetCommandOverrides(null);
            Bus<UnitDeselectEvent>.Raise(new UnitDeselectEvent(this));
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

            if (IsSelected)
            {
                Bus<UnitSelectEvent>.Raise(new UnitSelectEvent(this));
            }
        }

        public void Heal(int amount)
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            OnHealthUpdated?.Invoke(this, lastHealth, CurrentHealth);
        }
    }
}