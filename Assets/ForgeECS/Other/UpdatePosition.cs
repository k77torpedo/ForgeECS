using System;
using Unity.Mathematics;

[Serializable]
public struct UpdatePosition {
    //Fields
    public bool send;                   // Determines if this should be send or received
    public float3 position;
}
