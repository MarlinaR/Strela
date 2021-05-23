using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Levels;
using UnityEngine;

namespace Blocks
{
    public class AutoPiston : PistonBase
    {
        private const string ActiveCycleTimeProp = "activeCycle";
        private const string InactiveCycleTimeProp = "inactiveCycle";
    
        public float activeCycleTime;
        public float inactiveCycleTime;
    
        // private void Start()
        // {
        //     StartCoroutine(PistonCycle());
        // }

        private void OnEnable()
        {
            StartCoroutine(PistonCycle());
        }

        IEnumerator PistonCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(inactiveCycleTime);

                if (enabled)
                {
                    PistonActivate();
                }
               
        
                yield return new WaitForSeconds(activeCycleTime);

                if (enabled)
                {
                    PistonDeactivate();
                }
            }
        }

        public override List<Tuple<string, PropertyType, object>> GetConfiguration()
        {
            var config = base.GetConfiguration();
            config.Add(new Tuple<string, PropertyType, object>(ActiveCycleTimeProp, PropertyType.Number, activeCycleTime));
            config.Add(new Tuple<string, PropertyType, object>(InactiveCycleTimeProp, PropertyType.Number, inactiveCycleTime));
            return config;
        }

        public override void SetConfiguration(Tuple<string, object> prop)
        {
            switch (prop.Item1)
            {
                case ActiveCycleTimeProp:
                    activeCycleTime = (float) prop.Item2;
                    break;
                case InactiveCycleTimeProp:
                    inactiveCycleTime = (float) prop.Item2;
                    break;
                default:
                    base.SetConfiguration(prop);
                    break;
            }
        }

        public override PistonConfig MakeConfig()
        {
            var config = base.MakeConfig();
            config.activeCycle = activeCycleTime;
            config.inactiveCycle = inactiveCycleTime;
            return config;
        }

        public override bool IsDependent => false;
    }
}
