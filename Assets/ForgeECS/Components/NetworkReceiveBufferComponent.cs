using System;
using Unity.Entities;

[Serializable]
[InternalBufferCapacity(16)]
public struct NetworkReceiveBufferComponent : IBufferElementData {
    //Fields
    public BinaryMessage Value;

    //Functions
    public static implicit operator BinaryMessage (NetworkReceiveBufferComponent e) { return e.Value; }
    public static implicit operator NetworkReceiveBufferComponent (BinaryMessage e) { return new NetworkReceiveBufferComponent { Value = e }; }
}
