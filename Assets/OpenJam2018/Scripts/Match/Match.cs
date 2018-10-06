using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace OpenJam2018
{
    public class Match : MonoBehaviour
    {

        public GameObject searchingGame, noGameFound, creatingGame, gameCreated, gameCreateFailed, gameFound, joiningGame, gameJoinFailed, gameJoined;
        public Text gameInfo;

        NetworkManager m_NetworkManager;
        MatchInfoSnapshot m_MatchInfoSnapshot;

        void Awake()
        {
            m_NetworkManager = FindObjectOfType<NetworkManager>();
        }
        void Start()
        {
            m_NetworkManager.StartMatchMaker();
            SearchGame();
        }

        public void SearchGame()
        {
            ShowState(searchingGame);

            m_NetworkManager.matchMaker.ListMatches(0, 1, "", true, 0, 0, (bool success, string extendedInfo, List<MatchInfoSnapshot> matches) =>
            {
                if (matches.Count == 0)
                {
                    ShowState(noGameFound);
                    return;
                }
                m_MatchInfoSnapshot = matches[0];
                ShowState(gameFound);
                gameInfo.text = ""
                    + "\n" + "networkId: " + m_MatchInfoSnapshot.networkId
                    + "\n" + "size: " + m_MatchInfoSnapshot.currentSize + "/" + m_MatchInfoSnapshot.maxSize
                    + "\n" + "hostNodeId: " + m_MatchInfoSnapshot.hostNodeId
                    + "\n" + "networkId: " + m_MatchInfoSnapshot.networkId
                    + "\n" + "directConnectCount: " + m_MatchInfoSnapshot.directConnectInfos.Count
                ;
            });
        }
        public void CreateGame()
        {
            ShowState(creatingGame);

            m_NetworkManager.matchMaker.CreateMatch(name, 20, true, "", "", "", 0, 0, (bool success, string extendedInfo, MatchInfo matchInfo) =>
            {
                if (!success)
                {
                    ShowState(gameCreated);
                    return;
                }
                ShowState(gameCreated);
                m_NetworkManager.StartHost(matchInfo);
            });
        }
        public void JoinGame()
        {
            ShowState(joiningGame);

            m_NetworkManager.matchMaker.JoinMatch(m_MatchInfoSnapshot.networkId, "", "", "", 0, 0, (bool success, string extendedInfo, MatchInfo matchInfo) =>
            {
                if (!success)
                {
                    ShowState(gameJoinFailed);
                    return;
                }
                ShowState(gameJoined);
                m_NetworkManager.StartClient(matchInfo);
            });
        }

        void ShowState(GameObject state)
        {
            HideAllState();
            state.SetActive(true);
        }
        void HideAllState()
        {
            searchingGame.SetActive(false);
            noGameFound.SetActive(false);
            creatingGame.SetActive(false);
            gameCreated.SetActive(false);
            gameCreateFailed.SetActive(false);
            gameFound.SetActive(false);
            joiningGame.SetActive(false);
            gameJoinFailed.SetActive(false);
            gameJoined.SetActive(false);
        }
    }
}
