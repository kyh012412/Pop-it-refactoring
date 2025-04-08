using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopIt
{
    public class FbMusicPlayer : MonoBehaviour
    {
        public static FbMusicPlayer instance;
        public AudioClip bgm;
        public static bool globalSoundState;

        private AudioSource audioSource;

        void Awake()
        {
            //globalSoundState = true;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            instance = this;

            //Enable audio & music in the first run
            // 최초 방문일 때 소리크기에 대한 희망값을 변수에 저장
            Debug.Log("첫 번째 방문인지 확인");
            if (!PlayerPrefs.HasKey("Inited"))
            {
                Debug.Log("첫 번째 방문으로 감지됨");
                print("Game is Inited!");
                PlayerPrefs.SetInt("Inited", 1);
                PlayerPrefs.SetInt("SoundState", 1);
                PlayerPrefs.SetInt("MusicState", 1);
            }

            //fetch saved status
            //Sound
            if (PlayerPrefs.GetInt("SoundState") == 1)
                globalSoundState = true;
            else
                globalSoundState = false;
            //music
            if (PlayerPrefs.GetInt("MusicState") == 0)
                ToggleMusic();
        }

        void Start()
        {
            playSfx(bgm);
        }

        void Update()
        {
            if (!audioSource.isPlaying)
            {
                playSfx(bgm);
            }
        }

        void playSfx(AudioClip _clip)
        {
            audioSource.clip = _clip;
            audioSource.Play();
        }

        public void ToggleMusic()
        {
            audioSource.mute = !audioSource.mute;
        }

        public void ToggleSound()
        {
            globalSoundState = !globalSoundState;
        }
    }
}