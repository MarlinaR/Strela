using System;
using System.Collections.Generic;

namespace Blocks
{
    public enum PropertyType
    {
        Number,
        NumberArray,
        Object,
        ObjectArray,
        Bool,
    }

    public interface IConfigurable : ILevelBlock
    {
        List<Tuple<string, PropertyType, object>> GetConfiguration();

        void SetConfiguration(Tuple<string, object> configuration);
    }
}