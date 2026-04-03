using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items.Unusables
{
    public class IcarusFeather : Junk
    {
        public override Color ItemColor { get; set; } = Color.LightBlue;
        public override string Symbol => "~";
        public override string Name => "Złamane pióro Ikara";

    }
}
