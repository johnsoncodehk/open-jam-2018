using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Character : NetworkBehaviour
    {

        public Vector3 moveRaw;
        public float moveSpeed = 1;

        CharacterController m_Controller;
        Animator m_Animator;

        void Awake()
        {
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
        }
        void Update()
        {
            m_Animator.SetFloat("Move Raw X", moveRaw.x);
            m_Animator.SetFloat("Move Raw Z", moveRaw.z);

            m_Controller.SimpleMove(moveRaw * moveSpeed);
        }

        [Command]
        public void CmdSyncPosition()
        {
            RpcSyncPosition(transform.position);
        }
        [Command]
        public void CmdSetMoveRawX(float raw)
        {
            RpcSetMoveRawX(raw);
        }
        [Command]
        public void CmdSetMoveRawZ(float raw)
        {
            RpcSetMoveRawZ(raw);
        }
        [Command]
        public void CmdAttack()
        {
            RpcAttack();
        }
        [Command]
        public void CmdLookAt(Vector2 position)
        {
            RpcLookAt(position);
        }

        [ClientRpc]
        public void RpcSyncPosition(Vector3 position)
        {
            transform.position = position;
        }
        [ClientRpc]
        public void RpcSetMoveRawX(float raw)
        {
            moveRaw.x = raw;
        }
        [ClientRpc]
        public void RpcSetMoveRawZ(float raw)
        {
            moveRaw.z = raw;
        }
        [ClientRpc]
        public void RpcAttack()
        {
            Attack();
        }
        [ClientRpc]
        public void RpcLookAt(Vector2 position)
        {
            LookAt(position);
        }

        public virtual void Attack()
        {
            m_Animator.SetTrigger("Attack");
        }
        public virtual void LookAt(Vector2 position)
        {

        }
    }
}
