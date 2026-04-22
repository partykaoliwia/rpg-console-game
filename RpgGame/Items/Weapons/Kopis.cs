// FILE: Kopis.cs
using RpgGame.Items.Weapons;

namespace RpgGame.Items.Weapons
{
    public class Kopis : HeavyWeapon
    {
        public override string Symbol => "P";
        public override string Name => "Kopis";

        public Kopis(int damage) : base(damage) { }
    }
}