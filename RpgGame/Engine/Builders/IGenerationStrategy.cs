using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Builders
{
    public interface IGenerationStrategy
    {
        void GenerateMap(IDungeonBuilder builder);
    }
}
