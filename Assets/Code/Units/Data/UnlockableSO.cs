using System.Collections.Generic;
using System.Linq;
using Code.TechTree;
using UnityEngine;

namespace Code.Units.Data
{
    public abstract class UnlockableSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; } = "Unit";
        [field: SerializeField] public float BuildTime { get; private set; } = 5;
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public SupplyCostSO Cost { get; private set; }
        [field: SerializeField] public TechTreeSO TechTree { get; private set; }

        [field: SerializeField] public List<UnlockableSO> UnlockRequirements { get; private set; }
            = new List<UnlockableSO>();
        public IEnumerable<UnlockableSO> Requirements => UnlockRequirements.ToList();

    }
}