using System;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units.Animations;
using Code.Units.Combat;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Attack target", story: "[Unit] attack [Target] until it dies", category: "Action/Units", id: "de2be21f34161934e5a792a6e82a6c03")]
    public partial class AttackTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractCombatUnit> Unit;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;
        [SerializeReference] public BlackboardVariable<ParameterSO> AttackParam;
        [SerializeReference] public BlackboardVariable<ParameterSO> IndexParam;
        [SerializeReference] public BlackboardVariable<int> ClipCount = new(2);
        [SerializeReference] public BlackboardVariable<Transform> FirePositionTrm;

        [SerializeReference] public BlackboardVariable<float> MeleeOffset = new(0.8f);
        
        private Transform _selfTransform;
        private Transform _targetTransform;
        private IDamageable _targetDamageable;
        private NavMeshAgent _navMeshAgent;
        private UnitAnimator _unitAnimator;
        private float _lastAttackTime;
        private bool _isAnimationEnd;
        
        protected override Status OnStart()
        {
            if (!InitializeNode()) return Status.Failure;

            if (Time.time <= _lastAttackTime + AttackConfig.Value.AttackDelay)
                return Status.Failure;
            
            _unitAnimator.OnAnimationEnd += HandleAnimationEnd;
            _unitAnimator.OnAttackTrigger += HandleAttackTrigger;
            AttackTarget();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_isAnimationEnd)
                return Status.Success;
            
            return Status.Running;
        }

        private void AttackTarget()
        {
            _isAnimationEnd = false;
            _navMeshAgent.ResetPath();
            _navMeshAgent.isStopped = true;
            
            Vector3 lookDirection = _targetTransform.position - _selfTransform.position;
            lookDirection.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection.normalized);
            _selfTransform.rotation = lookRotation;
            
            
            _unitAnimator.SetParameter(IndexParam.Value, (float) Random.Range(0, ClipCount.Value));
            _unitAnimator.SetParameter(AttackParam.Value, true);
            
        }

        protected override void OnEnd()
        {
            if (CurrentStatus == Status.Success || _unitAnimator != null)
            {
                _unitAnimator.OnAnimationEnd -= HandleAnimationEnd;
                _unitAnimator.OnAttackTrigger -= HandleAttackTrigger;
                _unitAnimator.SetParameter(AttackParam.Value, false);
            }
            if(_navMeshAgent != null && _navMeshAgent.isOnNavMesh)
                _navMeshAgent.isStopped = false;
        }

        private void HandleAttackTrigger()
        {
            if (_targetDamageable.Transform == null) return;
            
            if (FirePositionTrm.Value == null)
            {
                Vector3 targetPosition = _targetTransform.position;
                targetPosition.y += MeleeOffset.Value;
                Vector3 normal = (_selfTransform.position - targetPosition).normalized;
                
                _targetDamageable.TakeDamage(AttackConfig.Value.Damage, targetPosition, normal);
            }
            else
            {
                float yOffset = FirePositionTrm.Value.position.y - Unit.Value.transform.position.y;
                Vector3 targetPosition = _targetDamageable.Transform.position + new Vector3(0, yOffset, 0);
                Bus<ProjectileLaunchEvent>.Raise(Unit.Value.Owner, new ProjectileLaunchEvent(
                    AttackConfig.Value.ProjectileItem,
                    FirePositionTrm.Value.position,
                    targetPosition,
                    _targetDamageable,
                    AttackConfig.Value.ProjectileSpeed,
                    AttackConfig.Value.Damage)); 
            }
        }

        private void HandleAnimationEnd()
        {
            _isAnimationEnd = true;
            _lastAttackTime = Time.time;
            _unitAnimator.SetParameter(AttackParam.Value, false);
        }

        private bool InitializeNode()
        {
            if(Unit.Value == null || Target.Value == null || AttackConfig.Value == null)
                return false;

            _selfTransform = Unit.Value.transform;
            _targetTransform = Target.Value.transform;
            _navMeshAgent = Unit.Value.Agent;
            _unitAnimator = Unit.Value.UnitAnimator;
            if (_targetTransform.TryGetComponent(out _targetDamageable) == false) return false;
            return true;
        }

        
    }
}

