using System.Collections.Generic;
using Code.Units;
using UnityEngine;

namespace Code.Util
{
    public struct ClosestBaseBuildingComparer : IComparer<BaseBuilding>
    {
        private Vector3 _targetPosition;

        public ClosestBaseBuildingComparer(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
        }
        
        public int Compare(BaseBuilding x, BaseBuilding y)
        {
            //반환값이 <0 이면 현재값이 비교대상보다 작고, 0이면 같고, >0 이면 현재값이 비교대상보다 크다.
            return (x.transform.position - _targetPosition).sqrMagnitude
                .CompareTo((y.transform.position - _targetPosition).sqrMagnitude);
        }
    }
}