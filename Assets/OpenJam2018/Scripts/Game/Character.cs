using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Character : NetworkBehaviour
    {

        public float moveRaw = 0;

        CharacterController2D m_Controller;
        Animator m_Animator;

        void Awake()
        {
            m_Controller = GetComponent<CharacterController2D>();
            m_Animator = GetComponent<Animator>();
        }
        void FixedUpdate()
        {
            m_Controller.Move(moveRaw * 40 * Time.fixedDeltaTime, false, false);
        }
        void Update()
        {
            m_Animator.SetFloat("Move Raw", moveRaw);
        }

        public virtual void LookAt(Vector2 position)
        {

        }
        public virtual void Attack()
        {
            m_Animator.SetTrigger("Attack");
        }
    }
}
