using UnityEditor;
using UnityEngine;

namespace Levels
{
    [ExecuteAlways]
    public class LevelLoader : MonoBehaviour
    {
        public GameObject cameraPrefab;
        public GameObject gameSettingsPrefab;
        public GameObject levelPrefab;
        
        public LevelLayout levelToLoad;
        public bool doReload;

        private bool isLoaded = true;
    
        public void ReloadLevel(LevelLayout layout)
        {
            levelToLoad = layout;
            isLoaded = false;
            doReload = false;
        }
    
    
        void Update()
        {
            if (doReload)
            {
                ReloadLevel(levelToLoad);
            }
        
            if (isLoaded || levelToLoad == null)
            {
                return;
            }
        
            Debug.Log("LEVEL LOADER: Level loaded");

            if (Application.isEditor)
            {
#if UNITY_EDITOR
                PrefabUtility.InstantiatePrefab(cameraPrefab);
                PrefabUtility.InstantiatePrefab(gameSettingsPrefab);
#endif
            }
            else
            {
                Instantiate(cameraPrefab, new Vector3(0, 0, -10), Quaternion.identity);
                Instantiate(gameSettingsPrefab, Vector3.zero, Quaternion.identity);
            }

            LevelConfig levelObj = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity).GetComponent<LevelConfig>();
            levelToLoad.InstantiateLevel(levelObj.transform, ref levelObj.levelMap);
            isLoaded = true;
        }
    }
}
