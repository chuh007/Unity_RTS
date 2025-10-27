using System;
using Code.Units.Combat;
using Code.Util;
using Unity.Behavior;
using UnityEngine;

namespace Code.Units.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Is In AttackDistance", story: "[Target] is in [Unit] attack distance by [Config]", category: "Conditions", id: "a6b323cca82d179e436795ae969b0092")]
    public partial class IsInAttackDistanceCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<GameObject> Unit;
        [SerializeReference] public BlackboardVariable<AttackConfigSO> Config;
        
        public override bool IsTrue()
        {
            if (Target.Value == null) return true; //타겟 없으면 종료
            Collider targetCollider = Target.Value.GetComponent<Collider>();
            Vector3 targetPosition 
                = DistanceUtil.GetTargetPosition(Unit.Value.gameObject, targetCollider);;
            Vector3 selfPosition = Unit.Value.transform.position;
            
            return Vector3.Distance(targetPosition, selfPosition) <= Config.Value.AttackRange;
        }
        
        //잘 생각해서 컬라이더에서 가장 가까운 위치로 타겟 포지션을 가져오는 함수를 만들고
        //이를 이용해서 건물도 잘 공격하게 맹글어 봅시다.!
    }
}
