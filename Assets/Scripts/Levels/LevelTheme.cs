using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "LevelTheme", menuName = "Level/LevelTheme")]
    public class LevelTheme : ScriptableObject
    {
        public GameObject[] grassPrefabs;
        public GameObject[] leftSidePanels;
        public GameObject[] rightSidePanels;

        public GameObject[] wallPrefabs;
    
        public GameObject[] autoPistonPrefabs;
        public GameObject[] autoPistonSections;
    
        public GameObject[] leverPrefabs;

        public GameObject[] pistonPrefabs;
        public GameObject[] pistonSections;

        public GameObject target;
        public GameObject playerPrefab;
        public GameObject globalLightPrefab;
        public GameObject coin;
    }
}
