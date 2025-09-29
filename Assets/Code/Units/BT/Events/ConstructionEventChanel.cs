using System;
using Code.Units.Buildings;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

#if UNITY_EDITOR
namespace Code.Units.BT.Events
{
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Construction event chanel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Construction event chanel", message: "[Unit] [ConstructionEventType] on [ConstructionDummy]", category: "Events", id: "f553a997739f7a55fccc9fcaf40eb8f3")]
    public sealed partial class ConstructionEventChanel : EventChannel<AbstractUnit, ConstructionEventType, ConstructionDummy> { }
}

