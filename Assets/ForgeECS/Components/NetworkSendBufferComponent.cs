using System;
using Unity.Entities;

[Serializable]
[InternalBufferCapacity(16)]
public struct NetworkSendBufferComponent : IBufferElementData {
    //Fields
    public BinaryMessage Value;

    //Functions
    public static implicit operator BinaryMessage (NetworkSendBufferComponent e) { return e.Value; }
    public static implicit operator NetworkSendBufferComponent (BinaryMessage e) { return new NetworkSendBufferComponent { Value = e }; }
}
