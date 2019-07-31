using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using Unity.Entities;

namespace BeardedManStudios.Forge.Networking.Generated {

    [GeneratedInterpol("{\"inter\":[]")]
    public partial class ECSNetworkObject : NetworkObject {
        //Fields
        public const int IDENTITY = -5000;
        public Entity AttachedEntity { get; set; } = Entity.Null;

        public override int UniqueIdentity { get { return IDENTITY; } }

        protected override void ReadDirtyFields (BMSByte data, ulong timestep) {
            throw new System.NotImplementedException();
        }

        protected override void ReadPayload (BMSByte payload, ulong timestep) {
            throw new System.NotImplementedException();
 
        }

        protected override BMSByte SerializeDirtyFields () {
            throw new System.NotImplementedException();
        }

        protected override BMSByte WritePayload (BMSByte data) {
            throw new System.NotImplementedException();
        }

        private void Initialize () {
            if (readDirtyFlags == null) {
                readDirtyFlags = new byte[0];
            }

            //Register Default System RPCS

            //Register our RPCS
        }

        public ECSNetworkObject () : base() { Initialize(); }
        public ECSNetworkObject (NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
        public ECSNetworkObject (NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }
    }
}