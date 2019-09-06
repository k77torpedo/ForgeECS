using System;
using Unity.Entities;

[Serializable]
[InternalBufferCapacity(32)]
public struct UpdateRotationBufferComponent : IBufferElementData {
    //Fields
    public UpdateRotation Value;


    //Functions
    public static implicit operator UpdateRotation (UpdateRotationBufferComponent e) { return e.Value; }
    public static implicit operator UpdateRotationBufferComponent (UpdateRotation e) { return new UpdateRotationBufferComponent { Value = e }; }
}