using System;
using Code.Units.Buildings;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Start construction", story: "Set start to [ConstructionDummy]", category: "Action/Building", id: "a4ef476cfc71b0174c1081c453af6f7d")]
    public partial class StartConstructionAction : Action
    {
        [SerializeReference] public BlackboardVariable<ConstructionDummy> ConstructionDummy;

        protected override Status OnStart()
        {
            if(ConstructionDummy.Value == null)
                return Status.Failure;
            
            ConstructionDummy.Value.SetGhostVisual(false);
            ConstructionDummy.Value.ChangeConstructionStage(0);
            return Status.Running;
        }
    }
}

