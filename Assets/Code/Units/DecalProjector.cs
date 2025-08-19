using UnityEngine;

namespace Code.Units
{
    public class DecalProjector : MonoBehaviour
    {
        public void SetActiveDecal(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}