using UnityEngine;

namespace Code.Commands
{
    public interface IHasGhostPrefab
    {
        GameObject GhostPrefab { get; }
    }
}