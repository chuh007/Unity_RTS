using System;
using Code.CoreSystem;
using Code.GameEvents;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        public NavMeshAgent Agent { get; private set; }
        public float AgentRadius => Agent.radius;
        
        protected override void Awake()
        {
            base.Awake();
            Agent = GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            base.Start();
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }

        public void MoveTo(Vector3 position)
        {
            Agent.SetDestination(position);
        }

        public void Stop()
        {
            Agent.ResetPath();
        }
    }
}