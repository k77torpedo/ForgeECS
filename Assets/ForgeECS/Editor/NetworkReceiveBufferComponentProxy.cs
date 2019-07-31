using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[AddComponentMenu("DOTS/NetworkReceiveBufferComponent")]
public class NetworkReceiveBufferComponentProxy : DynamicBufferProxy<NetworkReceiveBufferComponent> {
}

