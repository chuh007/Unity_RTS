using Code.Commands;
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
    public class Worker : AbstractCombatUnit, IBuildingConstructor
    {
        [SerializeField] private GameObject backpackObject;
        [SerializeField] private BaseCommandSO[] cancelConstructionCommandSet;

        public bool HasSupplies => backpackObject.activeSelf;

        public bool IsBuilding => GetVariable<UnitCommands>(BTVariables.Command, out var commandVar)
                                  && commandVar.Value == UnitCommands.ConstructBuilding;
        
        protected override void Start()
        {
            base.Start();
            if (GetVariable<SupplyEventChannel>(BTVariables.SupplyEventChannel, out var supplyChannel))
            {
                supplyChannel.Value.Event += HandleSupplyEvent;
            }
            
            if(GetVariable<int>(BTVariables.SupplyAmountHeld, out var supplyAmountHeld))
            {
                supplyAmountHeld.OnValueChanged += () => SetActiveBackpack(supplyAmountHeld.Value > 0);
            }

            if (GetVariable(BTVariables.ConstructionEventChannel,
                    out BlackboardVariable<ConstructionEventChannel> constructEvtChannel))
            {
                constructEvtChannel.Value.Event += HandleConstructionEvent;
            }
        }

        private void HandleConstructionEvent(AbstractUnit unit, ConstructionEventType evtType, ConstructionDummy dummy)
        {
            switch (evtType)
            {
                case ConstructionEventType.ArrivedAt:
                    if (dummy != null && (dummy.ProgressData.State == BuildingState.Constructing 
                                          || dummy.ProgressData.State == BuildingState.Completed ))
                    {
                        //현재 건축상태인 곳에 도달했으면 할일이 없으니까 break해준다.
                        Stop();
                        break;
                    }
                    SetCommandOverrides(cancelConstructionCommandSet); //건축상태가 아니면 건축 시작한다.
                    break;
                case ConstructionEventType.Begin:
                    SetCommandOverrides(cancelConstructionCommandSet);
                    break;
                case ConstructionEventType.Cancel:
                case ConstructionEventType.Abort:
                case ConstructionEventType.Completed:
                    SetCommandOverrides(null);
                    break;
            }
        }

        private void SetActiveBackpack(bool isActive) => backpackObject.SetActive(isActive);

        private void HandleSupplyEvent(AbstractUnit unit, int amount, SupplySO supplyData)
        {
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, amount, supplyData));
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

        
        public GameObject ConstructBuilding(BuildingSO buildingData, Vector3 targetLocation)
        {
            GameObject instance = Instantiate(buildingData.ConstructionDummy, targetLocation, Quaternion.identity);

            if (!instance.TryGetComponent(out ConstructionDummy dummy))
            {
                Debug.LogError($"Missing construction dummy script on {instance.name}");
            }
            
            dummy.StartPlacementGhost(this); //더미는 고스트 상태로 전환된다.
            //기타 내용은 블랙보드에서 처리한다.
            //여기다가 알잘딱 변수들을 Set해서 건설이 시작되게 만들어보세요.
            SetVariableValue(BTVariables.ConstructBuildingSO, buildingData);
            SetVariableValue(BTVariables.TargetLocation, targetLocation);
            SetVariableValue(BTVariables.ConstructionDummy, dummy);
            SetVariableValue(BTVariables.Command, UnitCommands.ConstructBuilding);
            
            SetCommandOverrides(cancelConstructionCommandSet);
            
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, -buildingData.Cost.Minerals, buildingData.Cost.MineralSO));
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, -buildingData.Cost.Gas, buildingData.Cost.GasSO));
            
            return instance; //만들어진 더미를 리턴한다.
        }

        public void CancelConstruction()
        {
            if (GetVariable<ConstructionDummy>(BTVariables.ConstructionDummy, out var dummy))
            {
                Destroy(dummy.Value.gameObject);
                BuildingSO buildingToCancel = dummy.Value.BuildingSo;
                Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, Mathf.FloorToInt( buildingToCancel.Cost.Minerals * 0.8f),
                    buildingToCancel.Cost.MineralSO));
                
                Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, Mathf.FloorToInt(buildingToCancel.Cost.Gas * 0.8f),
                    buildingToCancel.Cost.GasSO));
                dummy.Value = null;
            }
            
            SetCommandOverrides(null); //원래 커맨드로 변경해준다.
            Stop();
        }

        public void ResumeConstruction(ConstructionDummy dummy)
        {
            SetVariableValue(BTVariables.ConstructBuildingSO, dummy.BuildingSo);
            SetVariableValue(BTVariables.TargetLocation, dummy.transform.position);
            SetVariableValue(BTVariables.ConstructionDummy, dummy);
            SetVariableValue(BTVariables.Command, UnitCommands.ConstructBuilding);
            
            //타겟지역에 도착해서 실질적 건설을 시작해야 Cancel로 변경하는거다.
            //SetCommandOverrides(cancelConstructionCommandSet);
        }

        public override void DeSelect()
        {
            decalProjector.SetActiveDecal(false);
            IsSelected = false;
            if (IsBuilding == false)
            {
                SetCommandOverrides(null);
            }
            
            Bus<UnitDeselectEvent>.Raise(Owner, new UnitDeselectEvent(this));
        }
    }
}