using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Items;
using RpgGame.Items.Money;

namespace RpgGame.Engine.Builders
{
    public interface IFactory
    {
        IItem CreateRandomJunk();
        Currency CreateRandomCurrency();
        IItem CreateRandomWeapon(int index);
        IAttackable CreateEnemy(int distanceFromStart);
        IAttackable CreateBoss();
    }
}
