using System;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

/// <summary>
/// A <see cref="NetworkObjectFactoryBase"/> that allows further <see cref="NetworkObject"/>s to be registered during runtime by
/// adding functions that call the respective <see cref="NetworkObject"/>s constructor. Currently 2 standardized constructors are supported.
/// </summary>
public class DynamicNetworkObjectFactory : NetworkObjectFactoryBase {
    //Fields
    readonly object                                                                 _lockerConstructor1 = new object();
    readonly object                                                                 _lockerConstructor2 = new object();
    Dictionary<int, Func<NetWorker, uint, FrameStream, NetworkObject>>              _registeredNetworkObjectConstructors1 = new Dictionary<int, Func<NetWorker, uint, FrameStream, NetworkObject>>();
    Dictionary<int, Func<NetWorker, INetworkBehavior, int, byte[], NetworkObject>>  _registeredNetworkObjectConstructors2 = new Dictionary<int, Func<NetWorker, INetworkBehavior, int, byte[], NetworkObject>>();
    NetworkObjectFactory                                                            _forgeGeneratedFactory = new NetworkObjectFactory();


    //Functions
    public override void NetworkCreateObject (NetWorker pNetworker, int pIdentity, uint pId, FrameStream pFrame, Action<NetworkObject> pCallback) {
        Func<NetWorker, uint, FrameStream, NetworkObject> factoryFunc;
        bool success = false;
        lock (_lockerConstructor1) {
            success = _registeredNetworkObjectConstructors1.TryGetValue(pIdentity, out factoryFunc);
        }

        if (success) {
            if (pNetworker.IsServer) {
                if (pFrame.Sender != null && pFrame.Sender != pNetworker.Me) {
                    if (!ValidateCreateRequest(pNetworker, pIdentity, pId, pFrame)) {
                        return;
                    }
                }
            }
            
            MainThreadManager.Run(() => {
                NetworkObject obj = factoryFunc(pNetworker, pId, pFrame);
                if (obj != null && pCallback != null) {
                    pCallback(obj);
                }
            });
        } else {
            _forgeGeneratedFactory.NetworkCreateObject(pNetworker, pIdentity, pId, pFrame, pCallback);
        }
    }

    public void RegisterConstructor (int pIdentity, Func<NetWorker, uint, FrameStream, NetworkObject> pFunc) {
        if (pFunc == null) {
            return;
        }

        lock (_lockerConstructor1) {
            if (_registeredNetworkObjectConstructors1.ContainsKey(pIdentity)) {
                _registeredNetworkObjectConstructors1[pIdentity] = pFunc;
            } else {
                _registeredNetworkObjectConstructors1.Add(pIdentity, pFunc);
            }
        }
    }

    public void RegisterConstructor (int pIdentity, Func<NetWorker, INetworkBehavior, int, byte[], NetworkObject> pFunc) {
        if (pFunc == null) {
            return;
        }

        lock (_lockerConstructor2) {
            if (_registeredNetworkObjectConstructors2.ContainsKey(pIdentity)) {
                _registeredNetworkObjectConstructors2[pIdentity] = pFunc;
            } else {
                _registeredNetworkObjectConstructors2.Add(pIdentity, pFunc);
            }
        }
    }

    public NetworkObject NetworkCreateObject (int pIdentity, NetWorker pNetworker, uint pId, FrameStream pFrame) {
        Func<NetWorker, uint, FrameStream, NetworkObject> factoryFunc;
        bool success = false;
        lock (_lockerConstructor1) {
            success = _registeredNetworkObjectConstructors1.TryGetValue(pIdentity, out factoryFunc);
        }

        if (success) {
            return factoryFunc(pNetworker, pId, pFrame);
        }

        return default(NetworkObject);
    }

    public NetworkObject NetworkCreateObject (int pIdentity, NetWorker pNetworker, INetworkBehavior pNetworkBehavior, int pCreateCode, byte[] pMetadata) {
        Func<NetWorker, INetworkBehavior, int, byte[], NetworkObject> factoryFunc;
        bool success = false;
        lock (_lockerConstructor2) {
            success = _registeredNetworkObjectConstructors2.TryGetValue(pIdentity, out factoryFunc);
        }

        if (success) {
            return factoryFunc(pNetworker, pNetworkBehavior, pCreateCode, pMetadata);
        }

        return default(NetworkObject);
    }
}
