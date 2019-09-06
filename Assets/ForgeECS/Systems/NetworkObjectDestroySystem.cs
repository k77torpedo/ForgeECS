using UnityEngine;
using Unity.Entities;
using BeardedManStudios.Forge.Networking;

/// <summary>
/// This system enables an <see cref="Entity"/> to destroy itself and its <see cref="NetworkObject"/> by 
/// simply adding a <see cref="NetworkObjectDestroyComponent"/> to itself.
/// </summary>
public class NetworkObjectDestroySystem : ComponentSystem {
    //Functions
    protected override void OnUpdate () {
        if (!ECSNetworkManager.IsInitialized || ECSNetworkManager.Instance.Networker == null) {
            return;
        }

        Entities.ForEach((Entity pEntity, ref NetworkObjectComponent pNetworkObjectComponent, ref NetworkObjectDestroyComponent pNetworkObjectDestroyComponent) => {
            //Destroy the NetworkObject
            NetworkObject nObj;
            if (ECSNetworkManager.Instance.Networker.NetworkObjects.TryGetValue(pNetworkObjectComponent.networkId, out nObj)) {
                nObj.Destroy();
                Debug.Log("N-Destroy!");
            } else {
                //There is no NetworkObject with a valid NetworkId present so we simply destroy the Entity.
                EntityManager.DestroyEntity(pEntity);
                Debug.Log("Destroy!");
            }
        });
    }
}
