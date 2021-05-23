using System.Collections;
using System.Collections.Generic;
using Blocks;
using Levels;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour, IInteractable, ILevelBlock
{
    private AudioSource myAudioSource;
    private bool isDestroyed = false;

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        myAudioSource.Play();
        GameSettings.currentLevel.coin += 1;
    }

    public void SaveToLayout(LevelLayout level)
    {
        level.coinPositions.Add(transform.position);
    }

    public bool IsDependent => false;
}
