using System;
using Code.CoreSystem;
using Code.Environments;
using Code.GameEvents;
using UnityEngine;

namespace Code.Players
{
    public class UserSupplies : MonoBehaviour
    {
        [SerializeField] private SupplySO mineralSO;
        [SerializeField] private SupplySO gasSO;
        
        [field:SerializeField] public int Minerals { get; private set; }
        [field:SerializeField] public int Gas { get; private set; }
        
        public delegate void SupplyChanged(int amount, SupplySO supplyType);
        public event SupplyChanged OnSupplyChanged;
        
        private void Awake()
        {
            Minerals = 0;
            Gas = 0;

            Bus<SupplyEvent>.OnEvent += HandleSupplyEvent;
        }

        private void OnDestroy()
        {
            Bus<SupplyEvent>.OnEvent -= HandleSupplyEvent;
        }

        private void HandleSupplyEvent(SupplyEvent evt)
        {
            if (evt.SupplyData.Equals(mineralSO))
            {
                Minerals += evt.Amount;
                OnSupplyChanged?.Invoke(Minerals, mineralSO);
            }

            if (evt.SupplyData.Equals(gasSO))
            {
                Gas += evt.Amount;
                OnSupplyChanged?.Invoke(Gas, gasSO);
            }
        }
    }
}