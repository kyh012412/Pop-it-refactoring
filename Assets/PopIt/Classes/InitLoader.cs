using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopIt
{
    public class InitLoader : MonoBehaviour
    {
        /// <summary>
        /// We always need to start the game from Init scene in order to make sure all classes are fully inited the way we need.
        /// </summary>

        void Awake()
        {
            // 어느 씬에서 PlayMode에 진입해도 Init 씬을 거쳐서 정상적으로 동작하게 하는 코드
            if (!GameObject.FindGameObjectWithTag("MusicPlayer"))
                SceneManager.LoadScene(SceneEnum.Init.ToString());
        }
    }
}