using Unity.Entities;
using BeardedManStudios.Forge.Networking;

/// <summary>
/// This system enables an <see cref="Entity"/> to create a <see cref="NetworkObject"/> by 
/// simply adding a <see cref="NetworkObjectCreateComponent"/> to itself.
/// </summary>
public class NetworkObjectCreateSystem : ComponentSystem {
    //Functions
    protected override void OnUpdate () {
        if (!ECSNetworkManager.IsInitialized || ECSNetworkManager.Instance.Networker == null) {
            return;
        }

        Entities.ForEach((Entity pEntity, ref NetworkObjectCreateComponent pNetworkObjectCreateComponent) => {
            //We remove the NetworkObjectCreateComponent as we no longer need it
            EntityManager.RemoveComponent<NetworkObjectCreateComponent>(pEntity);

            //Create a NetworkObject
            ECSNetworkManager.Instance.InstantiateNetworkObject(pNetworkObjectCreateComponent.identity, pNetworkObjectCreateComponent.createCode, null);
        });
    }
}
