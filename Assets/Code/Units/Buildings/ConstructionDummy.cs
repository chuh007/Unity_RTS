using System;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Units.Buildings
{
    public class ConstructionDummy : AbstractCommandable, IBuilding
    {
        [SerializeField] private GameObject[] constructionStage;
        [SerializeField] private GameObject ghostVisual;

        private int _currentStateIndex = 0;
        private IBuildingConstructor _unitBuildingThis;
        public BuildingSO BuildingSo { get; private set; }
        
        [field: SerializeField]
        public ConstructionProgress ProgressData { get; private set; }
            = new ConstructionProgress(BuildingState.Ghost, 0, 0);
        
        [field: SerializeField] public Renderer MainRenderer { get; private set; }

        public IBuildingConstructor UnitBuildingThis
        {
            get => _unitBuildingThis;
            set
            {
                if (_unitBuildingThis != value)
                {
                    Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
                    if (value != null)
                        Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
                }
                _unitBuildingThis = value;
            }
        }

        protected override void Start()
        {
            base.Start();
            BuildingSo = UnitSo as BuildingSO;
            Debug.Assert(BuildingSo != null, $"BuildingSo is not assigned in {name}");
            CurrentHealth = 0;
        }
        
        public void UpdateConstructionProgress(float progress)
        {
            if (progress < 0 || progress > 1 || _currentStateIndex >= constructionStage.Length)
                return;

            if (_currentStateIndex == 0 && progress > 0.45f)
                ChangeConstructionStage(1);

            if (_currentStateIndex == 1 && progress > 0.9f)
                ChangeConstructionStage(2);
        }

        public void StartPlacementGhost(IBuildingConstructor constructor)
        {
            UnitBuildingThis = constructor;
            SetGhostVisual(true);
            ProgressData = new ConstructionProgress(BuildingState.Ghost, Time.time, 0);
        }

        public void StartConstruction(IBuildingConstructor constructor)
        {
            UnitBuildingThis = constructor;
            if (ProgressData.State == BuildingState.Ghost)
            {
                SetGhostVisual(false);
                ChangeConstructionStage(0);
            }

            ProgressData = new ConstructionProgress(BuildingState.Constructing,
                Time.time - BuildingSo.BuildTime * ProgressData.Progress, ProgressData.Progress);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            if (evt.Unit is IBuildingConstructor constructor && constructor == _unitBuildingThis)
            {
                if (ProgressData.State == BuildingState.Ghost)
                {
                    Destroy(gameObject);
                    return;
                }

                ProgressData = new ConstructionProgress(
                    BuildingState.Paused,
                    ProgressData.StartTime,
                    (Time.time - ProgressData.StartTime) / BuildingSo.BuildTime);
            }
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }
        
        private void ChangeConstructionStage(int index)
        {
            if (index < 0 || index >= constructionStage.Length)
                return;
            
            constructionStage[_currentStateIndex].SetActive(false);
            _currentStateIndex = index;
            constructionStage[_currentStateIndex].SetActive(true);
        }

        private void SetGhostVisual(bool isActive)
        {
            constructionStage[_currentStateIndex].SetActive(!isActive);
            ghostVisual.SetActive(isActive);
        }

        public void ConstructionComplete()
        {
            if (IsSelected)
            {
                DeSelect();
            }
        }
    }
}