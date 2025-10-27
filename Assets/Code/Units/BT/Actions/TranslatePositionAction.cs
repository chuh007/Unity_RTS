using System;
using Code.Units.Animations;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Translate position", story: "[Unit] move to [TargetLocation] at [Speed]", category: "Action/Transform", id: "a2321862989de87cfaf73c93259395c9")]
    public partial class TranslatePositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<float> Speed;

        [SerializeReference] public BlackboardVariable<ParameterSO> SpeedParameter;

        private UnitAnimator _animator;
        private float _endTime;
        private Vector3 _direction;
        private Transform _selfTrm;

        protected override Status OnStart()
        {
            if (Unit.Value == null) return Status.Failure;
            _animator = Unit.Value.UnitAnimator;
            _selfTrm = Unit.Value.transform;

            float distance = Vector3.Distance(_selfTrm.position, TargetLocation.Value);
            _endTime = Time.time + distance / Speed.Value; //걸리는 시간 측정
            _direction = (TargetLocation.Value - _selfTrm.position).normalized;
            _selfTrm.forward = _direction; //해당 방향으로 유닛 회전.
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Time.time > _endTime) return Status.Success;
            
            _animator?.SetParameter(SpeedParameter.Value, Speed.Value);
            _selfTrm.position += Speed.Value * Time.deltaTime * _direction;

            return Status.Running;
        }

        protected override void OnEnd()
        {
            _animator?.SetParameter(SpeedParameter.Value, 0f);
        }
    }
}

