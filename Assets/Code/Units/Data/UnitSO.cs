using Code.Units.Combat;
using UnityEngine;

namespace Code.Units.Data
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Units/Unit", order = 10)]
    public class UnitSO : AbstractUnitSO
    {
        [field: SerializeField] public AttackConfigSO AttackConfig { get; private set; }
    }
}