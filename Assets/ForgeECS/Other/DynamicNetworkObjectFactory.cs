using System;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

/// <summary>
/// A <see cref="NetworkObjectFactoryBase"/> that allows further <see cref="NetworkObject"/>s to be registered during runtime.
/// </summary>
public class DynamicNetworkObjectFactory : NetworkObjectFactoryBase {
    //Fields
    readonly object                             _locker = new object();
    Dictionary<int, FactoryFunction>            _registeredFactories = new Dictionary<int, FactoryFunction>();
    NetworkObjectFactory                        _forgeGeneratedFactory = new NetworkObjectFactory();

    public delegate NetworkObject               FactoryFunction (NetWorker pNetworker, uint pId, FrameStream pFrame);


    //Functions
    public override void NetworkCreateObject (NetWorker networker, int identity, uint id, FrameStream frame, Action<NetworkObject> callback) {
        FactoryFunction factoryFunc;
        bool success = false;
        lock (_locker) {
            success = _registeredFactories.TryGetValue(identity, out factoryFunc);
        }

        if (success) {
            if (networker.IsServer) {
                if (frame.Sender != null && frame.Sender != networker.Me) {
                    if (!ValidateCreateRequest(networker, identity, id, frame))
                        return;
                }
            }
            
            MainThreadManager.Run(() => {
                NetworkObject obj = factoryFunc(networker, id, frame);
                if (obj != null && callback != null) {
                    callback(obj);
                }
            });
        } else {
            _forgeGeneratedFactory.NetworkCreateObject(networker, identity, id, frame, callback);
        }
    }

    public void RegisterFactory (int pIdentity, FactoryFunction pFactoryFunction) {
        if (pFactoryFunction != null) {
            return;
        }

        lock (_locker) {
            if (_registeredFactories.ContainsKey(pIdentity)) {
                _registeredFactories[pIdentity] = pFactoryFunction;
            } else {
                _registeredFactories.Add(pIdentity, pFactoryFunction);
            }
        }
    }
}
