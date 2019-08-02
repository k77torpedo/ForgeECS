using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[AddComponentMenu("DOTS/NetworkReceiveBufferComponent")]
public class GenericMessageReceiveBufferComponentProxy : DynamicBufferProxy<GenericMessageReceiveBufferComponent> {
}

