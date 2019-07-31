using System;
using Unity.Entities;

[Serializable]
public struct NetworkObjectComponent : IComponentData {
    //Fields
    public uint networkId;                                          //Associated NetworkId
    public BlittableBool destroyed;                                 //Flag for destroying via the NetworkObjectDestroySystem
    public BlittableBool isServer;                                  //isServer
    public BlittableBool isOwner;                                   //isOwner
}