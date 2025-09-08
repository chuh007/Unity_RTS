using System;
using Code.Units.Animations;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to target location", story: "[Unit] move to [TargetLocation]", category: "Action/Navigation", id: "59ac8e034c1c8ad0031ec4b170aefcba")]
    public partial class MoveToTargetLocationAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<ParameterSO> SpeedParameter;
        
        private NavMeshAgent _agent;
        private UnitAnimator _animator;
        
        protected override Status OnStart()
        {
            if (Unit.Value == null) return Status.Failure;
            _agent = Unit.Value.Agent;
            _animator = Unit.Value.UnitAnimator;
            if (_agent == null) return Status.Failure;

            if (Vector3.Distance(_agent.transform.position, TargetLocation.Value) 
                    < _agent.stoppingDistance)
            {
                return Status.Success;
            }

            _agent.SetDestination(TargetLocation.Value);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_animator != null)
            {
                _animator.SetParameter(SpeedParameter.Value, _agent.velocity.magnitude);
            }
            if (_agent.pathPending == false && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                return Status.Success; 
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_animator != null)
            {
                _animator.SetParameter(SpeedParameter.Value, 0f);
            }
        }
    }
}

