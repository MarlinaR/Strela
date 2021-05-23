using System;
using System.Collections.Generic;
using Blocks;
using UnityEngine;

namespace Levels
{
    [Serializable]
    public class LeverConfig
    {
        public Vector2 position;
        public PistonConfig[] pistons;
    
        public GameObject InstantiatePrefab(GameObject prefab, GameObject pistonPrefab, GameObject pistonSection, Transform host, Dictionary<Vector2Int, GameObject> map)
        {
            var obj = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
            obj.transform.SetParent(host);
            var lever = obj.GetComponent<Lever>();

            obj.transform.position = position;
            lever.pistons = new PistonBase[pistons.Length];

            for (int i = 0; i < pistons.Length; i++)
            {
                var pos = pistons[i].position.ToIntVector();
                if (map.ContainsKey(pos))
                {
                    lever.pistons[i] = map[pos].GetComponent<PistonBase>();
                }
                else
                {
                    lever.pistons[i] = pistons[i].InstantiatePrefab(pistonPrefab, pistonSection, host).GetComponent<PistonBase>();
                    map.Add(pos, lever.pistons[i].gameObject);
                }
            }

            return obj;
        }
    }
}
