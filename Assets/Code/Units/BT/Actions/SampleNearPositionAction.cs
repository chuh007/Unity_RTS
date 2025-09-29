using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Sample near position", story: "Set [TargetLocation] to the closest point on navmesh from [Unit]", category: "Action", id: "06458043b58171dc636b8095b50ca8e3")]
    public partial class SampleNearPositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        
        [SerializeReference] public BlackboardVariable<float> Radius = new(5f);
        
        protected override Status OnStart()
        {
            if (Unit.Value == null || Unit.Value.Agent == null)
                return Status.Failure;

            NavMeshAgent agent = Unit.Value.Agent;

            NavMeshQueryFilter queryFilter = new NavMeshQueryFilter
            {
                agentTypeID = agent.agentTypeID,
                areaMask = agent.areaMask
            };

            if (NavMesh.SamplePosition(Unit.Value.transform.position, out NavMeshHit navHit, Radius.Value, queryFilter))
            {
                TargetLocation.Value = navHit.position;
                return Status.Success;
            }
            
            return Status.Failure;
        }
    }
}

