using Code.Environments;
using UnityEngine;

namespace Code.Units
{
    [CreateAssetMenu(fileName = "Supply Cost", menuName = "Units/Cost", order = 0)]
    public class SupplyCostSO : ScriptableObject
    {
        [field: SerializeField] public int Minerals { get; private set; } = 50;
        [field: SerializeField] public SupplySO MineralsSO { get; private set; }
        [field: SerializeField] public int  Gas { get; private set; }
        [field: SerializeField] public SupplySO GasSO { get; private set; }
    }
}