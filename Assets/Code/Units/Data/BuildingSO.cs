using UnityEngine;

namespace Code.Units.Data
{
    [CreateAssetMenu(fileName = "Building", menuName = "Buildings/Data", order = 11)]
    public class BuildingSO : AbstractUnitSO
    {
        [field: SerializeField] public GameObject ConstructionDummy { get; private set; }
    }
}