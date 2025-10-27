using UnityEngine;

namespace Code.Units.BT
{
    public enum BTVariables
    {
        TargetLocation,
        Command,
        TargetGameObject,
        SupplyAmountHeld,
        SupplyType,
        GatherableSupply,
        HeadQuarter,
        SupplyEventChannel,
        ConstructionDummy,
        ConstructBuildingSO,
        ConstructionEventChannel,
        AttackConfig,
        FirePositionTrm,
    }

    [CreateAssetMenu(fileName = "Variable data", menuName = "Units/BT/Variable", order = 10)]
    public class VariableSO : ScriptableObject
    {
        [field: SerializeField] public BTVariables VariableName { get; set; }
    }
}