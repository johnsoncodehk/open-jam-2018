using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenJam2018
{
    public class ArcherCharacter : Character
    {
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
            arrow.Spawn(arrowSpeed);
        }
        protected override IEnumerator StartEnemyAI()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

                Vector3 offset = new Vector3(0.5f, 0, 0);
                var (target, distance) = FindNearPlayer(offset);

                if (!target)
                {
                    TrySetMoveRawX(0);
                    TrySetMoveRawZ(0);
                    continue;
                }

                Vector3 moveTo = target.transform.position;
                moveTo.x = transform.position.x;
                if (!MoveTo(moveTo))
                    continue;

                Vector3 lookAt = bowHolder.position;
                lookAt.x -= Random.Range(1, 5);
                lookAt.y += Random.Range(1, 10);
                CmdLookAt(lookAt);
                CmdAttack();
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
