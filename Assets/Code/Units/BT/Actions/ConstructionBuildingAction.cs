using System;
using Code.Units.Buildings;
using Code.Units.Data;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Object = UnityEngine.Object;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Construction building", story: "[Unit] construct [BuildingSO] at [TargetLocation]", category: "Action/Building", id: "06dcad7d31d2d91151bd414c3d66ea2e")]
    public partial class ConstructionBuildingAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<BuildingSO> BuildingSO;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        [SerializeReference] public BlackboardVariable<ConstructionDummy> Dummy;

        private float _startBuildTime;
        private float _targetHealth;
        //private Vector3 _finalPosition;
        
        protected override Status OnStart()
        {
            if (Unit.Value == null || BuildingSO.Value == null || BuildingSO.Value.Prefab == null
                || Dummy.Value == null)
                return Status.Failure;
            
            //Dummy.Value.UpdateConstructionProgress(0);
            _startBuildTime = Dummy.Value.ProgressData.StartTime;
            //_finalPosition = TargetLocation.Value;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            float normalizeTime = (Time.time - _startBuildTime) / BuildingSO.Value.BuildTime;
            Dummy.Value.UpdateConstructionProgress(normalizeTime);

            _targetHealth += Time.deltaTime * (BuildingSO.Value.Health / BuildingSO.Value.BuildTime);
            if (_targetHealth >= 1)
            {
                int healAmount = Mathf.FloorToInt(_targetHealth);
                Dummy.Value.Heal(healAmount);
                _targetHealth -= healAmount; //소수점만 남는다.
            }
            
            if (normalizeTime >= 1)
            {
                Vector3 position = Dummy.Value.transform.position;
                GameObject newBuilding =
                    Object.Instantiate(BuildingSO.Value.Prefab, position, Quaternion.identity);
                
                BaseBuilding building = newBuilding.GetComponent<BaseBuilding>();
                building.Owner = Dummy.Value.Owner;
                if(Dummy.Value.IsSelected) //더미를 선택한 상태였다면 새로운 빌딩도 선택상태로 전환해서 생성
                    building.Select();

                
                Dummy.Value.ConstructionComplete();
                Object.Destroy(Dummy.Value.gameObject);
                Dummy.Value = null;
            }
            
            return normalizeTime >= 1 ? Status.Success : Status.Running;
        }
    }
}

