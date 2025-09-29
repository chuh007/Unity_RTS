using System.Collections;
using System.Collections.Generic;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units.Buildings;
using Code.Units.Data;
using UnityEngine;

namespace Code.Units
{
    public class BaseBuilding : AbstractCommandable, IBuilding
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
        
        public BuildingSO BuildingSo { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            BuildingSo = UnitSo as BuildingSO;
        }

        protected override void Start()
        {
            base.Start();
            Bus<BuildingSpawnEvent>.Raise(new BuildingSpawnEvent(this));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bus<BuildingDeathEvent>.Raise(new BuildingDeathEvent(this));
            
        }

        public void BuildUnit(UnitSO unitToBuild)
        {
            if (_buildingQueue.Count >= MAX_QUEUE_SIZE)
            {
                Debug.LogError("Building queue is full, can not add more units");
                return;
            }

            Bus<SupplyEvent>.Raise(new SupplyEvent(-unitToBuild.Cost.Minerals, unitToBuild.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(new SupplyEvent(-unitToBuild.Cost.Gas, unitToBuild.Cost.GasSO));
            
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
            
            Bus<SupplyEvent>.Raise(new SupplyEvent(unitToCancel.Cost.Minerals, unitToCancel.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(new SupplyEvent(unitToCancel.Cost.Gas, unitToCancel.Cost.GasSO));
            
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