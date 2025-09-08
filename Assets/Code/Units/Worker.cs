using Code.CoreSystem;
using Code.Environments;
using Code.GameEvents;
using Code.Units.BT;
using Code.Units.BT.Events;
using Unity.Behavior;
using UnityEngine;

namespace Code.Units
{
    public class Worker : AbstractUnit
    {
        [SerializeField] private GameObject backpackObject;
        
        protected override void Start()
        {
            base.Start();
            if (GetVariableValue<SupplyEventChannel>(BTVariables.SupplyEventChannel,
                    out var supplyChannel))
            {
                supplyChannel.Value.Event += HandleSupplyEvent;
            }

            if (GetVariableValue<int>(BTVariables.SupplyAmountHeld, out var supplyAmountHeld))
            {
                supplyAmountHeld.OnValueChanged += () => SetActiveBackpack(supplyAmountHeld.Value > 0);
            }
        }

        private void SetActiveBackpack(bool isActive) => backpackObject.SetActive(isActive);

        private void HandleSupplyEvent(AbstractUnit unit, int amount, SupplySO supplyData)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(amount, supplyData));
        }

        public void Gather(GatherableSupply gatherable)
        {
            SetVariableValue(BTVariables.GatherableSupply, gatherable);
            SetVariableValue(BTVariables.TargetGameObject, gatherable.gameObject);
            SetVariableValue(BTVariables.Command, UnitCommands.Gather);
        }
    }
}