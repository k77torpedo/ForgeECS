using UnityEngine;
using Unity.Entities;

[DisallowMultipleComponent]
[AddComponentMenu("DOTS/NetworkObjectDestroyComponentProxy")]
public class NetworkObjectDestroyComponentProxy : ComponentDataProxy<NetworkObjectDestroyComponent> {
}
