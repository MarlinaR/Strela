using System;
using System.Collections.Generic;
using Levels;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Blocks
{
    public class Lever : MonoBehaviour, IInteractable, IConfigurable
    {
        public const string PistonsProp = "pistons";
    
        public PistonBase[] pistons;

        public Sprite brokenSprite;

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
        
            foreach (var piston in pistons)
            {
                piston.SwitchState();
            }

            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = brokenSprite;
        }

        public List<Tuple<string, PropertyType, object>> GetConfiguration()
        {
            var pistonList = new List<GameObject>();
            foreach (var piston in pistons) 
            {
                pistonList.Add(piston.gameObject);
            }
            return new List<Tuple<string, PropertyType, object>>()
            {
                new Tuple<string, PropertyType, object>(PistonsProp, PropertyType.ObjectArray,
                    new Tuple<Type, List<GameObject>>(pistons.GetType().GetElementType(), pistonList)),
            };
        }

        public void SetConfiguration(Tuple<string, object> prop)
        {
            switch (prop.Item1)
            {
                case PistonsProp:
                    var arr = prop.Item2 as IList<GameObject>;
                    pistons = new PistonBase[arr.Count];
                    for (int i = 0; i < arr.Count; i++)
                    {
                        var piston = arr[i].GetComponent<PistonBase>();
                        if (piston == null)
                        {
                            throw new ArgumentException("Invalid configuration array");
                        }

                        pistons[i] = piston;
                    }
                    break;
            }
        }

        public bool IsDependent => false;

        public void SaveToLayout(LevelLayout level)
        {
            var config = new LeverConfig();
            config.pistons = new PistonConfig[pistons.Length];
            for (int i = 0; i < pistons.Length; i++)
            {
                config.pistons[i] = pistons[i].MakeConfig();
            }

            config.position = transform.position;
        
            level.leverConfigs.Add(config);
        }
    }
}
