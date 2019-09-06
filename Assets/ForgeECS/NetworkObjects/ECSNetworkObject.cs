using System;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Frame;
using Unity.Entities;
using Unity.Mathematics;

namespace BeardedManStudios.Forge.Networking.Generated {

    [GeneratedInterpol("{\"inter\":[]")]
    public partial class ECSNetworkObject : NetworkObject, IECSNetworkObject {
        //Fields
        public const int IDENTITY = 5000;
        public const int RPC_UPDATE_POSITION = 0 + 4;
        public const int RPC_UPDATE_ROTATION = 1 + 4;

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

        private void Initialize () {
            if (readDirtyFlags == null) {
                readDirtyFlags = new byte[0];
            }

            //Register our custom RPCs
            RegisterRPCs();

            //Setup events
            ownershipChanged += UpdateAttachedEntity;
            onDestroy += DestroyAttachedEntity;
        }

        public void RegisterComponents () {
            AttachedManager.EntityManager.AddBuffer<UpdatePositionBufferComponent>(AttachedEntity);
            AttachedManager.EntityManager.AddBuffer<UpdateRotationBufferComponent>(AttachedEntity);
        }

        private void RegisterRPCs () {
            RegisterRpc("UpdatePosition", OnUpdatePosition, typeof(float), typeof(float), typeof(float));
            RegisterRpc("UpdateRotation", OnUpdateRotation, typeof(float), typeof(float), typeof(float), typeof(float));
        }

        private void OnUpdatePosition (RpcArgs pArgs) {
            if (AttachedEntity == Entity.Null || AttachedManager == null || AttachedManager.EntityManager == null) {
                return;
            }

            //Read the position-data to update from the stream
            UpdatePosition updatePosition = new UpdatePosition() {
                send = false,
                position = new float3(pArgs.GetNext<float>(), pArgs.GetNext<float>(), pArgs.GetNext<float>())
            };

            //Get the Buffer from our Entity
            DynamicBuffer<UpdatePositionBufferComponent> buffer = AttachedManager.EntityManager.GetBuffer<UpdatePositionBufferComponent>(AttachedEntity);

            //Add the UpdatePositionBufferComponent so it can be used by the appropriate system
            buffer.Add(new UpdatePositionBufferComponent() { Value = updatePosition });
        }

        private void OnUpdateRotation (RpcArgs pArgs) {
            if (AttachedEntity == Entity.Null || AttachedManager == null || AttachedManager.EntityManager == null) {
                return;
            }

            //Read the position-data to update from the stream
            UpdateRotation updateRotation = new UpdateRotation() {
                send = false,
                rotation = new float4(pArgs.GetNext<float>(), pArgs.GetNext<float>(), pArgs.GetNext<float>(), pArgs.GetNext<float>())
            };

            //Get the ReceiveBuffer from our Entity
            DynamicBuffer<UpdateRotationBufferComponent> buffer = AttachedManager.EntityManager.GetBuffer<UpdateRotationBufferComponent>(AttachedEntity);

            //Add the UpdateRotationBufferComponent so it can be used by the appropriate system
            buffer.Add(new UpdateRotationBufferComponent() { Value = updateRotation });
        }

        private void UpdateAttachedEntity (NetWorker sender) {
            //Update or set the NetworkObjectComponent of our Entity
            if (AttachedEntity == Entity.Null || AttachedManager == null || AttachedManager.EntityManager == null) {
                return;
            }

            AttachedManager.EntityManager.SetComponentData(
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