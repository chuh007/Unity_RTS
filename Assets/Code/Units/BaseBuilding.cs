using System.Collections;
using System.Collections.Generic;
using Code.Units.Data;
using UnityEngine;

namespace Code.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        private const int MAX_QUEUE_SIZE = 5;
        private List<UnitSO> _buildingQueue = new List<UnitSO>(MAX_QUEUE_SIZE);
        
        public int QueueSize => _buildingQueue.Count;
        public UnitSO[] Queue => _buildingQueue.ToArray();
        
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public UnitSO SOBeingBuilt { get; private set; }

        public delegate void QueueUpdatedEvent(UnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;
        
        public void BuildUnit(UnitSO unitToBuild)
        {
            // 자원 소모, 빌딩 큐, 생성
            
            _buildingQueue.Add(unitToBuild);
            if (_buildingQueue.Count == 1)
            {
                StartCoroutine(DoBuildUnit());
            }
            else
            {
                OnQueueUpdated?.Invoke(Queue);
            }
        }

        private IEnumerator DoBuildUnit()
        {
            while (_buildingQueue.Count > 0)
            {
                SOBeingBuilt = _buildingQueue[0];
                CurrentQueueStartTime = Time.time;
                OnQueueUpdated?.Invoke(Queue);
                
                yield return new WaitForSeconds(SOBeingBuilt.BuildTime);

                GameObject newUnit = Instantiate(SOBeingBuilt.Prefab, transform.position, Quaternion.identity);
                
                _buildingQueue.RemoveAt(0);
            }
            OnQueueUpdated?.Invoke(Queue);
        }
    }
}