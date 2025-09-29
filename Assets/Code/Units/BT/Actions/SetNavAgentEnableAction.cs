using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set NavAgent enable", story: "[Unit] set navAgent active status to [IsActive]", category: "Action/Navigation", id: "4da78324add9afe11acc8bb677ef3d03")]
    public partial class SetNavAgentEnableAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<bool> IsActive;

        [SerializeReference] public BlackboardVariable<bool> IsWarpTo = new(false);
    
        protected override Status OnStart()
        {
            if(Unit.Value == null || Unit.Value.Agent == null)
                return Status.Failure;
        
            Unit.Value.Agent.enabled = IsActive.Value;

            if (IsActive.Value && IsWarpTo.Value)
            {
                Unit.Value.Agent.Warp(Unit.Value.transform.position);
            }
        
            return Status.Success;
        }
    }
}

