using System;
using Code.Environments;
using Code.Units.Animations;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Gather supplies", story: "[Unit] gather [Amount] supplies from [Supply]", category: "Action/Units", id: "8cdef977ae1c4648ebec7f15b33669a6")]
    public partial class GatherSuppliesAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<int> Amount;
        [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
        
        [SerializeReference] public BlackboardVariable<SupplySO> SupplyType;
        [SerializeReference] public BlackboardVariable<ParameterSO> AttackParameter;
        [SerializeReference] public BlackboardVariable<float> AttackDelay = new(1f);
        
        private float _enterTime;
        private float _currentAnimationTime;
        private UnitAnimator _animator;
        
        protected override Status OnStart()
        {
            if (Supply.Value == null)
                return Status.Failure;

            _animator = Unit.Value.UnitAnimator;
            _currentAnimationTime = 0f;
            _animator?.SetParameter(AttackParameter.Value);
            _enterTime = Time.time;
            
            Supply.Value.BeginGather();
            SupplyType.Value = Supply.Value.SupplyData; //어떤 광물을 캐고 있는지 대입
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
                _animator?.SetParameter(AttackParameter.Value);
            if (Supply.Value.SupplyData.BaseGatherTime + _enterTime <= Time.time)
            {
                return Status.Success;
            }

            _currentAnimationTime += Time.deltaTime;

            if (_currentAnimationTime >= AttackDelay.Value)
            {
                _currentAnimationTime = 0f;
                float remain = Supply.Value.SupplyData.BaseGatherTime - (Time.time - _enterTime);

                if (remain > AttackDelay.Value)
                    _animator?.SetParameter(AttackParameter.Value);
            }
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (Supply.Value == null) return; //다 캔거면 할거없고
            if (CurrentStatus == Status.Success)
            {
                Amount.Value = Supply.Value.EndGather(); //캔 수량만큼 업데이트
            }
            else
            {
                Supply.Value.AbortGather(); //취소
            }
        }
    }
}

