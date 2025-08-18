using System;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour, ISelectable, IMoveable
    {
        [SerializeField] private GameObject decalProjector;
        private NavMeshAgent _agent;
        
        public bool IsSelected { get; private set; }
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Select()
        {
            IsSelected = true;
            decalProjector?.SetActive(true);
        }

        public void Deselect()
        {
            IsSelected = false;
            decalProjector?.SetActive(false);
        }

        public void MoveTo(Vector3 position)
        {
            _agent.SetDestination(position);
        }

        public void Stop()
        {
            _agent.ResetPath();
        }
    }
}