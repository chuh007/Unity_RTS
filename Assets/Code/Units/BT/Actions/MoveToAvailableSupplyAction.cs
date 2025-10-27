using System;
using System.Collections.Generic;
using System.Linq;
using Code.Environments;
using Code.Units.Animations;
using Code.Util;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Code.Units.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to available supply", story: "[Unit] move to [GatherableSupply] or nearby not busy", category: "Action/Navigation", id: "af61281af5a14c40caa5b8f36960e231")]
    public partial class MoveToAvailableSupplyAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractUnit> Unit;
        [SerializeReference] public BlackboardVariable<GatherableSupply> GatherableSupply;
        [SerializeReference] public BlackboardVariable<float> SearchRadius = new(7f);
        [SerializeReference] public BlackboardVariable<ParameterSO> SpeedParameter;

        private NavMeshAgent _agent;
        private LayerMask _supplyLayerMask;
        private SupplySO _supplySO;
        private Collider _collider;
        private UnitAnimator _animator;
        private Collider[] _results = new Collider[15];
        private List<GatherableSupply> _notBusySupplies = new List<GatherableSupply>(15);

        protected override Status OnStart()
        {
            if (!HasValidInputs())
            {
                return Status.Failure;
            }

            _animator = Unit.Value.UnitAnimator;
            _collider = GatherableSupply.Value.GetComponent<Collider>();
            _supplyLayerMask = LayerMask.GetMask("Supplies");
            Vector3 targetPosition = GetTargetPosition();
            _agent.SetDestination(targetPosition);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _animator?.SetParameter(SpeedParameter.Value, _agent.velocity.magnitude);
            
            if (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
            {
                return Status.Running;
            }
            
            if(GatherableSupply.Value != null 
               && !GatherableSupply.Value.IsBusy
               && GatherableSupply.Value.Amount > 0)
            {
                return Status.Success;
            }
            
            //도착했는데 자원이 없거나, 누군가 채굴중이라면 주변에 다른 자원이 있는지 확인
            if (UpdateNearByNotBusySupply())
            {
                _collider = GatherableSupply.Value.GetComponent<Collider>();
                Vector3 targetPosition = GetTargetPosition();
                _agent.SetDestination(targetPosition);
                return Status.Running;
            }
            
            return Status.Failure;
        }

        protected override void OnEnd()
        {
            _animator?.SetParameter(SpeedParameter.Value, 0f);
        }

        #region Utility Methods

        private bool HasValidInputs()
        {
            if (Unit.Value == null || Unit.Value.Agent == null ||
                (GatherableSupply.Value == null && _supplySO == null))
                return false;
            _agent = Unit.Value.Agent;

            if (GatherableSupply.Value != null)
            {
                _supplySO = GatherableSupply.Value.SupplyData; 
                //처음 채굴하라고 명령을 내린 것과 같은 종류 것을 찾아야 하니 셋팅
            }
            else
            {
                return UpdateNearByNotBusySupply();
            }

            return true;
        }

        private bool UpdateNearByNotBusySupply()
        {
            int cnt = Physics.OverlapSphereNonAlloc(
                _agent.transform.position, SearchRadius.Value, _results, _supplyLayerMask);
            
            _notBusySupplies.Clear();
            for(int i = 0; i < cnt; i++)
            {
                if(_results[i].TryGetComponent(out GatherableSupply supply) 
                   && supply.SupplyData.Equals(_supplySO) 
                   && !supply.IsBusy)
                {
                    _notBusySupplies.Add(supply);
                }
            }

            if (_notBusySupplies.Count > 0)
            {
                _notBusySupplies.Sort(new ClosestSupplyComparer(_agent.transform.position));
                GatherableSupply.Value = _notBusySupplies.First();
                return true; //가장 가까운 자원을 리턴한다.
            }

            return false;
        }

        private Vector3 GetTargetPosition()
        {
            if (_collider == null)
                return GatherableSupply.Value.transform.position;
            return _collider.ClosestPoint(_agent.transform.position);
        }

        #endregion
       
    }
}

