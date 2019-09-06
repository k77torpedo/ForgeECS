using System;
using Unity.Entities;

[Serializable]
[InternalBufferCapacity(32)]
public struct UpdatePositionBufferComponent : IBufferElementData {
    //Fields
    public UpdatePosition Value;


    //Functions
    public static implicit operator UpdatePosition (UpdatePositionBufferComponent e) { return e.Value; }
    public static implicit operator UpdatePositionBufferComponent (UpdatePosition e) { return new UpdatePositionBufferComponent { Value = e }; }
}
