using System;
using UnityEngine;

[Serializable]
public class ECSNetworkManagerSetting {
    //Fields
    [SerializeField] int                                _maxConnections;
    [SerializeField] bool                               _useTCP;
    [SerializeField] bool                               _useMainThreadManagerForRPCs;
    [SerializeField] ECSNetworkManagerEndpoint          _serverAddress;
    [SerializeField] ECSNetworkManagerEndpoint          _serverNATAddress;
    [SerializeField] ECSNetworkManagerEndpoint          _clientAddress;
    [SerializeField] ECSNetworkManagerEndpoint          _clientNATAddress;

    public int                                          MaxConnections { get { return _maxConnections; } set { _maxConnections = value; } }
    public bool                                         UseTCP { get { return _useTCP; } set { _useTCP = value; } }
    public bool                                         UseMainThreadManagerForRPCs { get { return _useMainThreadManagerForRPCs; } set { _useMainThreadManagerForRPCs = value; } }
    public ECSNetworkManagerEndpoint                    ServerAddress { get { return _serverAddress; } set { _serverAddress = value; } }
    public ECSNetworkManagerEndpoint                    ServerNATAddress { get { return _serverNATAddress; } set { _serverNATAddress = value; } }
    public ECSNetworkManagerEndpoint                    ClientAddress { get { return _clientAddress; } set { _clientAddress = value; } }
    public ECSNetworkManagerEndpoint                    ClientNATAddress { get { return _clientNATAddress; } set { _clientNATAddress = value; } }


    //Functions
    public ECSNetworkManagerSetting () {
        _serverAddress = new ECSNetworkManagerEndpoint();
        _serverNATAddress = new ECSNetworkManagerEndpoint();
        _clientAddress = new ECSNetworkManagerEndpoint();
        _clientNATAddress = new ECSNetworkManagerEndpoint();
    }

    public ECSNetworkManagerSetting (ECSNetworkManagerSetting pSetting) {
        _maxConnections = pSetting.MaxConnections;
        _useTCP = pSetting.UseTCP;
        _useMainThreadManagerForRPCs = pSetting.UseMainThreadManagerForRPCs;
        _serverAddress = new ECSNetworkManagerEndpoint(pSetting.ServerAddress);
        _serverNATAddress = new ECSNetworkManagerEndpoint(pSetting.ServerNATAddress);
        _clientAddress = new ECSNetworkManagerEndpoint(pSetting.ClientAddress);
        _clientNATAddress = new ECSNetworkManagerEndpoint(pSetting.ClientNATAddress);
    }
}
