using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Player : NetworkBehaviour
    {

        [SyncVar(hook = "CharacterNetIdHook")]
        public uint characterNetId = NetworkInstanceId.Invalid.Value;

        Character m_Character;

        public override void OnStartServer()
        {
            StartCoroutine(WaitGameSceneReady());
        }
        public override void OnStartClient()
        {
            UpdateCharacter();
        }
        IEnumerator WaitGameSceneReady()
        {
            while (!FindObjectOfType<Castle>())
                yield return new WaitForEndOfFrame();

            GameObject character = Instantiate(NetworkManagerHandler.instance.spawnPrefabs[1], FindObjectOfType<Castle>().RandomPlayerPosition(), Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(character.gameObject, connectionToClient);

            characterNetId = character.GetComponent<Character>().netId.Value;
        }

        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (m_Character)
            {
                if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
                    m_Character.CmdSetMoveRawX(Input.GetAxisRaw("Horizontal"));

                if (Input.GetButtonDown("Vertical") || Input.GetButtonUp("Vertical"))
                    m_Character.CmdSetMoveRawZ(Input.GetAxisRaw("Vertical"));


                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = m_Character.transform.position.z - Camera.main.transform.position.z;
                    m_Character.CmdLookAt(Camera.main.ScreenToWorldPoint(mousePosition));
                }

                if (Input.GetButtonDown("Fire1"))
                    m_Character.CmdAttack();
            }
        }

        void CharacterNetIdHook(uint netId)
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
