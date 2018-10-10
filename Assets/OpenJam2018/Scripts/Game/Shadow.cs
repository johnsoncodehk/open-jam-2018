using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenJam2018
{
    public class Shadow : MonoBehaviour
    {
        public static void AddTo(Transform tran)
        {
            var shadow = Resources.Load<Shadow>("Shadow");
            shadow = Instantiate(shadow, tran.position, shadow.transform.rotation);
            shadow.m_Target = tran;
        }

        Transform m_Target;
        static Vector3 m_RaycastOffset = new Vector3(0, 0.1f, 0);
        static int m_IgnoreLayer;
        float m_GroundPositionY = 0;

        void Awake()
        {
            m_IgnoreLayer = ~((1 << LayerMask.NameToLayer("Character")) | (1 << LayerMask.NameToLayer("Hitbox")) | (1 << LayerMask.NameToLayer("Airbox")));
        }
        void Update()
        {
            if (!m_Target)
            {
                Destroy(gameObject);
                return;
            }
            if (Physics.Raycast(m_Target.transform.position + m_RaycastOffset, Vector3.down, out var hit, Mathf.Infinity, m_IgnoreLayer))
            {
                // Debug.DrawRay(m_Target.transform.position + m_RaycastOffset, Vector3.down * hit.distance, Color.yellow);
                m_GroundPositionY = hit.point.y;
            }
            // elsea
            // {
            //     Debug.DrawRay(m_Target.transform.position + m_RaycastOffset, m_Target.transform.position + Vector3.down * 100, Color.white);
            // }

            Vector3 pos = m_Target.position;
            pos.y = m_GroundPositionY + 0.01f;
            transform.position = pos;
        }
    }
}
