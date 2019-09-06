using System;
using Unity.Mathematics;

[Serializable]
public struct UpdateRotation {
    //Fields
    public bool send;                   // Determines if this should be send or received
    public float4 rotation;
}