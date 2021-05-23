using Levels;

namespace Blocks
{
    public interface ILevelBlock
    {
        void SaveToLayout(LevelLayout level);

        bool IsDependent { get; }
    }
}