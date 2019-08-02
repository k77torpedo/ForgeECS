using System;
using Unity.Entities;

[Serializable]
[InternalBufferCapacity(16)]
public struct GenericMessageReceiveBufferComponent : IBufferElementData {
    //Fields
    public GenericMessage Value;

    //Functions
    public static implicit operator GenericMessage (GenericMessageReceiveBufferComponent e) { return e.Value; }
    public static implicit operator GenericMessageReceiveBufferComponent (GenericMessage e) { return new GenericMessageReceiveBufferComponent { Value = e }; }
}
