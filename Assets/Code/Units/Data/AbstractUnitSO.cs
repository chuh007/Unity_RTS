using Code.TechTree;
using UnityEngine;

namespace Code.Units.Data
{
    public abstract class AbstractUnitSO : UnlockableSO
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public UpgradeSO[] Upgrades { get; private set; }
    }
}