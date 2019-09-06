using System;
using Unity.Entities;

[Serializable]
public struct NetworkObjectCreateComponent : IComponentData {
    //Fields
    public int identity;                //Identity of the NetworkObject (see NetworkObject.IDENTITY)
    public int createCode;
}