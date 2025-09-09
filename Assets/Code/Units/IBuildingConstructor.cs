using Code.Units.Data;
using UnityEngine;

namespace Code.Units
{
    public interface IBuildingConstructor
    {
        bool IsBuilding { get; }
        GameObject Build(BuildingSO buildingData, Vector3 targetLocation);
        void CancelBuilding();
        void ResumeBuilding(BaseBuilding building);
    }
}