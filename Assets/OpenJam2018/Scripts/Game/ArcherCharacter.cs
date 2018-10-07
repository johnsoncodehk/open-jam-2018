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
            arrow.Spawn(arrowSpeed);
        }
    }
}
