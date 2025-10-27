using System;
using Code.Environments;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Code.Units.BT.Events
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/SupplyEventChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "SupplyEventChannel", message: "[Unit] gather [Amount] [SupplyType]", category: "Events", id: "a2bb4ea1aa963e7ab9833a7ae2911c8d")]
    public sealed partial class SupplyEventChannel : EventChannel<AbstractUnit, int, SupplySO> { }
}

