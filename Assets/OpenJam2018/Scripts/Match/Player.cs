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
        ArcherCharacter m_ArcherCharacter;

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
            while (!Game.instance)
                yield return new WaitForEndOfFrame();

            RandomCharacter();
        }

        public void RandomCharacter()
        {
            if (Game.instance.playerRemain <= 0)
                return;
            Game.instance.playerRemain--;

            Character character = Instantiate(
                NetworkManagerHandler.instance.spawnPrefabs[Random.Range(0, 2)],
                FindObjectOfType<Game>().RandomPlayerPosition(),
                Quaternion.identity
            ).GetComponent<Character>(); ;
            NetworkServer.SpawnWithClientAuthority(character.gameObject, connectionToClient);

            characterNetId = character.netId.Value;
            character.onDead += RandomCharacter;
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

                if (m_ArcherCharacter && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
                    m_Character.CmdLookAt(GetLook(m_ArcherCharacter.bowHolder));

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
            {
                m_Character = NetworkManagerHandler.FindLocalObject(new NetworkInstanceId(characterNetId)).GetComponent<Character>();
                if (m_Character is ArcherCharacter)
                    m_ArcherCharacter = m_Character as ArcherCharacter;
            }
            else
            {
                m_Character = null;
                m_ArcherCharacter = null;
            }
        }
        static Vector3 GetLook(Transform character)
        {
            float rad = 45f * Mathf.Deg2Rad;
            float cosA = Mathf.Cos(rad);
            float sinA = Mathf.Sin(rad);

            Vector3 crtWorldPos = character.position;
            crtWorldPos.y += (cosA - 1) * character.position.y;
            crtWorldPos.z += sinA * character.position.y;

            Vector3 crtCameraPos = Camera.main.transform.InverseTransformPoint(crtWorldPos);
            float z = crtCameraPos.z;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = crtCameraPos.z;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 mosueCameraPos = Camera.main.transform.InverseTransformPoint(mouseWorldPos);
            Vector3 d = mosueCameraPos - crtCameraPos;

            d.x = Mathf.Max(1f, d.x);

            return character.position + d;
        }
    }
}
