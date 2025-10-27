using System;
using Code.Units.Buildings;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Code.Units.BT.Events
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Construction event channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Construction event channel", message: "[Unit] [ConstructionEventType] on [ConstructionDummy]", category: "Events", id: "af959130800dc3bccf399555b0b84e84")]
    public sealed partial class ConstructionEventChannel : EventChannel<AbstractUnit, ConstructionEventType, ConstructionDummy> { }
}

