using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace OpenJam2018
{

    public enum GameMode
    {
        Local,
        Online,
    }
    public class Game : NetworkBehaviour
    {

        public static Game instance;
        public static GameMode mode = GameMode.Local;

        public Character archer, swordsman, bossArcher, bossSwordsman;
        public BoxCollider playerSpawnArea;
        public BoxCollider[] groundSpawnAreas = new BoxCollider[0];
        public BoxCollider[] bowSpawnAreas = new BoxCollider[0];
        public Text playerRemainText, enemyRemainText, gameOverText;
        public Button quitButton;
        public GameObject ghost;

        [SyncVar(hook = "HookPlayerRemain")] public int playerRemain;
        [SyncVar(hook = "HookEnemyRemain")] public int enemyRemain;

        int m_ArcherCount = 10;
        int m_SwordsmanCount = 50;
        float m_ArcherPerTime = 0.5f;
        float m_SwordsmanPerTime = 0.2f;

        public override void OnStartServer()
        {
            if (mode == GameMode.Local)
            {
                playerRemain = 5;
                enemyRemain = 500;
            }
            else
            {
                playerRemain = 100;
                enemyRemain = 5000;
            }
            UpdatePlayerRemain();
            UpdateEnemyRemain();

            StartCoroutine(StartSpawnArcher());
            StartCoroutine(StartSpawnSwordsman());
        }
        public override void OnStartClient()
        {
            UpdatePlayerRemain();
            UpdateEnemyRemain();
        }

        public Vector3 RandomPlayerPosition()
        {
            return RandomPosition(new BoxCollider[] { playerSpawnArea });
        }
        public void CheckGameOver()
        {
            if (Character.enemyTeam.Count + Character.enemyBowTeam.Count + enemyRemain == 0)
                StartCoroutine(WaitToGameOver(true));
            else if (Character.playerTeam.Count + playerRemain == 0)
                StartCoroutine(WaitToGameOver(false));
        }
        IEnumerator WaitToGameOver(bool win)
        {
            yield return new WaitForSeconds(5);
            CmdGameOver(win);
        }
        [Command]
        public void CmdCreateGhost(Vector3 position)
        {
            RpcShowGhost(position);
        }
        [Command]
        public void CmdGameOver(bool win)
        {
            RpcGameOver(win);
        }
        [ClientRpc]
        public void RpcShowGhost(Vector3 position)
        {
            Instantiate(ghost, position, Quaternion.identity);
        }
        [ClientRpc]
        public void RpcGameOver(bool win)
        {
            gameOverText.text = win ? "You Win!" : "You Lose";
            gameOverText.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
        }
        public void OnClickQuit()
        {
            Application.Quit();
        }

        IEnumerator StartSpawnArcher()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_ArcherPerTime);

                if (Character.enemyBowTeam.Count > m_ArcherCount)
                    continue;

                CreateEnemy(archer, RandomPosition(bowSpawnAreas));
            }
        }
        IEnumerator StartSpawnSwordsman()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_SwordsmanPerTime);

                if (Character.enemyTeam.Count > m_SwordsmanCount)
                    continue;

                CreateEnemy(swordsman, RandomPosition(groundSpawnAreas));
            }
        }
        Vector3 RandomPosition(BoxCollider[] areas)
        {
            BoxCollider box = areas[Random.Range(0, areas.Length)];
            Vector3 min = box.transform.position + box.center;
            Vector3 max = box.transform.position + box.center;
            min -= box.size * 0.5f;
            max += box.size * 0.5f;

            Vector3 pos = new Vector3();
            pos.x = Random.Range(min.x, max.x);
            pos.y = Random.Range(min.y, max.y);
            pos.z = Random.Range(min.z, max.z);
            return pos;
        }
        void CreateEnemy(Character enemy, Vector3 position)
        {
            if (enemyRemain <= 0)
                return;
            enemyRemain--;

            Character character = Instantiate(enemy, position, Quaternion.identity);
            character.team = GameTeam.Enemy;
            NetworkServer.Spawn(character.gameObject);
        }
        void Awake()
        {
            instance = this;

            gameOverText.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
        }
        void HookPlayerRemain(int remain)
        {
            playerRemain = remain;
            UpdatePlayerRemain();
        }
        void HookEnemyRemain(int remain)
        {
            enemyRemain = remain;
            UpdateEnemyRemain();
        }
        void UpdatePlayerRemain()
        {
            playerRemainText.text = playerRemain.ToString();
        }
        void UpdateEnemyRemain()
        {
            enemyRemainText.text = enemyRemain.ToString();
        }
    }
}
