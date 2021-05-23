using System;
using System.Collections.Generic;
using System.Globalization;
using Levels;
using UnityEngine;

namespace Blocks
{
    public class PistonBase : MonoBehaviour, IConfigurable
    {
        private const string LengthProp = "length";
        private const string PistonSpeedProp = "pistonSpeed";
        private const string IsActiveProp = "isActive";
        private const string RotationProp = "rotation";
    
        public int length;
        public float pistonSpeed;

        public GameObject pistonBlockPrefab;
    
        public bool isActive;

        private bool isInit;
    
        private PistonSection[] myBlocks;

        public void SwitchState()
        {
            if (isActive)
            {
                PistonDeactivate();
            }
            else
            {
                PistonActivate();
            }
        }

        protected void PistonActivate()
        {
            if (isActive)
            {
                return;
            }

            isActive = true;

            if (!isInit || length != myBlocks.Length)
            {
                RespawnBlocks();
            }
            for (int i = 0; i < length; i++)
            {
                myBlocks[i].MoveToPostion(transform.position + transform.right * (i + 1) * myBlocks[i].SectionSize, pistonSpeed);
            }
        }

        protected void PistonDeactivate()
        {
            if (!isActive)
            {
                return;
            }

            isActive = false;
        
            if (!isInit || length != myBlocks.Length)
            {
                isInit = false;
                RespawnBlocks();
            }
        
            for (int i = 0; i < length; i++)
            {
                myBlocks[i].MoveToPostion(transform.position, pistonSpeed);
            }
        }
    
        private void Awake()
        {
            RespawnBlocks();

            if (isActive)
            {
                isActive = false;
                var savedPistonSpeed = pistonSpeed;
                pistonSpeed = 50;
                SwitchState();
                pistonSpeed = savedPistonSpeed;
            }
        }

        private void Update()
        {
            if (!isInit || length != myBlocks.Length)
            {
                RespawnBlocks();
            }
        }

        private void RespawnBlocks()
        {
            if (isInit && length == myBlocks.Length)
            {
                return;
            }
            
            if (myBlocks != null)
            {
                foreach (var block in myBlocks)
                {
                    Destroy(block.gameObject);
                }
            }
            myBlocks = new PistonSection[length];

            for (int i = 0; i < length; i++)
            {
                var obj = Instantiate(pistonBlockPrefab, transform);
                myBlocks[i] = obj.GetComponent<PistonSection>();
            }

            isInit = true;
        }

        public virtual List<Tuple<string, PropertyType, object>> GetConfiguration()
        {
            return new List<Tuple<string, PropertyType, object>>()
            {
                new Tuple<string, PropertyType, object>(LengthProp, PropertyType.Number, length),
                new Tuple<string, PropertyType, object>(PistonSpeedProp, PropertyType.Number, pistonSpeed),
                new Tuple<string, PropertyType, object>(IsActiveProp, PropertyType.Bool, Convert.ToInt32(isActive)),
                new Tuple<string, PropertyType, object>(RotationProp, PropertyType.Number, transform.rotation.eulerAngles.z)
            };
        }

        public virtual void SetConfiguration(Tuple<string, object> prop)
        {
            switch (prop.Item1)
            {
                case LengthProp:
                    length = Convert.ToInt32(prop.Item2);
                    RespawnBlocks();
                    break;
                case IsActiveProp:
                    var newActive = Convert.ToBoolean(Mathf.RoundToInt((float)prop.Item2));
                    if (newActive != isActive)
                    {
                        SwitchState();
                    }
                    break;
                case PistonSpeedProp:
                    pistonSpeed = (float) prop.Item2;
                    break;
                case RotationProp:
                    var angle = (float) prop.Item2;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    break;
            }
        }

        public virtual bool IsDependent => true;

        public virtual PistonConfig MakeConfig()
        {
            var config = new PistonConfig();
            config.length = length;
            config.position = transform.position;
            config.rotation = transform.rotation;
            config.isActive = isActive;
            config.pistonSpeed = pistonSpeed;
            config.activeCycle = -1;
            config.inactiveCycle = -1;
            return config;
        }
    
        public void SaveToLayout(LevelLayout level)
        {
            level.pistonConfigs.Add(MakeConfig());
        }
    }
}
