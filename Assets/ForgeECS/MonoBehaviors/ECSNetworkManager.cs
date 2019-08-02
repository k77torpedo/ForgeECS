using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;


public class ECSNetworkManager : MonoBehaviour {
    //Fields
    public const string DEFAULT_WORLD_NAME = "ForgeECS";
    [SerializeField] string worldName;
    [SerializeField] ECSNetworkManagerSetting _settings;
    [SerializeField] GameObject[] _prefabEntityArchetypes;

    World _world;
    EntityManager _entityManager;
    NetWorker _networker;
    BMSByte _tmpMetadata;
    DynamicNetworkObjectFactory _factory;

    public EntityManager EntityManager { get { return _entityManager; } }


    //Events
    public delegate void NetworkFailedToBindEvent (ECSNetworkManager pNetworkManager);
    public event NetworkFailedToBindEvent OnFailedToBind;
    public delegate void NetworkStartEvent (ECSNetworkManager pNetworkManager);
    public event NetworkStartEvent OnStart;


    //Functions
    #region Unity
    void Awake () {
        _tmpMetadata = new BMSByte();
    }

    //################## TEST!!!!!


    void OnGUI () {
        //if (_networker == null) {
        //    return;
        //}

        //string text = _networker.IsBound + "###" + _networker.IsConnected + "###" + _networker.IsServer + "\r\n" +
        //    _networker.NetworkObjects.Count + "###" + _networker.NetworkObjectList.Count + "\r\n" +
        //    "Players: " + _networker.Players.Count + "\r\n" +
        //    "";
        //GUI.Label(new Rect(5f, 5f, 800f, 800f), text);
    }

    public void InstantiateTest () {
        InstantiateNetworkObject(ECSNetworkObject.IDENTITY, 0);
    }

    public void DestroyTest () {
        foreach (var item in _networker.NetworkObjects.Values) {
            item.Destroy(50);
        }
    }

    public void TakeOwnershipTest () {
        foreach (var item in _networker.NetworkObjects.Values) {
            item.TakeOwnership();
        }
    }

    //################# END OF TEST!!!!!!!



    void OnDestroy () {
        if (_networker != null) {
            if (_networker.IsServer) {
                UnregisterEventsServer();
            } else {
                UnregisterEventsClient();
            }
        }

        Disconnect();
        if (_world != null) {
            _world.Dispose();
        }
    }

    void OnApplicationQuit () {
        Disconnect();
        NetWorker.EndSession();
        if (_world != null) {
            _world.Dispose();
        }
    }

    #endregion

    #region Start
    public void StartAsServer () {
        StartAsServer(_settings);
    }

    public void StartAsServer (ECSNetworkManagerSetting pSettings) {
        _settings = pSettings;
        Disconnect();
        InitializeDefaults();
        NetWorker server;
        if (_settings.UseTCP) {
            server = new TCPServer(_settings.MaxConnections);
            ((TCPServer)server).Connect(_settings.ServerAddress.Host, _settings.ServerAddress.Port);
        } else {
            server = new UDPServer(_settings.MaxConnections);
            if (_settings.ServerNATAddress == null || _settings.ServerNATAddress.Host == null || _settings.ServerNATAddress.Host.Trim().Length == 0) {
                ((UDPServer)server).Connect(_settings.ServerAddress.Host, _settings.ServerAddress.Port);
            } else {
                ((UDPServer)server).Connect(_settings.ServerAddress.Host, _settings.ServerAddress.Port, _settings.ServerNATAddress.Host, _settings.ServerNATAddress.Port);
            }
        }

        if (!server.IsBound) {
            RaiseFailedToBind(this);
            return;
        }

        UnregisterEventsServer();
        _networker = server;
        RegisterEventsServer();
        RaiseStart(this);
    }

    public void StartAsClient () {
        StartAsClient(_settings);
    }

    public void StartAsClient (ECSNetworkManagerSetting pSettings) {
        _settings = pSettings;
        Disconnect();
        InitializeDefaults();
        NetWorker client;
        if (_settings.UseTCP) {
            client = new TCPClient();
            ((TCPClient)client).Connect(_settings.ClientAddress.Host, _settings.ClientAddress.Port);
        } else {
            client = new UDPClient();
            if (_settings.ClientNATAddress == null || _settings.ClientNATAddress.Host == null || _settings.ClientNATAddress.Host.Trim().Length == 0) {
                ((UDPClient)client).Connect(_settings.ClientAddress.Host, _settings.ClientAddress.Port);
            } else {
                ((UDPClient)client).Connect(_settings.ClientAddress.Host, _settings.ClientAddress.Port, _settings.ClientNATAddress.Host, _settings.ClientNATAddress.Port);
            }
        }

        if (!client.IsBound) {
            RaiseFailedToBind(this);
            return;
        }

        UnregisterEventsClient();
        _networker = client;
        RegisterEventsClient();
        RaiseStart(this);
    }

    #endregion
    
    #region Disconnect
    public virtual void Disconnect () {
        if (_networker != null) {
            _networker.Disconnect(false);
            NetworkObject.ClearNetworkObjects(_networker);
            NetworkObject.Flush(_networker);
        }
    }

    #endregion

    #region Register Events
    protected virtual void RegisterEventsClient () {
        if (_networker != null) {
            _networker.objectCreated += FlushCreateActions;
            _networker.factoryObjectCreated += FlushCreateActions;
        }
    }

    protected virtual void UnregisterEventsClient () {
        if (_networker != null) {
            _networker.objectCreated -= FlushCreateActions;
            _networker.factoryObjectCreated -= FlushCreateActions;
        }
    }

    protected virtual void RegisterEventsServer () {
        if (_networker != null) {
            _networker.objectCreated += FlushCreateActions;
            _networker.factoryObjectCreated += FlushCreateActions;
        }
    }

    protected virtual void UnregisterEventsServer () {
        if (_networker != null) {
            _networker.objectCreated -= FlushCreateActions;
            _networker.factoryObjectCreated -= FlushCreateActions;
        }
    }

    #endregion

    #region Events
    void RaiseFailedToBind (ECSNetworkManager pNetworkManager) {
        if (OnFailedToBind != null) {
            OnFailedToBind(pNetworkManager);
        }
    }

    void RaiseStart(ECSNetworkManager pNetworkManager) {
        if (OnStart != null) {
            OnStart(pNetworkManager);
        }
    }

    void FlushCreateActions (NetworkObject pObj) {
        if (pObj.CreateCode < 0) {
            return;
        }

        MainThreadManager.Run(() => {
            if (pObj == null || pObj.Networker == null) {
                return;
            }

            CreateECSEntity(pObj);
            pObj.ReleaseCreateBuffer();
            pObj.Networker.FlushCreateActions(pObj);
        });
    }

    #endregion

    #region Helpers
    public void InitializeDefaults () {
        MainThreadManager.Create();
        if (!_settings.UseTCP) {
            NetWorker.PingForFirewall(_settings.ServerAddress.Port);
        }

        if (_settings.UseMainThreadManagerForRPCs && Rpc.MainThreadRunner == null) {
            Rpc.MainThreadRunner = MainThreadManager.Instance;
        }

        UnityObjectMapper.Instance.UseAsDefault();
        RegisterFactory();
        if (string.IsNullOrEmpty(worldName)) {
            _world = new World(DEFAULT_WORLD_NAME);
        } else {
            _world = new World(worldName);
        }

        _entityManager = _world.EntityManager;
    }

    public void SendFrame (FrameStream pFrame, NetworkingPlayer pTargetPlayer = null) {
        if (_networker is IServer) {
            if (pTargetPlayer != null) {
                if (_networker is TCPServer) {
                    ((TCPServer)_networker).SendToPlayer(pFrame, pTargetPlayer);
                } else {
                    ((UDPServer)_networker).Send(pTargetPlayer, pFrame, true);
                }
            } else {
                if (_networker is TCPServer) {
                    ((TCPServer)_networker).SendAll(pFrame);
                } else {
                    ((UDPServer)_networker).Send(pFrame, true);
                }
            }
        } else {
            if (_networker is TCPClientBase) {
                ((TCPClientBase)_networker).Send(pFrame);
            } else {
                ((UDPClient)_networker).Send(pFrame, true);
            }
        }
    }

    public void RegisterFactory () {
        DynamicNetworkObjectFactory currentFactory = NetworkObject.Factory as DynamicNetworkObjectFactory;
        if (currentFactory == null) {
            if (_factory == null) {
                _factory = new DynamicNetworkObjectFactory();
            }

            NetworkObject.Factory = _factory;
        } else {
            _factory = currentFactory;
        }

        //TODO: into Bootstrap
        //Register our NetworkObject-Types to the Factory so it can produce them.
        _factory.RegisterConstructor(
            ECSNetworkObject.IDENTITY,
            (NetWorker pNetworker, uint pId, FrameStream pFrame) => {       
                return new ECSNetworkObject(pNetworker, pId, pFrame);
            });

        _factory.RegisterConstructor(
            ECSNetworkObject.IDENTITY,
            (NetWorker pNetworker, INetworkBehavior pBehavior, int pCreateCode, byte[] pMetaData) => {
                return new ECSNetworkObject(pNetworker, pBehavior, pCreateCode, pMetaData);
            });
    }

    public NetworkObject InstantiateNetworkObject (int pIdentity, int pCreateCode, byte[] pMetaData = null) {
        if (_factory == null) {
            return null;
        }

        //Get a new NetworkObject from our Factory
        return _factory.NetworkCreateObject(pIdentity, _networker, null, pCreateCode, pMetaData);
    }

    public void CreateECSEntity (NetworkObject pObj) {
        if (_entityManager == null
            || pObj.CreateCode < 0
            || pObj.CreateCode >= _prefabEntityArchetypes.Length
            || _prefabEntityArchetypes[pObj.CreateCode] == null) {
            return;
        }

        //If the NetworkObject implements IECSNetworkObject an Entity can be created and be associated with it.
        IECSNetworkObject ecsObj = pObj as IECSNetworkObject;
        if (ecsObj != null) {
            //Note: this is currently a hack since essential functions are being kept internal as of 08/02/2019
            Entity entity = _entityManager.CreateEntity();
            GameObjectEntity.CopyAllComponentsToEntity(_prefabEntityArchetypes[pObj.CreateCode], _entityManager, entity);

            //Associate the NetworkObject with the Entity
            ecsObj.AttachedEntity = entity;
            ecsObj.AttachedManager = this;

            _entityManager.AddComponent<NetworkObjectComponent>(entity);
            _entityManager.SetComponentData<NetworkObjectComponent>(
                entity,
                new NetworkObjectComponent() {
                    networkId = pObj.NetworkId,
                    isOwner = pObj.IsOwner,
                    isServer = pObj.IsServer
                });
        }
    }

    #endregion
}
