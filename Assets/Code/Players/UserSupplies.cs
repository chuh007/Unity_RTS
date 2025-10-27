using System;
using System.Collections.Generic;
using Code.CoreSystem;
using Code.Environments;
using Code.GameEvents;
using Code.Units;
using UnityEngine;

namespace Code.Players
{
    public class UserSupplies : MonoBehaviour
    {
        public static UserSupplies Instance { get; private set; }
        
        [SerializeField] private SupplySO mineralSO;
        [SerializeField] private SupplySO gasSO;
        
        public Dictionary<Owner, int> Minerals { get; private set; }
        public Dictionary<Owner, int> Gas { get; private set; }

        public delegate void SupplyChanged(Owner owner, int amount, SupplySO supplyType);
        public event SupplyChanged OnSupplyChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            Minerals = new Dictionary<Owner, int>();
            Gas = new Dictionary<Owner, int>();

            foreach (Owner owner in Enum.GetValues(typeof(Owner)))
            {
                Minerals.Add(owner, 0);
                Gas.Add(owner, 0);
            }
            
            Bus<SupplyEvent>.RegisterForAll(HandleSupplyEvent);
        }

        private void OnDestroy()
        {
            Bus<SupplyEvent>.UnRegisterForAll(HandleSupplyEvent);
        }

        private void HandleSupplyEvent(SupplyEvent evt)
        {
            if (evt.SupplyData.Equals(mineralSO))
            {
                Minerals[evt.Owner] += evt.Amount;
                OnSupplyChanged?.Invoke(evt.Owner, Minerals[evt.Owner], mineralSO);
            }
            
            if(evt.SupplyData.Equals(gasSO))
            {
                Gas[evt.Owner] += evt.Amount;
                OnSupplyChanged?.Invoke(evt.Owner, Gas[evt.Owner], gasSO);
            }
        }

        public bool HasEnoughSupplies(Owner owner, SupplyCostSO cost)
            => Minerals[owner] >= cost.Minerals && Gas[owner] >= cost.Gas;
    }
}