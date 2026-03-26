using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class IcarusFeather : Junk
    {
        public override Color ItemColor => Color.DarkOliveGreen;
        public override char Symbol => '~';
        public override string Name => "Złamane pióro Ikara";

    }
}
