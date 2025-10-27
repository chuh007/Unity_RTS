using System;
using Code.Environments;
using Code.Units.Animations;
using Code.Units.Combat;
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
        [SerializeReference] public BlackboardVariable<ParameterSO> AttackParam;
        // [SerializeReference] public BlackboardVariable<float> AttackDelay = new(1f);
        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;

        private float _enterTime;
        //private float _currentAnimationTime;
        private float _lastSwingTime;
        private UnitAnimator _animator;
        private bool _isSwing;
        
        protected override Status OnStart()
        {
            if (Supply.Value == null)
                return Status.Failure;

            _animator = Unit.Value.UnitAnimator;
            //_currentAnimationTime = 0f;
            //_animator?.SetParameter(AttackParam.Value);
            _enterTime = Time.time;
            _lastSwingTime = 0f;

            Supply.Value.BeginGather();
            SupplyType.Value = Supply.Value.SupplyData; //어떤 광물을 캐고 있는지 대입
            _isSwing = false;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Supply.Value.SupplyData.BaseGatherTime + _enterTime <= Time.time)
            {
                return Status.Success;
            }

            if (Time.time >= _lastSwingTime + AttackConfig.Value.AttackDelay && !_isSwing)
            {
                _isSwing = true;
                _animator.OnAnimationEnd += HandleSwingAnimationEnd;
                _animator.SetParameter(AttackParam.Value, true);
            }
            
            return Status.Running;
        }

        private void HandleSwingAnimationEnd()
        {
            _isSwing = false;
            _lastSwingTime = Time.time;
            _animator.SetParameter(AttackParam.Value, false);
            _animator.OnAnimationEnd -= HandleSwingAnimationEnd;
        }

        protected override void OnEnd()
        {
            _animator.OnAnimationEnd -= HandleSwingAnimationEnd;
            _animator.SetParameter(AttackParam.Value, false);
            
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

