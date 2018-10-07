using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Character : NetworkBehaviour
    {

        [SyncVar] public bool faceLeft;

        public Vector3 moveRaw;
        public float moveSpeed = 1;
        public float hitForce = 1;

        Rigidbody m_Rigidbody;
        CharacterController m_Controller;
        Animator m_Animator;
        bool m_SyncPosition;
        Vector3 impact;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
        }
        void Update()
        {
            m_Animator.SetFloat("Move Raw X", moveRaw.x * (faceLeft ? -1 : 1));
            m_Animator.SetFloat("Move Raw Z", moveRaw.z);

            m_Controller.SimpleMove(moveRaw * moveSpeed);

            if (impact.magnitude > 0.2) m_Controller.Move(impact * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
        }
        void FixedUpdate()
        {
            if (m_SyncPosition)
            {
                m_SyncPosition = false;
                RpcSyncPosition(transform.position);
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
            {
                Character otherCharacter = other.GetComponentInParent<Character>();
                if (otherCharacter.faceLeft != faceLeft)
                {
                    AddImpact(otherCharacter.faceLeft ? Vector3.left : Vector3.right, hitForce);
                }
            }
        }
        void AddImpact(Vector3 dir, float force)
        {
            dir.Normalize();
            if (dir.y < 0) dir.y = -dir.y;
            impact += dir.normalized * force / m_Rigidbody.mass;
        }

        public override void OnStartClient()
        {
            if (faceLeft)
                transform.localEulerAngles = new Vector3(0, 180, 0);
        }

        [Command]
        public void CmdSetMoveRawX(float raw)
        {
            m_SyncPosition = true;
            RpcSetMoveRawX(raw);
        }
        [Command]
        public void CmdSetMoveRawZ(float raw)
        {
            m_SyncPosition = true;
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
