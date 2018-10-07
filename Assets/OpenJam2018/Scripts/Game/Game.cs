using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Game : NetworkBehaviour
    {

        public Character archer, swordsman;
        public BoxCollider playerSpawnArea;
        public BoxCollider[] groundSpawnAreas = new BoxCollider[0];
        public BoxCollider[] bowSpawnAreas = new BoxCollider[0];

        int m_ArcherCount = 5;
        int m_SwordsmanCount = 10;
        float m_ArcherPerTime = 1;
        float m_SwordsmanPerTime = 1;

        public override void OnStartServer()
        {
            StartCoroutine(StartSpawnArcher());
            StartCoroutine(StartSpawnSwordsman());
        }

        public Vector3 RandomPlayerPosition()
        {
            return RandomPosition(new BoxCollider[] { playerSpawnArea });
        }

        IEnumerator StartSpawnArcher()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_ArcherPerTime);

                if (Character.enemyBowTeam.Count > m_ArcherCount)
                    continue;

                Character character = Instantiate(archer, RandomPosition(bowSpawnAreas), Quaternion.identity);
                character.team = GameTeam.Enemy;
                NetworkServer.Spawn(character.gameObject);
            }
        }
        IEnumerator StartSpawnSwordsman()
        {
            while (true)
            {
                yield return new WaitForSeconds(m_SwordsmanPerTime);

                if (Character.enemyTeam.Count > m_SwordsmanCount)
                    continue;

                Character character = Instantiate(swordsman, RandomPosition(groundSpawnAreas), Quaternion.identity);
                character.team = GameTeam.Enemy;
                NetworkServer.Spawn(character.gameObject);
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
    }
}
