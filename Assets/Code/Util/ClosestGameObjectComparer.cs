using System.Collections.Generic;
using UnityEngine;

namespace Code.Util
{
    public struct ClosestGameObjectComparer : IComparer<GameObject>
    {
        private Vector3 _targetPosition;

        public ClosestGameObjectComparer(Vector3 position)
        {
            _targetPosition = position;
        }
        
        public int Compare(GameObject x, GameObject y)
        {
            return (x.transform.position - _targetPosition).sqrMagnitude
                .CompareTo((y.transform.position - _targetPosition).sqrMagnitude);
        }
    }
}