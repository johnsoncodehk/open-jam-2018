using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenJam2018
{
    public class Ghost : MonoBehaviour
    {
        public void AnimationFinish()
        {
            Destroy(gameObject);
        }
    }
}
