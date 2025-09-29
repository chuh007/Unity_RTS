using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Reset unit command override", story: "Reset [Unit] commands", category: "Action/Units", id: "449facb8299885b2160aa2b52265b218")]
    public partial class ResetUnitCommandOverrideAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;

        protected override Status OnStart()
        {
            if (Unit.Value == null)
                return Status.Failure;
            Unit.Value.SetCommandOverrides(null);
            return Status.Success;
        }
    }
}

