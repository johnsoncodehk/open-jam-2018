using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenJam2018
{
    public class Character : NetworkBehaviour
    {

        public static List<Character> playerTeam = new List<Character>();
        public static List<Character> enemyTeam = new List<Character>();
        public static List<Character> enemyBowTeam = new List<Character>();

        [SyncVar, HideInInspector] public GameTeam team;
        public int raw => team == GameTeam.Player ? 1 : -1;

        [SyncVar] public int hp = 1;
        public int atk = 1;
        public float moveSpeed = 1;
        public float hitForce = 1;
        public Color hurtColor;
        public GameObject ghost;

        Vector3 m_MoveRaw;
        Rigidbody m_Rigidbody;
        CharacterController m_Controller;
        Animator m_Animator;
        bool m_SyncPosition;
        Vector3 m_Impact, m_Impact0;
        Material m_Material;
        Vector3 m_StartPosition, m_LastPosition;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_Material = GetComponentInChildren<SpriteRenderer>().material;
            m_Material = Instantiate(m_Material);
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
                sprite.material = m_Material;
        }
        void Update()
        {
            m_Animator.SetFloat("Move Raw X", m_MoveRaw.x * raw);
            m_Animator.SetFloat("Move Raw Z", m_MoveRaw.z);

            m_Controller.SimpleMove(m_MoveRaw * moveSpeed);

            if (m_Impact.magnitude > 0.2)
            {
                m_Controller.Move(m_Impact * Time.deltaTime);
                m_Material.color = Color.Lerp(Color.white, hurtColor, m_Impact.magnitude / m_Impact0.magnitude);
            }
            else
            {
                m_Material.color = Color.white;
            }
            m_Impact = Vector3.Lerp(m_Impact, Vector3.zero, 5 * Time.deltaTime);
        }
        void FixedUpdate()
        {
            m_LastPosition = transform.position;
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
                Character character = other.GetComponentInParent<Character>();
                if (character)
                {
                    if (character.team != team)
                        OnHit(character.atk, character.hitForce);
                }
            }
        }
        void OnDestroy()
        {
            if (team == GameTeam.Enemy)
                if (this is ArcherCharacter)
                    enemyBowTeam.Remove(this);
                else
                    enemyTeam.Remove(this);
            else
                playerTeam.Remove(this);

            Instantiate(ghost, m_LastPosition, Quaternion.identity);
        }

        void AddImpact(Vector3 dir, float force)
        {
            dir.Normalize();
            if (dir.y < 0) dir.y = -dir.y;
            m_Impact += dir.normalized * force / m_Rigidbody.mass;
            m_Impact0 = m_Impact;
        }
        protected virtual IEnumerator StartEnemyAI()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

                Vector3 offset = new Vector3(0.5f, 0, 0);
                var (target, distance) = FindNearPlayer(offset);

                if (!target)
                {
                    BackToHome();
                    continue;
                }

                if (!MoveTo(target.transform.position + offset))
                    continue;

                CmdAttack();
                yield return new WaitForSeconds(0.5f);
            }
        }
        protected (Character, float) FindNearPlayer(Vector3 offset)
        {
            if (playerTeam.Count == 0)
                return (null, 0);

            Character nearC = playerTeam[0];
            float nearD = Vector3.Distance(transform.position, nearC.transform.position + offset);
            for (int i = 1; i < playerTeam.Count; i++)
            {
                Character c = playerTeam[i];
                float d = Vector3.Distance(transform.position, c.transform.position + offset);
                if (d < nearD)
                {
                    nearC = c;
                    nearD = d;
                }
            }
            return (nearC, nearD);
        }
        protected bool MoveTo(Vector3 position)
        {
            Vector3 d = position - transform.position;
            bool moving = Mathf.Abs(d.x) > 0.5f || Mathf.Abs(d.z) > 0.25f;

            if (d.x > 0.5) TrySetMoveRawX(1);
            else if (d.x < -0.5) TrySetMoveRawX(-1);
            else TrySetMoveRawX(0);

            if (Mathf.Abs(d.x) < 2 || Mathf.Abs(d.x / d.z) < 1.2f)
            {
                if (d.z > 0.25) TrySetMoveRawZ(1);
                else if (d.z < -0.25) TrySetMoveRawZ(-1);
                else TrySetMoveRawZ(0);
            }
            else TrySetMoveRawZ(0);

            return !moving;
        }
        protected bool BackToHome()
        {
            return MoveTo(m_StartPosition);
        }

        public override void OnStartClient()
        {
            m_StartPosition = transform.position;

            if (team == GameTeam.Enemy)
                transform.localEulerAngles = new Vector3(0, 180, 0);

            if (team == GameTeam.Enemy)
            {
                if (this is ArcherCharacter)
                    enemyBowTeam.Add(this);
                else
                    enemyTeam.Add(this);
            }
            else
                playerTeam.Add(this);
        }
        public override void OnStartServer()
        {
            if (team == GameTeam.Enemy)
                StartCoroutine(StartEnemyAI());
        }

        public void OnHit(int atk, float force)
        {
            AddImpact(team == GameTeam.Enemy ? Vector3.right : Vector3.left, force);
            hp -= atk;
            if (hp <= 0 && isServer)
                NetworkServer.Destroy(gameObject);
        }
        public void TrySetMoveRawX(float raw)
        {
            if (raw == m_MoveRaw.x)
                return;
            CmdSetMoveRawX(raw);
        }
        public void TrySetMoveRawZ(float raw)
        {
            if (raw == m_MoveRaw.z)
                return;
            CmdSetMoveRawZ(raw);
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
            m_MoveRaw.x = raw;
        }
        [ClientRpc]
        public void RpcSetMoveRawZ(float raw)
        {
            m_MoveRaw.z = raw;
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
        { }
    }
}
