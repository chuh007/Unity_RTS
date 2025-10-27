using System.Collections;
using System.Collections.Generic;
using Code.CoreSystem;
using Code.GameEvents;
using Code.TechTree;
using Code.Units.Buildings;
using Code.Units.Data;
using ObjectPool.RunTime;
using UnityEngine;

namespace Code.Units
{
    public class BaseBuilding : AbstractCommandable, IBuilding
    {
        public const int MAX_QUEUE_SIZE = 5;
        
        private List<UnlockableSO> _buildingQueue = new List<UnlockableSO>(MAX_QUEUE_SIZE);

        public int QueueSize => _buildingQueue.Count;
        public UnlockableSO[] Queue => _buildingQueue.ToArray(); //콜렉션을 방해하지 않게 복사본으로 뺀다.
        
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public UnlockableSO SOBeingBuilt { get; private set; } //현재 빌드중인 유닛정보
        [field: SerializeField] public PoolingItemSO DestroyEffect { get; private set; }
        
        //UI를 위해 발행하는 이벤트 
        public delegate void QueueUpdatedEvent(UnlockableSO[] unitsInQueue);
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
            Bus<BuildingSpawnEvent>.Raise(Owner, new BuildingSpawnEvent(Owner, this));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bus<PoolEffectPlayEvent>.Raise(Owner,
                new PoolEffectPlayEvent(DestroyEffect, transform.position, Quaternion.identity));
            Bus<BuildingDeathEvent>.Raise(Owner, new BuildingDeathEvent(Owner, this));
        }

        public void BuildUnlockable(UnlockableSO unlockable)
        {
            if (_buildingQueue.Count >= MAX_QUEUE_SIZE)
            {
                Debug.LogError("Building queue is full, can not add more units");
                return;
            }
            
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, -unlockable.Cost.Minerals, unlockable.Cost.MineralSO));
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, -unlockable.Cost.Gas, unlockable.Cost.GasSO));
             
            _buildingQueue.Add(unlockable);
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

                if (SOBeingBuilt is AbstractUnitSO unitSO)
                {
                    GameObject newUnit = Instantiate(unitSO.Prefab, transform.position, Quaternion.identity);
                    if (newUnit.TryGetComponent(out AbstractCommandable commandable))
                    {
                        commandable.Owner = Owner; 
                    }
                }
                else if (SOBeingBuilt is UpgradeSO upgrade)
                {
                    Bus<UpgradeResearchedEvent>.Raise(Owner, new UpgradeResearchedEvent(Owner, upgrade));
                }
                _buildingQueue.RemoveAt(0);
            }
            
            OnQueueUpdated?.Invoke(Queue);
        }

        public void CancelGenerate(int idx)
        {
            Debug.Assert(idx >= 0 && idx < _buildingQueue.Count, 
                "Attempting to cancel building a unit outside index");

            UnlockableSO unlockToCancel = _buildingQueue[idx];
            
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, unlockToCancel.Cost.Minerals, unlockToCancel.Cost.MineralSO));
            Bus<SupplyEvent>.Raise(Owner, new SupplyEvent(Owner, unlockToCancel.Cost.Gas, unlockToCancel.Cost.GasSO));
            
            
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