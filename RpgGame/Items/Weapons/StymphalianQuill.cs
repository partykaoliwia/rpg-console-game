// FILE: StymphalianQuill.cs
using RpgGame.Items.Weapons;

namespace RpgGame.Items.Weapons
{
    public class StymphalianQuill : LightWeapon
    {
        public override string Symbol => "»";
        public override string Name => "Stymphalian Quill";

        public StymphalianQuill(int damage) : base(damage) { }
    }
}