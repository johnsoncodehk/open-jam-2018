﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenJam2018
{
    public class Arrow : MonoBehaviour
    {

        public static List<Arrow> deadArrows = new List<Arrow>();

        public GameTeam team;
        public int atk = 1;
        public float hitForce = 1;

        Rigidbody m_Rigidbody;
        Collider m_Collider;
        SpriteRenderer m_SpriteRenderer;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_SpriteRenderer.material = Instantiate(m_SpriteRenderer.material);

            Shadow.AddTo(transform);
        }
        void Update()
        {
            Vector2 v = m_Rigidbody.velocity;
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.localEulerAngles = new Vector3(0, 0, angle - 90);
        }
        public void OnTriggerEnter(Collider other)
        {
            if (!enabled)
                return;

            while (deadArrows.Count > 10)
            {
                if (deadArrows[0])
                    Destroy(deadArrows[0].gameObject);
                deadArrows.RemoveAt(0);
            }

            Character character = other.GetComponent<Character>();
            if (character && character.team != team)
            {
                character.OnHit(atk, hitForce);
                Destroy(gameObject);
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Destroy(gameObject);
                // deadArrows.Add(this);

                // Destroy(m_Rigidbody);
                // Destroy(m_Collider);
                // enabled = false;
                // transform.SetParent(other.transform);
                return;
            }
        }

        public void Spawn(float v)
        {
            Arrow arrow = Instantiate(this, transform.position, transform.rotation);
            arrow.gameObject.SetActive(true);
            arrow.GetComponent<Rigidbody>().velocity = arrow.transform.up * v;
        }
    }
}
