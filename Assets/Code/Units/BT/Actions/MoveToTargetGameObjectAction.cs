using System;
using Code.Units.Animations;
using Code.Util;
using TMPro;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to TargetGameObject", story: "[Unit] move to [TargetGameObject]", category: "Action/Navigation", id: "e3e8fa6cab9cb573da3e195790776f03")]
    public partial class MoveToTargetGameObjectAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;

        [SerializeReference] public BlackboardVariable<float> MoveThreshold = new(0.25f);
        [SerializeReference] public BlackboardVariable<ParameterSO> SpeedParameter;

        private NavMeshAgent _agent;
        private Vector3 _lastPosition;
        private Collider _targetCollider;
        private UnitAnimator _animator;

        protected override Status OnStart()
        {
            if (Unit.Value == null || Unit.Value.Agent == null || TargetGameObject.Value == null)
                return Status.Failure;

            _animator = Unit.Value.UnitAnimator;
            _agent = Unit.Value.Agent;
            _targetCollider = TargetGameObject.Value.GetComponent<Collider>();
            
            Vector3 targetPosition 
                = DistanceUtil.GetTargetPosition(Unit.Value.gameObject, _targetCollider);
            if (Vector3.Distance(targetPosition, _agent.transform.position) < _agent.stoppingDistance)
                return Status.Success;

            _agent.SetDestination(targetPosition);
            _lastPosition = targetPosition;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _animator?.SetParameter(SpeedParameter.Value, _agent.velocity.magnitude);
            
            Vector3 newTargetPosition 
                = DistanceUtil.GetTargetPosition(Unit.Value.gameObject, _targetCollider);

            if (Vector3.Distance(newTargetPosition, _lastPosition) >= MoveThreshold)
            {
                _agent.SetDestination(newTargetPosition);
                _lastPosition = newTargetPosition;
                return Status.Running;
            }

            if (_agent.pathPending == false && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                return Status.Success;
            }
            return Status.Running;
        }
        
        // private Vector3 GetTargetPosition()
        // {
        //     if (_targetCollider != null)
        //     {
        //         return _targetCollider.ClosestPoint(_agent.transform.position);
        //     }
        //
        //     return TargetGameObject.Value.transform.position;
        // }


        protected override void OnEnd()
        {
            _animator?.SetParameter(SpeedParameter.Value, 0f);
        }
    }
}

