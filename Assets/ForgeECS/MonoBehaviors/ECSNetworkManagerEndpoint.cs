using System;
using UnityEngine;

[Serializable]
public class ECSNetworkManagerEndpoint {
    //Fields
    [SerializeField] string         _host;
    [SerializeField] ushort         _port;

    public string                   Host { get { return _host; } set { _host = value; } }
    public ushort                   Port { get { return _port; } set { _port = value; } }


    //Functions
    public ECSNetworkManagerEndpoint () { }

    public ECSNetworkManagerEndpoint (ECSNetworkManagerEndpoint pEndpoint) {
        _host = pEndpoint.Host;
        _port = pEndpoint.Port;
    }

    public ECSNetworkManagerEndpoint (string pHost, ushort pPort) {
        _host = pHost;
        _port = pPort;
    }
}
