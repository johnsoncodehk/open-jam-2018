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

        public GameObject searchingGame, noGameFound, creatingGame, gameCreated, gameCreateFailed, gameFound, joiningGame, gameJoinFailed, gameJoined, notSupport;
        public Text gameInfo;
        public Dropdown serverDropdown;

        MatchInfoSnapshot m_MatchInfoSnapshot;

        void Start()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                ShowState(notSupport);
            else
                SearchGame();
        }

        public void LocalHost()
        {
            Game.mode = GameMode.Normal;
            NetworkManagerHandler.instance.StartHost();
        }
        public void InfiniteMode()
        {
            Game.mode = GameMode.Infinite;
            NetworkManagerHandler.instance.StartHost();
        }
        public void SearchGame()
        {
            SearchGame("eu1-mm.unet.unity3d.com", "Europe", () =>
            {
                SearchGame("us1-mm.unet.unity3d.com", "United States", () =>
                {
                    SearchGame("ap1-mm.unet.unity3d.com", "Singapore", () =>
                    { });
                });
            });
        }
        void SearchGame(string url, string name, System.Action onFail)
        {
            ShowState(searchingGame);

            NetworkManagerHandler.instance.matchHost = url;
            NetworkManagerHandler.instance.StartMatchMaker();
            NetworkManagerHandler.instance.matchMaker.ListMatches(0, 1, "", true, 0, 0, (bool success, string extendedInfo, List<MatchInfoSnapshot> matches) =>
            {
                if (matches.Count == 0)
                {
                    ShowState(noGameFound);
                    onFail();
                    return;
                }
                m_MatchInfoSnapshot = matches[0];
                ShowState(gameFound);
                gameInfo.text = "size: " + m_MatchInfoSnapshot.currentSize + "/" + m_MatchInfoSnapshot.maxSize + " (" + name + ")";
            });
        }
        public void CreateGame()
        {
            ShowState(creatingGame);

            switch (serverDropdown.value)
            {
                case 0: NetworkManagerHandler.instance.matchHost = "mm.unet.unity3d.com"; break;
                case 1: NetworkManagerHandler.instance.matchHost = "eu1-mm.unet.unity3d.com"; break;
                case 2: NetworkManagerHandler.instance.matchHost = "us1-mm.unet.unity3d.com"; break;
                case 3: NetworkManagerHandler.instance.matchHost = "ap1-mm.unet.unity3d.com"; break;
            }
            NetworkManagerHandler.instance.StartMatchMaker();
            NetworkManagerHandler.instance.matchMaker.CreateMatch(name, 20, true, "", "", "", 0, 0, (bool success, string extendedInfo, MatchInfo matchInfo) =>
            {
                if (!success)
                {
                    ShowState(gameCreateFailed);
                    return;
                }
                NetworkManagerHandler.instance.m_MatchInfo = matchInfo;
                ShowState(gameCreated);
                Game.mode = GameMode.Online;
                NetworkManagerHandler.instance.StartHost(matchInfo);
            });
        }
        public void JoinGame()
        {
            ShowState(joiningGame);

            NetworkManagerHandler.instance.matchMaker.JoinMatch(m_MatchInfoSnapshot.networkId, "", "", "", 0, 0, (bool success, string extendedInfo, MatchInfo matchInfo) =>
            {
                if (!success)
                {
                    ShowState(gameJoinFailed);
                    return;
                }
                ShowState(gameJoined);
                Game.mode = GameMode.Online;
                NetworkManagerHandler.instance.StartClient(matchInfo);
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
            notSupport.SetActive(false);
        }
    }
}
