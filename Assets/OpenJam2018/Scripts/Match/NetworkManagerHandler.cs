using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace OpenJam2018
{
    public class NetworkManagerHandler : NetworkManager
    {
        public static NetworkManagerHandler instance;

        public MatchInfo m_MatchInfo;

        void Awake()
        {
            instance = this;
        }
        void OnApplicationQuit()
        {
            if (m_MatchInfo != null)
            {
                matchMaker.DestroyMatch(m_MatchInfo.networkId, m_MatchInfo.domain, (bool success, string extendedInfo) => { });
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("OnClientConnect: " + conn);
        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("OnClientDisconnect: " + conn);
        }
        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("OnServerConnect: " + conn);
        }
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Debug.Log("OnServerDisconnect: " + conn);
            NetworkServer.DestroyPlayersForConnection(conn);
        }


        public static GameObject FindLocalObject(NetworkInstanceId netId)
        {
            if (NetworkServer.active)
                return NetworkServer.FindLocalObject(netId);
            else
                return ClientScene.FindLocalObject(netId);
        }
    }
}
