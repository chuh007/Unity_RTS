using UnityEngine;

namespace Code.Util
{
    public static class DistanceUtil
    {
        public static Vector3 GetTargetPosition(GameObject self, Collider targetCollider)
        {
            if ( targetCollider != null)
            {
                return targetCollider.ClosestPoint(self.transform.position);
            }
            
            return targetCollider.transform.position;
        }

        public static Vector3 GetTargetPosition(Collider collider, Vector3 position)
        {
            return collider.ClosestPoint(position);
        }
    }
}