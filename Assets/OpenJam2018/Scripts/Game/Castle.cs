﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Castle : NetworkBehaviour
    {

        public Character archer, swordsman;
        public BoxCollider[] groundSpawnAreas = new BoxCollider[0];
        public Transform archerStartPositionMin, archerStartPositionMax;
        public int archerCount = 5;
        public int warriorCount = 10;
        public float archerPerTime = 1;
        public float warriorPerTime = 1;

        List<Character> m_Archers = new List<Character>();
        List<Character> m_Swordsmans = new List<Character>();

        public override void OnStartServer()
        {
            // StartCoroutine(StartSpawnArcher());
            StartCoroutine(StartSpawnSwordsman());
        }

        IEnumerator StartSpawnArcher()
        {
            while (true)
            {
                yield return new WaitForSeconds(archerPerTime);

                if (m_Archers.Count > archerCount)
                    continue;

                Character character = Instantiate(archer);
                float randomX = Random.Range(archerStartPositionMin.position.x, archerStartPositionMax.position.x);
                float randomY = Random.Range(archerStartPositionMin.position.y, archerStartPositionMax.position.y);
                character.transform.position = new Vector2(randomX, randomY);
                m_Archers.Add(character);
            }
        }
        IEnumerator StartSpawnSwordsman()
        {
            while (true)
            {
                yield return new WaitForSeconds(warriorPerTime);

                if (m_Swordsmans.Count > warriorCount)
                    continue;

                Character character = Instantiate(swordsman, RandomPosition(groundSpawnAreas), Quaternion.identity);
                character.faceLeft = true;
                NetworkServer.Spawn(character.gameObject);
                m_Swordsmans.Add(character);
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