using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Stop unit", story: "[Unit] stop moving", category: "Action/Navigation", id: "ad55cf2d0c6a4e2165ac43aa38e1622e")]
    public partial class StopUnitAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        
        protected override Status OnStart()
        {
            if (Unit.Value == null || Unit.Value.Agent == null)
                return Status.Failure;
            
            Unit.Value.Agent.ResetPath();
            return Status.Success;
        }

    }
}

