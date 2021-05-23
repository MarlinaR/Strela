using Levels;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Blocks
{
    public class Target : MonoBehaviour, IInteractable, ILevelBlock
    {
        public Sprite destroyedTarget;

        private AudioSource myAudioSource;
        private bool isBroken;
    
        private void Awake()
        {
            myAudioSource = GetComponent<AudioSource>();
        }
    
        public void Interact()
        {
            if (isBroken)
            {
                return;
            }
        
            isBroken = true;
            myAudioSource.Play();
            GetComponent<Light2D>().intensity = 0.25f;
            GetComponent<SpriteRenderer>().sprite = destroyedTarget;
            GameSettings.currentLevel.Win();
        }


        public void SaveToLayout(LevelLayout level)
        {
            level.targetPositions.Add(transform.position);
        }

        public bool IsDependent => false;
    }
}
