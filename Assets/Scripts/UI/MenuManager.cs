using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        public InputField inputField;
        public AudioClip audioClip;

        private AudioSource _audioSource;

        void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = audioClip;
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.volume = 0.4f;
        }

        public void StartGame()
        {
            if (GameSettings.instance != null)
            {
                _audioSource.Play();
                GameSettings.instance.Username = inputField.text;
                GameSettings.instance.NextLevel();
            }
        }

        public void StartEditor()
        {
            if (GameSettings.instance != null)
            {
                _audioSource.Play();
                GameSettings.instance.Username = inputField.text;
                GameSettings.instance.OpenEditor();
            }
        }
    }
}
