using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Rotate to target", story: "[Unit] rotate to [Target]", category: "Action/Transform", id: "be338e5687143c642f509eb0ddf5bbab")]
    public partial class RotateToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<Transform> Target;

        protected override Status OnStart()
        {
            if(Unit.Value == null || Target.Value == null)
                return Status.Failure;

            Vector3 unitPosition = Unit.Value.transform.position;
            Vector3 targetPosition = Target.Value.position;

            Unit.Value.transform.forward = (targetPosition - unitPosition).normalized;
            return Status.Success;
        }
    }
}

