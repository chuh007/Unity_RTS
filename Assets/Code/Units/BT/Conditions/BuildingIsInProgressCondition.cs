using System;
using Code.Units.Buildings;
using Unity.Behavior;
using UnityEngine;

namespace Code.Units.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Building is in Progress", story: "[ConstructionDummy] is being built", category: "Conditions", id: "36a94237394f9c0e5d1acaae95a5a2a2")]
    public partial class BuildingIsInProgressCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<ConstructionDummy> ConstructionDummy;
        
        public override bool IsTrue()
        {
            return true;
        }

        public override void OnStart()
        {
        }

        public override void OnEnd()
        {
        }
    }
}
