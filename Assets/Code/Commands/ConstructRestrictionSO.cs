using UnityEngine;
using UnityEngine.AI;

namespace Code.Commands
{
    [CreateAssetMenu(fileName = "Construct restriction", menuName = "Buildings/Restriction", order = 7)]
    public class ConstructRestrictionSO : ScriptableObject
    {
        public enum OverlapType
        {
            Sphere, Box
        }

        [field: SerializeField] public float Radius { get; private set; } = 1f;
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        [field: SerializeField] public OverlapType HitDetectionType { get; private set; }
        [field: SerializeField] public bool MustBeFullyOnNavMesh { get; private set; } = true;
        [field: SerializeField] public int NavMeshAgentTypeID { get; private set; }
        [field: SerializeField] public float NavMeshTolerance { get; private set; } = 0.1f;
        [field: SerializeField] public Vector3 Extents { get; private set; } = Vector3.one;

        private Collider[] _hitColliders = new Collider[1];

        public bool CanPlace(Vector3 position)
        {
            int hitCnt = HitDetectionType switch
            {
                OverlapType.Box => Physics.OverlapBoxNonAlloc(position, Extents, _hitColliders,
                    Quaternion.identity, LayerMask),
                OverlapType.Sphere => Physics.OverlapSphereNonAlloc(position, Radius, _hitColliders, LayerMask),
                _ => 0
            };

            if (MustBeFullyOnNavMesh)
            {
                NavMeshQueryFilter filter = new()
                {
                    areaMask = NavMesh.AllAreas,
                    agentTypeID = NavMeshAgentTypeID
                };

                bool isOnNavMesh = IsFullyOnNavMesh(position, filter);

                return hitCnt == 0 && isOnNavMesh;
            }

            return hitCnt == 0;
        }

        private bool IsFullyOnNavMesh(Vector3 position, NavMeshQueryFilter filter)
        {
            bool isOnNavMesh = NavMesh.SamplePosition(position + new Vector3(Extents.x, 0, Extents.z),
                out NavMeshHit _, NavMeshTolerance, filter);
            
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(Extents.x, 0, -Extents.z),
                out NavMeshHit _, NavMeshTolerance, filter);
            
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(-Extents.x, 0, Extents.z),
                out NavMeshHit _, NavMeshTolerance, filter);
            
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(-Extents.x, 0, -Extents.z),
                out NavMeshHit _, NavMeshTolerance, filter);

            return isOnNavMesh;
        }
    }
}
