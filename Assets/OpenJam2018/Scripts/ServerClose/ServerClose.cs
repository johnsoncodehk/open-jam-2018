using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace OpenJam2018
{
    public class ServerClose : MonoBehaviour
    {
        public void Restart()
        {
            Application.Quit();
        }
    }
}
