using System;
using Code.Units.Buildings;
using Unity.Behavior;
using UnityEngine;

namespace Code.Units.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Building is in Progress", story: "[ConstructionDummy] is being built", category: "Conditions", id: "3492cce037685764998e762468d77afa")]
    public partial class BuildingIsInProgressCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<ConstructionDummy> ConstructionDummy;

        public override bool IsTrue()
        {
            return ConstructionDummy.Value != null
                   && ConstructionDummy.Value.ProgressData.State == BuildingState.Constructing;
        }
    }
}
