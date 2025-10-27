using System.Collections.Generic;
using Code.Units.BT;
using Code.Units.Combat;
using Code.Util;
using Unity.Behavior;
using UnityEngine;

namespace Code.Units
{
    public class AbstractCombatUnit : AbstractUnit, IAttacker
    {
        [field: SerializeField] public DamageableSensor Sensor { get; private set; }
        
        protected override void Start()
        {
            base.Start();
            Debug.Assert(Sensor != null, $"Combat unit must have a sensor {gameObject.name}");
            
            SetVariableValue(BTVariables.AttackConfig, _unitSo.AttackConfig);
            Sensor.SetUpFrom(Owner, _unitSo.AttackConfig); //셋팅한다.
            Sensor.OnUnitEnter += HandleUnitEnter;
            Sensor.OnUnitExit += HandleUnitExit;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Sensor.OnUnitEnter -= HandleUnitEnter;
            Sensor.OnUnitExit -= HandleUnitExit;
        }

        private void HandleUnitEnter(IDamageable damageable)
        {
            //Debug.Log($"Detect unit enter ! {Sensor.Damageables.Count}");
            if (!GetVariable(BTVariables.Command, out BlackboardVariable<UnitCommands> currentCommand)
                || currentCommand.Value != UnitCommands.Attack) return;

            List<GameObject> nearByEnemies = GetSortedNearByEnemies();

            if (GetVariable(BTVariables.TargetGameObject, out BlackboardVariable<GameObject> targetVariable)
                && targetVariable.Value == null
                && nearByEnemies.Count > 0)
            {
                //타겟 오브젝트가 아직 설정이 안되었고, 근거리에 있는 적이 1기 이상이라면 
                SetVariableValue(BTVariables.TargetGameObject, nearByEnemies[0]);
            }
        }

        private void HandleUnitExit(IDamageable damageable)
        {
            //Debug.Log($"Detect unit exit ! {Sensor.Damageables.Count}");
            if (!GetVariable(BTVariables.Command, out BlackboardVariable<UnitCommands> currentCommand)
                || currentCommand.Value != UnitCommands.Attack) return;

            if (!GetVariable(BTVariables.TargetGameObject, out BlackboardVariable<GameObject> targetVariable)
                || (targetVariable.Value != null
                    && damageable.Transform != null
                    && damageable.Transform.gameObject != targetVariable.Value))
                return; //현재 나간녀석이 타겟과 같은 녀석인지 체크해야한다.
            
            //타겟이 나간거라면 새로운 타겟을 찾아야 한다.
            List<GameObject> nearByEnemies = GetSortedNearByEnemies();

            if (nearByEnemies.Count > 0)
            {
                SetVariableValue(BTVariables.TargetGameObject, nearByEnemies[0]);
            }
            else  //근처에 적이 하나도 없었다면.
            {
                SetVariableValue<GameObject>(BTVariables.TargetGameObject, null);
                SetVariableValue(BTVariables.TargetLocation, damageable.Transform.position);
                //마지막으로 나간 녀석의 위치를 기준으로 다시 이동한다.
            }
        }
        
        private List<GameObject> GetSortedNearByEnemies()
        {
            List<GameObject> nearByEnemies = Sensor.Damageables.ConvertAll(x => x.Transform.gameObject);
            nearByEnemies.Sort(new ClosestGameObjectComparer(transform.position));
            return nearByEnemies;
        }
        
        public void Attack(IDamageable damageable)
        {
            //Debug.Log($"{gameObject.name} should attack {damageable.Transform.name}");
            if (damageable.Transform.gameObject == gameObject) return;
            SetVariableValue(BTVariables.TargetGameObject, damageable.Transform.gameObject);
            SetVariableValue(BTVariables.Command, UnitCommands.Attack);
        }

        public void Attack(Vector3 location)
        {
            SetVariableValue<GameObject>(BTVariables.TargetGameObject, null);
            SetVariableValue(BTVariables.TargetLocation, location);
            SetVariableValue(BTVariables.Command, UnitCommands.Attack);

            List<GameObject> nearByEnemies = GetSortedNearByEnemies();
            if (nearByEnemies.Count > 0)
            {
                SetVariableValue(BTVariables.TargetGameObject, nearByEnemies[0]);
            }
        }
    }
}