using System.Collections.Generic;
using Code.Units.Data;
using UnityEngine;

namespace Code.TechTree
{
    public readonly struct Dependency
    {
        private readonly Dictionary<UnlockableSO, int> metPendencies;
        public HashSet<UnlockableSO> Dependencies { get; }
        
        public bool IsUnlocked => Dependencies.Count == metPendencies.Count;

        public Dependency(UnlockableSO unlockable)
        {
            Dependencies = new HashSet<UnlockableSO>(unlockable.Requirements);
            metPendencies = new Dictionary<UnlockableSO, int>(Dependencies.Count);
        }

        public void UnlockDependency(UnlockableSO unlockable)
        {
            // Debug.Log($"Attempting to unlock dependency {unlockable.Name}");
            if (Dependencies.Contains(unlockable) && !metPendencies.TryAdd(unlockable, 1))
            {
                metPendencies[unlockable]++;
            }

            // if (Dependencies.Contains(unlockable))
            // {
            //     Debug.Log($"Met dependencies for {unlockable.Name} : {metPendencies[unlockable]}");
            // }
        }

        public void LoseDependency(UnlockableSO unlockable)
        {
            // Debug.Log($"Attempting to unlock dependency {unlockable.Name}");
            if (!metPendencies.TryGetValue(unlockable, out int count)) return;
            count--;
            if (count > 0)
            {
                metPendencies[unlockable] = count;
            }
            else
            {
                metPendencies.Remove(unlockable);
            }
        }
    }
}