using System;
using BeardedManStudios.Forge.Networking.Frame;
using Unity.Entities;

namespace BeardedManStudios.Forge.Networking.Generated {

    [GeneratedInterpol("{\"inter\":[]")]
    public partial class ECSNetworkObject : NetworkObject, IECSNetworkObject {
        //Fields
        public const int IDENTITY = 5000;
        public const int RPC_GENERIC_MESSAGE = 0 + 4;
        private byte[] _dirtyFields = new byte[0];
        public Entity AttachedEntity { get; set; } = Entity.Null;
        public ECSNetworkManager AttachedManager { get; set; }

        public override int UniqueIdentity { get { return IDENTITY; } }


        //Functions
        protected override void ReadPayload (BMSByte pPayload, ulong pTimestep) {
        }

        protected override BMSByte WritePayload (BMSByte pData) {
            return pData;
        }

        protected override void ReadDirtyFields (BMSByte pData, ulong pTimestep) {
            if (readDirtyFlags == null)
                Initialize();

            Buffer.BlockCopy(pData.byteArr, pData.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
            pData.MoveStartIndex(readDirtyFlags.Length);
        }

        protected override BMSByte SerializeDirtyFields () {
            dirtyFieldsData.Clear();
            dirtyFieldsData.Append(_dirtyFields);

            // Reset all the dirty fields
            for (int i = 0; i < _dirtyFields.Length; i++)
                _dirtyFields[i] = 0;

            return dirtyFieldsData;
        }

        private void OnReceiveGenericMessage (RpcArgs pArgs) {
            if (AttachedEntity == Entity.Null || AttachedManager == null || AttachedManager.EntityManager == null) {
                return;
            }

            //Convert our data into a GenericMessage
            GenericMessage msg = new GenericMessage() {
                genericNumber = pArgs.GetNext<int>()
            };

            //Get the ReceiveBuffer from our Entity
            DynamicBuffer<GenericMessageReceiveBufferComponent> buffer = AttachedManager.EntityManager.GetBuffer<GenericMessageReceiveBufferComponent>(AttachedEntity);
            if (!buffer.IsCreated) {
                return;
            }

            buffer.Add(new GenericMessageReceiveBufferComponent() { Value = msg });
        }

        private void Initialize () {
            if (readDirtyFlags == null) {
                readDirtyFlags = new byte[0];
            }

            //Register our RPCS
            RegisterRpc("GenericMessage", OnReceiveGenericMessage, typeof(int));

            //Setup events
            ownershipChanged += UpdateNetworkObjectComponent;
            onDestroy += DestroyAttachedEntity;
        }

        private void UpdateNetworkObjectComponent (NetWorker sender) {
            UnityEngine.Debug.Log("OWNERSHIP CHANGED!!!!");
            //Update or set the NetworkObjectComponent of our Entity
            if (AttachedEntity == Entity.Null || AttachedManager == null || AttachedManager.EntityManager == null) {
                return;
            }

            AttachedManager.EntityManager.SetComponentData<NetworkObjectComponent>(
                AttachedEntity,
                new NetworkObjectComponent() {
                    networkId = NetworkId,
                    identity = UniqueIdentity,
                    isOwner = IsOwner,
                    isServer = IsServer
                });
        }

        private void DestroyAttachedEntity (NetWorker pSender) {
            //Always destroy our Entity with the NetworkObject
            if (AttachedEntity == Entity.Null || AttachedManager == null || AttachedManager.EntityManager == null) {
                return;
            }

            AttachedManager.EntityManager.DestroyEntity(AttachedEntity);
        }

        public ECSNetworkObject () : base() { Initialize(); }
        public ECSNetworkObject (NetWorker pNetworker, INetworkBehavior pNetworkBehavior = null, int pCreateCode = 0, byte[] pMetadata = null) : base(pNetworker, pNetworkBehavior, pCreateCode, pMetadata) { Initialize(); }
        public ECSNetworkObject (NetWorker pNetworker, uint pServerId, FrameStream pFrame) : base(pNetworker, pServerId, pFrame) { Initialize(); }
    }
}