using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pastel;

namespace RpgGame.Common
{
    public interface IDrawable
    {
        char Symbol { get; }
        Color ItemColor { get; }
    }
}
