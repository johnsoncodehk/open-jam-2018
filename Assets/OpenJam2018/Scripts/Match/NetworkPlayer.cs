using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class NetworkPlayer : NetworkBehaviour
    {

        [SyncVar(hook = "characterNetIdHook")]
        public uint characterNetId = NetworkInstanceId.Invalid.Value;

        Character m_Character;

        public override void OnStartServer()
        {
            GameObject character = NetworkManagerHandler.instance.spawnPrefabs.Find(prefab => prefab.name == "Swordsman");
            character = Instantiate(character);
            NetworkServer.SpawnWithClientAuthority(character, connectionToClient);

            characterNetId = character.GetComponent<Character>().netId.Value;
        }
        public override void OnStartClient()
        {
            UpdateCharacter();
        }

        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (m_Character)
            {
                m_Character.moveRaw = Input.GetAxisRaw("Horizontal");
                m_Character.LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (Input.GetButtonDown("Fire1"))
                    m_Character.Attack();
            }
        }

        void characterNetIdHook(uint netId)
        {
            characterNetId = netId;
            UpdateCharacter();
        }
        void UpdateCharacter()
        {
            if (characterNetId != NetworkInstanceId.Invalid.Value)
                m_Character = NetworkManagerHandler.FindLocalObject(new NetworkInstanceId(characterNetId)).GetComponent<Character>();
            else
                m_Character = null;
        }
    }
}
