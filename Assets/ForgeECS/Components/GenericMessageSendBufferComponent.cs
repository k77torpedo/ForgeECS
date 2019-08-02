using System;
using Unity.Entities;

[Serializable]
[InternalBufferCapacity(16)]
public struct GenericMessageSendBufferComponent : IBufferElementData {
    //Fields
    public GenericMessage Value;

    //Functions
    public static implicit operator GenericMessage (GenericMessageSendBufferComponent e) { return e.Value; }
    public static implicit operator GenericMessageSendBufferComponent (GenericMessage e) { return new GenericMessageSendBufferComponent { Value = e }; }
}
