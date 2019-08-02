using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[AddComponentMenu("DOTS/NetworkSendBufferComponent")]
public class GenericMessageSendBufferComponentProxy : DynamicBufferProxy<GenericMessageSendBufferComponent> {
}

