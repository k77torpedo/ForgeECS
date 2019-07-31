using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class ECSNetworkManager : MonoBehaviour {
    //Fields
    [Header("ECS-NetworkObjects:"), Space(5f)]
    public GameObject[] ECSPrototypes;




    //Functions
    void Awake () {
        Initialize();
    }

    public void Initialize () {
        if (NetworkObject.Factory == null) {
            NetworkObject.Factory = new ECSNetworkObjectFactory();
        }
    }
}
