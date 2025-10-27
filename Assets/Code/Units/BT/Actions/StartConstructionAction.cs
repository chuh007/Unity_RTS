using System;
using Code.Units.Buildings;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Start construction", story: "[Unit] start to [ConstructionDummy]", category: "Action/Building", id: "11cf20c199e338f8b47f982db698d387")]
    public partial class StartConstructionAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<ConstructionDummy> ConstructionDummy;

        protected override Status OnStart()
        {
            if (ConstructionDummy.Value == null || !(Unit.Value is IBuildingConstructor constructor))
                return Status.Failure;
            
            ConstructionDummy.Value.StartConstruction(constructor);
            //ConstructionDummy.Value.ChangeConstructionStage(0);
            return Status.Success;
        }
    }
}

