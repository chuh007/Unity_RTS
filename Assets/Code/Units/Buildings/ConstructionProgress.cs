using System;
using UnityEngine;

namespace Code.Units.Buildings
{
    public enum BuildingState
    {
        Ghost, Constructing, Paused, Completed, Destoryed
    }
    [Serializable]
    public struct ConstructionProgress
    {
        [field: SerializeField] public float StartTime { get; private set; }
        [field: SerializeField] public float Progress { get; private set; }
        [field: SerializeField] public BuildingState State { get; private set; }

        public ConstructionProgress(BuildingState state, float startTime, float progress)
        {
            StartTime = startTime;
            Progress = progress;
            State = state;
        }
    }
}