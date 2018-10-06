using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class NetworkManagerHandler : NetworkManager
    {
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Debug.Log("OnClientConnect: " + conn);
        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            Debug.Log("OnClientDisconnect: " + conn);
        }
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            Debug.Log("OnServerConnect: " + conn);
        }
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log("OnServerDisconnect: " + conn);
        }
    }
}
