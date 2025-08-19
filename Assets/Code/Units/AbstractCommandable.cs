using Code.CoreSystem;
using Code.GameEvents;
using Code.Units.Data;
using UnityEngine;

namespace Code.Units
{
    public class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [SerializeField] protected DecalProjector decalProjector;
        [field: SerializeField] public int CurrentHealth { get; private set; }
        [field: SerializeField] public int MaxHealth { get; private set; }
        [field: SerializeField] public UnitSO UnitSo { get; private set; }
        
        public bool IsSelected { get; private set; }

        protected virtual void Awake()
        {
            decalProjector.SetActiveDecal(false);
        }

        protected virtual void Start()
        {
            CurrentHealth = MaxHealth = UnitSo.Health;
        }

        protected virtual void OnDestroy()
        {
            
        }
        
        public void Select()
        {
            IsSelected = true;
            decalProjector?.SetActiveDecal(true);
            Bus<UnitSelectEvent>.Raise(new UnitSelectEvent(this));
        }

        public void Deselect()
        {
            IsSelected = false;
            decalProjector?.SetActiveDecal(false);
            Bus<UnitDeselectEvent>.Raise(new UnitDeselectEvent(this));
        }
    }
}