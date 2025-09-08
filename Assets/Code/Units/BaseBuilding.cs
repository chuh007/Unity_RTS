using System.Collections;
using System.Collections.Generic;
using Code.Units.Data;
using UnityEngine;

namespace Code.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public const int MAX_QUEUE_SIZE = 5;
        
        private List<UnitSO> _buildingQueue = new List<UnitSO>(MAX_QUEUE_SIZE);

        public int QueueSize => _buildingQueue.Count;
        public UnitSO[] Queue => _buildingQueue.ToArray(); //콜렉션을 방해하지 않게 복사본으로 뺀다.
        
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public UnitSO SOBeingBuilt { get; private set; } //현재 빌드중인 유닛정보

        //UI를 위해 발행하는 이벤트 
        public delegate void QueueUpdatedEvent(UnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;
        
        public void BuildUnit(UnitSO unitToBuild)
        {
            //자원 소모 관련 로직이 차후에 여기 들어와야 하고
            
            //빌딩 큐 검사 관련 로직이 여기 들어가야 하고
            
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
                //newUnit은 나중에 쓴다.
                _buildingQueue.RemoveAt(0);
            }
            
            OnQueueUpdated?.Invoke(Queue);
        }

        public void CancelGenerate(int idx)
        {
            Debug.Assert(idx >= 0 && idx < _buildingQueue.Count, 
                "Attempting to cancel building a unit outside index");

            UnitSO unitToCancel = _buildingQueue[idx];
            
            //여기에 나중에 자원 반환 로직이 들어간다.
            _buildingQueue.RemoveAt(idx);
            if (idx == 0) //지금 만들어지는게 취소된거라면.
            {
                StopAllCoroutines();
                if (_buildingQueue.Count > 0)
                {
                    StartCoroutine(DoBuildUnit());
                }
                else
                {
                    OnQueueUpdated?.Invoke(Queue);
                }
            }
            else
            {
                OnQueueUpdated?.Invoke(Queue);
            }
        }
    }
}