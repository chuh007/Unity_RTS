using UnityEngine;

namespace Code.Units.Data
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Unit/Unit", order = 0)]
    public class UnitSO : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public float BuildTime { get; private set; }
    }
}