using Levels;
using UnityEngine;

namespace Blocks
{
    public class Wall : MonoBehaviour, ILevelBlock
    {
        public void SaveToLayout(LevelLayout level)
        {
            level.wallsPositions.Add(transform.position);
        }

        public bool IsDependent => false;
    }
}
