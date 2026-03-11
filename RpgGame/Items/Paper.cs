using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Items
{
    public class Paper : Junk
    {
        public override char Symbol => '#';
        public override string Name => "Piece of paper";
    }
}
