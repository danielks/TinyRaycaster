using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRaycaster
{
    internal class Util
    {
        public static int float_to_int(float num)
        {
            //acredito que esse seja o comportamento da versao do artigo
            return Convert.ToInt32(MathF.Floor(num));
        }

        public static Raylib_cs.Color To_Raylib_Color(System.Drawing.Color color)
        {
            return new Raylib_cs.Color(color.R, color.G, color.B, color.A);
        }
    }
}
