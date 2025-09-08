using System.Collections.Generic;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units.Animations;
using Code.Units.BT;
using Code.Units.BT.Events;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(UnitAnimator))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        [SerializeField] private VariableSO[] btVariables;
        private Dictionary<BTVariables, SerializableGUID> _variableDict;
        
        public BehaviorGraphAgent GraphAgent { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        public float AgentRadius => Agent.radius;
        public UnitAnimator UnitAnimator { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            GraphAgent = GetComponent<BehaviorGraphAgent>();
            Agent = GetComponent<NavMeshAgent>();
            UnitAnimator = GetComponent<UnitAnimator>();
        }

        protected override void Start()
        {
            base.Start();
            //BT그래프 초기화가 Awake에서 이루어지기 때문에 BT관련 작업은 Start에서 해야 정상적
            _variableDict = new Dictionary<BTVariables, SerializableGUID>();
            foreach (VariableSO variable in btVariables)
            {
                if (!GraphAgent.GetVariableID(variable.VariableName.ToString(), out var guid))
                {
                    Debug.LogError($"Missing required variable for unit : {variable.VariableName}");
                    continue;
                }
                _variableDict.Add(variable.VariableName, guid); //딕셔너리에 저장.
            }
            
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }

        public void MoveTo(Vector3 position)
        {
            SetVariableValue(BTVariables.TargetLocation, position);
            SetVariableValue(BTVariables.Command, UnitCommands.Move);
            //Agent.SetDestination(position);
        }

        public void Stop()
        {
            SetVariableValue(BTVariables.TargetLocation, transform.position);
            SetVariableValue(BTVariables.Command, UnitCommands.Stop);
            //Agent.ResetPath();
        }

        public void SetVariableValue<T>(BTVariables variable, T value)
        {
            if (_variableDict.TryGetValue(variable, out SerializableGUID guid))
            {
                GraphAgent.SetVariableValue(guid, value);
                return;
            }
            
            Debug.LogError($"Variable not found : {variable} in unit : {gameObject.name}");
        }

        public bool GetVariableValue<T>(BTVariables variableName, out BlackboardVariable<T> variable)
        {
            if(_variableDict.TryGetValue(variableName, out SerializableGUID guid))
            {
                return GraphAgent.GetVariable(guid, out variable);
            }
            
            Debug.LogError($"Variable not found : {variableName} in unit : {gameObject.name}");
            variable = null;
            return false;
        }
    }
}