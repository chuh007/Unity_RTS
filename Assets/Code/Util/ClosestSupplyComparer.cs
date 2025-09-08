using System.Collections.Generic;
using Code.Environments;
using UnityEngine;

namespace Code.Util
{
    public struct ClosestSupplyComparer : IComparer<GatherableSupply>
    {
        private Vector3 _targetPosition;

        public ClosestSupplyComparer(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
        }
        
        public int Compare(GatherableSupply x, GatherableSupply y)
        {
            return (x.transform.position - _targetPosition).sqrMagnitude
                .CompareTo((y.transform.position - _targetPosition).sqrMagnitude);
        }
    }
}