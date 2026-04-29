using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Engine.Builders;
using RpgGame.Items;
using RpgGame.Items.Money;

namespace RpgGame.Engine.Factories
{
    public interface IFactory
    {
        string[] Title { get; }
        string IntroMessage { get; }
        string ThemeName { get; }
        IGenerationStrategy GenerationStrategy { get; }

        IItem CreateRandomJunk();
        Currency CreateRandomCurrency();
        IItem CreateRandomWeapon(int index);
        IAttackable CreateEnemy(int distanceFromStart);
        IAttackable CreateBoss();
    }
}
