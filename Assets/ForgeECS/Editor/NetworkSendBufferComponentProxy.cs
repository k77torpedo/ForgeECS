using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[AddComponentMenu("DOTS/NetworkSendBufferComponent")]
public class NetworkSendBufferComponentProxy : DynamicBufferProxy<NetworkSendBufferComponent> {
}

