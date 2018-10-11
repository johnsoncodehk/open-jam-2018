using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenJam2018
{
    public class ArcherCharacter : Character
    {

        static float lastAttackAudioTime;

        public Arrow arrow;
        public Transform bowHolder;
        public float arrowSpeed = 20;

        public override void LookAt(Vector2 position)
        {
            bowHolder.right = new Vector3(position.x, position.y, bowHolder.position.z) - bowHolder.position;
        }
        public override void Attack()
        {
            base.Attack();
            arrow.team = team;
            arrow.atk = atk;
            arrow.hitForce = hitForce;
            arrow.Spawn(team == GameTeam.Player ? arrowSpeed : Random.Range(0, arrowSpeed));

            if (Time.time - lastAttackAudioTime > 0.05f)
            {
                lastAttackAudioTime = Time.time;
                m_AttackAudio.Play();
            }
        }
        public override void OnAttack()
        { }
        protected override IEnumerator StartEnemyAI()
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

                Vector3 moveTo = target.transform.position;
                moveTo.x = Mathf.Max(m_StartPosition.x, target.transform.position.x + offset.x);
                if (!MoveTo(moveTo))
                    continue;

                Vector3 lookAt = bowHolder.position;
                lookAt.x -= Random.Range(1, 5);
                lookAt.y += Random.Range(1, 10);
                yield return new WaitForSeconds(Random.Range(0f, attackRecovery));
                CmdLookAt(lookAt);
                CmdAttack();
                yield return new WaitForSeconds(attackRecovery);
            }
        }
        protected override float GetTargetDistance(Vector3 targetPosition)
        {
            return Mathf.Abs(transform.position.z - targetPosition.z);
        }
    }
}
