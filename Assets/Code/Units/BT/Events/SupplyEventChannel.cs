using System;
using Code.Environments;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

#if UNITY_EDITOR
namespace Code.Units.BT.Events
{
    [CreateAssetMenu(menuName = "Behavior/Event Channels/SupplyEventChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "SupplyEventChannel", message: "[Unit] gather [Amount] [SupplyType]", category: "Events", id: "4fc260d5e4c43e66373131340fa47ba7")]
    public sealed partial class SupplyEventChannel : EventChannel<AbstractUnit, int, SupplySO> { }
}

