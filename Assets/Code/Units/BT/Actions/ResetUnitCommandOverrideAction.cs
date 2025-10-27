using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Reset unit command override", story: "Reset [Unit] commands", category: "Action/Units", id: "e260b5508202d54dc13a92e6adbd1928")]
    public partial class ResetUnitCommandOverrideAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractCommandable> Unit;

        protected override Status OnStart()
        {
            if (Unit.Value == null)
                return Status.Failure;
            
            Unit.Value.SetCommandOverrides(null);
            return Status.Success;
        }
    }
}

