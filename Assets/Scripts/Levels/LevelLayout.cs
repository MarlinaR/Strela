using System.Collections.Generic;
using System.IO;
using Blocks;
using UnityEditor;
using UnityEngine;

namespace Levels
{
    public class LevelLayout : ScriptableObject
    {
        public const int MinLevelWidth = 14;
        public const int MaxLevelWidth = 30;
        public const int MinLevelHeight = 14;

        public List<Vector2> wallsPositions;
        public List<PistonConfig> pistonConfigs;
        public List<LeverConfig> leverConfigs;

        public Vector2 playerPosition;
        public List<Vector2> targetPositions;
        public List<Vector2> coinPositions;
    
        // Level theme
        public LevelTheme theme;

        public void InstantiateGrass(Transform host)
        {
            var grassHost = new GameObject("GrassLayer").transform;
            grassHost.SetParent(host);
        
            for (int i = -MaxLevelWidth / 2; i <= MaxLevelWidth / 2; i++)
            {
                for (int j = -MinLevelHeight / 2 - 1; j <= MinLevelHeight / 2 + 1; j++)
                {
                    var prefab = GetRandom(theme.grassPrefabs);
                    Instantiate(prefab, new Vector2(i, j ), Quaternion.identity).transform.SetParent(grassHost);
                }            
            }
        
            Instantiate(theme.globalLightPrefab, new Vector3(0, 0, 0), Quaternion.identity).transform.SetParent(host);
        }

        public void InstantiateLevel(Transform host, ref Dictionary<Vector2Int, GameObject> map)
        {
            if (map == null)
            {
                map = new Dictionary<Vector2Int, GameObject>();
            }
            InstantiateGrass(host);
        
            var playerObj = Instantiate(theme.playerPrefab, playerPosition, Quaternion.identity, host);
            map.Add(playerPosition.ToIntVector(), playerObj);

            var wallHost = new GameObject("WallLayer").transform;
            wallHost.SetParent(host);
        
            foreach (var wallPos in wallsPositions)
            {
                var prefab = GetRandom(theme.wallPrefabs);
                var wallObj = Instantiate(prefab, wallPos, Quaternion.identity, wallHost);
                map.Add(wallPos.ToIntVector(), wallObj);
            }

            foreach (var pistonConfig in pistonConfigs)
            {
                var prefab = GetRandom(theme.autoPistonPrefabs);
                var pistonObj = pistonConfig.InstantiatePrefab(prefab, GetRandom(theme.autoPistonSections), host);
                map.Add(pistonObj.transform.position.ToIntVector(), pistonObj);
            }

            foreach (var leverConfig in leverConfigs)
            {
                var prefab = GetRandom(theme.leverPrefabs);
                var pistonPrefab = GetRandom(theme.pistonPrefabs);
                var pistonSectionPrefab = GetRandom(theme.pistonSections);
                Lever lever = leverConfig.InstantiatePrefab(prefab, pistonPrefab, pistonSectionPrefab, host, map).GetComponent<Lever>();
                map.Add(lever.transform.position.ToIntVector(), lever.gameObject);
            }

            foreach (var pos in targetPositions)
            {
                var targetObj = Instantiate(theme.target, pos, Quaternion.identity, host);
                map.Add(pos.ToIntVector(), targetObj);
            }

            foreach (var coinPosition in coinPositions)
            {
                var targetObj = Instantiate(theme.coin, coinPosition, Quaternion.identity, host);
                map.Add(coinPosition.ToIntVector(), targetObj);
            }
        }

        public void GetFromMap(Dictionary<Vector2Int, GameObject> levelMap)
        {
            wallsPositions = new List<Vector2>();
            pistonConfigs = new List<PistonConfig>();
            leverConfigs = new List<LeverConfig>();
            targetPositions = new List<Vector2>();
            coinPositions = new List<Vector2>();
        
            foreach (var obj in levelMap)
            {
                if (obj.Value == null)
                {
                    continue;
                }

                var blockComponent = obj.Value.GetComponent<ILevelBlock>();

                if (blockComponent.IsDependent)
                {
                    continue;
                }
                blockComponent.SaveToLayout(this);
            }
        }

        private static T GetRandom<T>(IReadOnlyList<T> collection)
        {
            int idx = Random.Range(0, collection.Count - 1);
            return collection[idx];
        }

        public static LevelLayout LoadFromAsset(string path)
        {
            if (!Application.isEditor)
            {
                Debug.Log("Cant load asset in non editor build");
            }
            
#if UNITY_EDITOR
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var asset in assets)
            {
                if (asset is LevelLayout level)
                {
                    return level;
                } 
            }
#endif
            return null;
        }

        public static LevelLayout LoadFromJson(string path)
        {
            var levelLayout = CreateInstance<LevelLayout>();
            var json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, levelLayout);
            return levelLayout;
        }
    }
}
