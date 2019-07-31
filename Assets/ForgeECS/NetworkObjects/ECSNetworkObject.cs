using Unity.Entities;

namespace BeardedManStudios.Forge.Networking.Generated {

    public partial class ECSNetworkObject : NetworkObject {
        //Fields
        public Entity AttachedEntity { get; set; } = Entity.Null;
    }
}