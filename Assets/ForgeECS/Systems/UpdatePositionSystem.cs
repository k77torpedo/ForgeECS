using Unity.Entities;
using BeardedManStudios.Forge.Networking;

public class UpdatePositionSystem : ComponentSystem {
    //Functions
    protected override void OnUpdate () {
        if (!ECSNetworkManager.IsInitialized || ECSNetworkManager.Instance.Networker == null) {
            return;
        }

        Entities.ForEach((Entity pEntity, UpdatePositionBufferComponent pUpdatePosition) => {
            ////We remove the NetworkObjectCreateComponent as we no longer need it
            //EntityManager.RemoveComponent<NetworkObjectCreateComponent>(pEntity);

            ////Create a NetworkObject
            //ECSNetworkManager.Instance.InstantiateNetworkObject(pNetworkObjectCreateComponent.identity, pNetworkObjectCreateComponent.createCode, null);
        });
    }
}
