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
    [NodeDescription(name: "Construction building", story: "[Unit] construct [BuildingSO] at [TargetLocation]", category: "Action/Building", id: "6e6a13bdbe87f2af1161f773b8d27842")]
    public partial class ConstructionBuildingAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<BuildingSO> BuildingSO;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        [SerializeReference] public BlackboardVariable<ConstructionDummy> Dummy;

        private float _startBuildTime;
        private float _targetHealth;
        
        protected override Status OnStart()
        {
            if (Unit.Value == null || BuildingSO.Value == null || BuildingSO.Value.Prefab == null
                || Dummy.Value == null)
                return Status.Failure;
            
            Dummy.Value.UpdateConstructionProgress(0);
            _startBuildTime = Time.time;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            float normalizeTime = (Time.time - _startBuildTime) / BuildingSO.Value.BuildTime;
            Dummy.Value.UpdateConstructionProgress(normalizeTime);

            if (normalizeTime >= 1)
            {
                GameObject building =
                    Object.Instantiate(BuildingSO.Value.Prefab, TargetLocation.Value, Quaternion.identity);
                Object.Destroy(Dummy.Value.gameObject);
                Dummy.Value = null;
                return Status.Success;
            }
            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}

