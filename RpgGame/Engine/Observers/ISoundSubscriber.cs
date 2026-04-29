using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Observers
{
    public interface ISoundSubscriber
    {
        int X { get; }
        int Y { get; }
        void UpdateOnSound(int sourceX, int sourceY, int range);
    }
}
