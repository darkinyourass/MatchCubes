using UnityEngine;

namespace DefaultNamespace
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;

        [SerializeField] private AudioSource _effectSource;
        [SerializeField] private AudioSource _musicSource;

        public AudioSource EffectSource
        {
            get => _effectSource;
            set => _effectSource = value;
        }

        public AudioSource MusicSource
        {
            get => _musicSource;
            set => _musicSource = value;
        }

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                }

                return _instance;
            }

        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            _effectSource.clip = audioClip;
            _effectSource.Play();
        }

        public void PlayMusic(AudioClip audioClip)
        {
            _musicSource.clip = audioClip;
            _musicSource.Play();
        }
    }
}