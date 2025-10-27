using System;
using Unity.Behavior;
using UnityEngine;

namespace Code.Units.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Is not dead unit", story: "[TargetGameObject] is not dead unit", category: "Conditions", id: "860f8f877240e772eb77b246bf9c3c1f")]
    public partial class IsNotDeadUnitCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;

        public override bool IsTrue()
        {
            if(TargetGameObject.Value == null ||
               !TargetGameObject.Value.TryGetComponent(out AbstractCommandable commandable))
                return false;
        
            return !commandable.IsDead;
        }
    }
}
