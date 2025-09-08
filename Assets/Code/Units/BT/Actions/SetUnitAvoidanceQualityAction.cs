using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set unit avoidance quality", story: "Set [Unit] avoidance quality to [NewValue]", category: "Action/Navigation", id: "cae967dd096c4cdee2592d7e213a2b40")]
    public partial class SetUnitAvoidanceQualityAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<int> NewValue;

        protected override Status OnStart()
        {
            if (NewValue > 4 || NewValue < 0 || Unit.Value == null || Unit.Value.Agent == null)
                return Status.Failure;

            Unit.Value.Agent.obstacleAvoidanceType = (ObstacleAvoidanceType) NewValue.Value;
            
            return Status.Success;
        }
    }
}

