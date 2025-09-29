using System;
using Code.Units.Animations;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set UnitAnimator trigger", story: "Set [Unit] trigger [ParameterSO]", category: "Action/Animation", id: "9f57bd767c94546e68cad807574128d6")]
    public partial class SetUnitAnimatorTriggerAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<ParameterSO> ParameterSO;

        protected override Status OnStart()
        {
            if (Unit.Value == null || Unit.Value.UnitAnimator == null)
                return Status.Failure;

            Unit.Value.UnitAnimator.SetParameter(ParameterSO.Value);
            
            return Status.Success;
        }
    }
}

