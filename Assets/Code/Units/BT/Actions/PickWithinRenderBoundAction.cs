using System;
using Code.Units.Buildings;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Pick within render bound", story: "Set [TargetLocation] to a random within [ConstructionDummy]", category: "Action/Navigation", id: "8de3752a0d07919d2dc7c64a2b690ddb")]
    public partial class PickWithinRenderBoundAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<ConstructionDummy> ConstructionDummy;

        protected override Status OnStart()
        {
            if (ConstructionDummy.Value == null || ConstructionDummy.Value.MainRenderer == null) 
                return Status.Failure;

            Renderer mainRenderer = ConstructionDummy.Value.MainRenderer;

            Bounds bounds = mainRenderer.bounds;
            Vector3 extents = bounds.extents; //절반크기

            int edgeAxis = Random.Range(0, 2); //0은 x축, 1은 y축
            float x = 0, z = 0;
            if (edgeAxis == 0)
            {
                x = Random.value < 0.5f ? -extents.x : extents.x;
                z = Random.Range(-extents.z, extents.z);
            }
            else
            {
                x = Random.Range(-extents.x, extents.x);
                z = Random.value < 0.5f ? -extents.z : extents.z;
            }

            TargetLocation.Value = new Vector3(bounds.center.x + x, TargetLocation.Value.y, bounds.center.z + z);
            
            return Status.Success;
        }
    }
}

