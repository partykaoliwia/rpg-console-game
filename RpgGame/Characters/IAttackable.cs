using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Common;

namespace RpgGame.Characters
{
    public interface IAttackable : IDrawable
    {
        string Name { get; }
        int Health { get; }
        int Strength { get; }
        int Armor { get; }
        bool IsAlive { get; }

        int TakeDamage(int damage);

    }
}
