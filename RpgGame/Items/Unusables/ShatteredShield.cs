// FILE: ShatteredShield.cs
using System.Drawing;
using RpgGame.Items.Unusables;

namespace RpgGame.Items.Unusables
{
    public class ShatteredShield : Junk
    {
        public override Color ItemColor { get; set; } = Color.DarkSlateGray;
        public override string Symbol => "0";
        public override string Name => "Shattered Shield";
    }
}