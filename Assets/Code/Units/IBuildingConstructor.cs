using Code.Units.Buildings;
using Code.Units.Data;
using UnityEngine;

namespace Code.Units
{
    public interface IBuildingConstructor
    {
        bool IsBuilding { get; }
        GameObject ConstructBuilding(BuildingSO buildingData, Vector3 targetLocation);
        void CancelConstruction();
        void ResumeConstruction(ConstructionDummy dummy);
    }
}