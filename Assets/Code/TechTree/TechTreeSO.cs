using System;
using System.Collections.Generic;
using System.Linq;
using Code.CoreSystem;
using Code.GameEvents;
using Code.Units;
using Code.Units.Data;
using UnityEngine;

namespace Code.TechTree
{
    [CreateAssetMenu(fileName = "Tech tree", menuName = "Tech/Tech tree", order = 1)]
    public class TechTreeSO : ScriptableObject
    {
        [SerializeField] private List<UnlockableSO> allUnlockables;
        public IEnumerable<UnlockableSO> AllUnlockables => allUnlockables.ToList();

        private Dictionary<Owner, Dictionary<UnlockableSO, Dependency>> _techTree;
        private Dictionary<Owner, HashSet<UnlockableSO>> _unlockedDependencies;
        public bool IsUnlocked(Owner owner, UnlockableSO unlockable)
            => _techTree[owner].TryGetValue(unlockable, out Dependency dependency) && dependency.IsUnlocked;

        public bool IsResearched(Owner owner, UnlockableSO unlockable)
            => _unlockedDependencies[owner].Contains(unlockable);
        
        private void OnEnable()
        {
            if (_techTree == null)
            {
                BuildTechTrees();
            }
            Bus<BuildingSpawnEvent>.RegisterForAll(HandleBuildingSpawn);
            Bus<BuildingDeathEvent>.RegisterForAll(HandleBuildingDeath);
            Bus<UpgradeResearchedEvent>.RegisterForAll(HandleUpgradeResearched);
        }

        private void OnDisable()
        {
            _techTree = null;
            Bus<BuildingSpawnEvent>.UnRegisterForAll(HandleBuildingSpawn);
            Bus<BuildingDeathEvent>.UnRegisterForAll(HandleBuildingDeath);
            Bus<UpgradeResearchedEvent>.UnRegisterForAll(HandleUpgradeResearched);
        }

        private void HandleUpgradeResearched(UpgradeResearchedEvent evt)
        {
            Debug.Log($"Reserched {evt.Upgrade.name} for {evt.Owner}");
            _unlockedDependencies[evt.Owner].Add(evt.Upgrade);

            foreach (var kvp in _techTree[evt.Owner])
            {
                kvp.Value.UnlockDependency(evt.Upgrade);
            }
        }

        private void HandleBuildingSpawn(BuildingSpawnEvent evt)
        {
            foreach (KeyValuePair<UnlockableSO, Dependency> kvp in _techTree[evt.Owner])
            {
                //현재 지어진 건물을 언록처리해준다.
                kvp.Value.UnlockDependency(evt.Building.BuildingSo);
            }
        }

        private void HandleBuildingDeath(BuildingDeathEvent evt)
        {
            foreach (KeyValuePair<UnlockableSO, Dependency> kvp in _techTree[evt.Owner])
            {
                //현재 지어진 건물을 언록처리해준다.
                kvp.Value.LoseDependency(evt.Building.BuildingSo);
            }
        }

        private void BuildTechTrees()
        {
            _techTree = new Dictionary<Owner, Dictionary<UnlockableSO, Dependency>>();
            _unlockedDependencies = new Dictionary<Owner, HashSet<UnlockableSO>>();
            
            foreach (Owner ownerType in Enum.GetValues(typeof(Owner)))
            {
                //Debug.Log($"============Add <color=blue>{ownerType}</color> to Tech dependency");
                _techTree.Add(ownerType, new Dictionary<UnlockableSO, Dependency>());
                _unlockedDependencies.Add(ownerType, new HashSet<UnlockableSO>());
                
                foreach (UnlockableSO unlockableSo in allUnlockables)
                {
                    //Debug.Log($"Configuring {unlockableSo}' {unlockableSo.Requirements.Count()}");
                    _techTree[ownerType].Add(unlockableSo, new Dependency(unlockableSo));
                }
            }
        }
    }
}