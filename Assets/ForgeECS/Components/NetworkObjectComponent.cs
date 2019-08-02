using System;
using Unity.Entities;

[Serializable]
public struct NetworkObjectComponent : IComponentData {
    //Fields
    public uint networkId;                                          //Associated NetworkId
    public BlittableBool isServer;                                  //isServer
    public BlittableBool isOwner;                                   //isOwner
}