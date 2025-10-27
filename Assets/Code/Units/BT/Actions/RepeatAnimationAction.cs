using System;
using Code.Units.Animations;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Repeat animation", story: "[Unit] repeat [Param] [Count] times", category: "Action/Animation", id: "9de63182e718d995a51d418ad037e82c")]
    public partial class RepeatAnimationAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<ParameterSO> Param;
        [SerializeReference] public BlackboardVariable<int> Count;
        [SerializeReference] public BlackboardVariable<float> IntervalDelay = new(0.4f);

        private UnitAnimator _animator;
        private bool _isPlaying;
        private float _lastTime;
        private int _currentCount;
        
        protected override Status OnStart()
        {
            if (Unit.Value == null || Unit.Value.UnitAnimator == null) return Status.Failure;

            _animator = Unit.Value.UnitAnimator;
            _animator.OnAnimationEnd += HandleAnimationEnd;
            _currentCount = 0;
            _lastTime = 0;
            _isPlaying = false;
            
            return Status.Running;
        }

        private void HandleAnimationEnd()
        {
            _currentCount++;
            _lastTime = Time.time;
            _animator.SetParameter(Param.Value, false);
            _isPlaying = false;
        }

        private void PlayAnimation()
        {
            _isPlaying = true;
            _animator.SetParameter(Param.Value, true);
        }

        protected override Status OnUpdate()
        {
            if(_currentCount >= Count.Value)
                return Status.Success;

            if (Time.time >= _lastTime + IntervalDelay.Value && !_isPlaying)
            {
                PlayAnimation();
            }
            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_animator != null)
            {
                _animator.SetParameter(Param.Value, false);
                _animator.OnAnimationEnd -= HandleAnimationEnd;
            }
        }
    }
}

