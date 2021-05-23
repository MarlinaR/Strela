using System;
using Blocks;
using UnityEngine;

namespace Levels
{
    [Serializable]
    public class PistonConfig
    {
        public Vector2 position;
        public Quaternion rotation;
    
        public int length;
        public float pistonSpeed;

        // For auto pistons
        public float activeCycle;
        public float inactiveCycle;

        // For non-auto pistons
        public bool isActive;

        public GameObject InstantiatePrefab(GameObject prefab, GameObject pistonSection, Transform host)
        {
            var obj = UnityEngine.Object.Instantiate(prefab, position, rotation);
            obj.transform.SetParent(host);

            var piston = obj.GetComponent<PistonBase>();
            piston.pistonSpeed = pistonSpeed;
            piston.length = length;
            piston.pistonBlockPrefab = pistonSection;
            if (isActive)
            {
                piston.SwitchState();
            }

            if (piston is AutoPiston autoPiston)
            {
                autoPiston.activeCycleTime = activeCycle;
                autoPiston.inactiveCycleTime = inactiveCycle;
            }

            return obj;
        }
    }
}
