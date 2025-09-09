using Code.CoreSystem;
using Code.Environments;
using Code.GameEvents;
using Code.Units.BT;
using Code.Units.BT.Events;
using Code.Units.Buildings;
using Code.Units.Data;
using Unity.Behavior;
using UnityEngine;

namespace Code.Units
{
    public class Worker : AbstractUnit, IBuildingConstructor
    {
        [SerializeField] private GameObject backpackObject;
        
        
        public bool HasSupplies => backpackObject.activeSelf;
        public bool IsBuilding { get; private set; }
        
        protected override void Start()
        {
            base.Start();
            if (GetVariable<SupplyEventChannel>(BTVariables.SupplyEventChannel,
                    out var supplyChannel))
            {
                supplyChannel.Value.Event += HandleSupplyEvent;
            }

            if (GetVariable<int>(BTVariables.SupplyAmountHeld, out var supplyAmountHeld))
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

        public void ReturnSupplies(GameObject targetHQ)
        {
            SetVariableValue(BTVariables.HeadQuarter, targetHQ);
            SetVariableValue(BTVariables.Command, UnitCommands.ReturnSupplies);
        }

        public GameObject Build(BuildingSO buildingData, Vector3 targetLocation)
        {
            GameObject instance = Instantiate(buildingData.ConstructionDummy, targetLocation, Quaternion.identity);
            if (!instance.TryGetComponent(out ConstructionDummy dummy))
            {
                Debug.LogError($"Missing construction dummy script on {instance.name}");
            }
            
            dummy.SetGhostVisual(true);
            SetVariableValue(BTVariables.ConstructBuildingSO, buildingData);
            SetVariableValue(BTVariables.TargetLocation, targetLocation);
            SetVariableValue(BTVariables.ConstructionDummy, dummy);
            SetVariableValue(BTVariables.Command, UnitCommands.ConstructBuilding);
            
            return instance;
        }

        public void CancelBuilding()
        {
            
        }

        public void ResumeBuilding(BaseBuilding building)
        {
            
        }
    }
}