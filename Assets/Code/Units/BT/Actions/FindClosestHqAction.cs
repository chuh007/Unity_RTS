using System;
using System.Collections.Generic;
using Code.Units.Data;
using Code.Util;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Find closest HQ", story: "[Unit] find closest [HQ]", category: "Action/Units", id: "d4fc36fa923587247f65a31bff76784a")]
    public partial class FindClosestHqAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<GameObject> HQ;

        [SerializeReference] public BlackboardVariable<float> SearchRadius = new(10);
        [SerializeReference] public BlackboardVariable<BuildingSO> TargetBuildingType;

        private Collider[] _results = new Collider[20]; //최대 20개까지 검사한다.
        
        protected override Status OnStart()
        {
            //이걸 이렇게 하는 이유는 안타깝게도 아직 Layermask가 블랙보드에 안들어간다.
            LayerMask buildingLayerMask = LayerMask.GetMask("Building");
            Vector3 center = Unit.Value.transform.position;
            int cnt = Physics.OverlapSphereNonAlloc(center, SearchRadius.Value, _results, buildingLayerMask);

            List<BaseBuilding> nearByHQ = new List<BaseBuilding>();

            for (int i = 0; i < cnt; i++)
            {
                if (_results[i].TryGetComponent(out BaseBuilding building)
                    && building.UnitSo.Equals(TargetBuildingType.Value))
                {
                    nearByHQ.Add(building);
                }
            }

            if (nearByHQ.Count == 0)
                return Status.Failure;
            
            nearByHQ.Sort(new ClosestBaseBuildingComparer(center));
            HQ.Value = nearByHQ[0].gameObject; //가장 가까운 HQ를 넣는다.
            
            return Status.Success;
        }
    }
}

