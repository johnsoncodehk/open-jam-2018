using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Character : NetworkBehaviour
    {

        public float moveRaw = 0;
        public float moveSpeed = 10;

        CharacterController2D m_Controller;
        Animator m_Animator;

        void Awake()
        {
            m_Controller = GetComponent<CharacterController2D>();
            m_Animator = GetComponent<Animator>();
        }
        void FixedUpdate()
        {
            m_Controller.Move(moveRaw * moveSpeed * Time.fixedDeltaTime, false, false);
        }
        void Update()
        {
            m_Animator.SetFloat("Move Raw", moveRaw);
        }

        [Command]
        public void CmdSetMoveRaw(float raw) {
            RpcSetMoveRaw(raw);
        }
        [Command]
        public void CmdAttack() {
            RpcAttack();
        }
        [Command]
        public void CmdLookAt(Vector2 position) {
            RpcLookAt(position);
        }

        [ClientRpc]
        public void RpcSetMoveRaw(float raw) {
            moveRaw = raw;
        }
        [ClientRpc]
        public void RpcAttack() {
            Attack();
        }
        [ClientRpc]
        public void RpcLookAt(Vector2 position) {
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
