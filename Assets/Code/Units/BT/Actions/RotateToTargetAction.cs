using Code.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Rotate to target", story: "[Unit] rotate to [Target]", category: "Action", id: "1e47ef2730b814abb1bab7c3da8209ce")]
public partial class RotateToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        if(Unit.Value == null || Target.Value == null)
            return Status.Failure;
        
        Vector3 unitPosition = Unit.Value.transform.position;
        Vector3 targetPosition = Target.Value.transform.position;

        Unit.Value.transform.forward = (targetPosition - unitPosition).normalized;
        return Status.Success;
    }
}

