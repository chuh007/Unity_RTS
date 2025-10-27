using ObjectPool.RunTime;
using UnityEngine;

namespace Code.Units.Combat
{
    [CreateAssetMenu(fileName = "Attack config", menuName = "Units/Attack config", order = 7)]
    public class AttackConfigSO : ScriptableObject
    {
        [field: SerializeField] public float DetectRange { get; private set; } = 8f;
        [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
        [field: SerializeField] public float AttackDelay { get; private set; } = 1f;
        [field: SerializeField] public int Damage { get; private set; } = 5;
        [field: SerializeField] public PoolingItemSO ProjectileItem { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 20f;
        [field: SerializeField] public LayerMask DamageableLayers { get; private set; }
        
    }
}