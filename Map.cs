using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRaycaster
{
    internal class Map
    {
        private int width;
        private int height;

        string map = "0000222222220000" +
             "1              0" +
             "1      11111   0" +
             "1     0        0" +
             "0     0  1110000" +
             "0     3        0" +
             "0   10000      0" +
             "0   0   11100  0" +
             "0   0   0      0" +
             "0   0   1  00000" +
             "0       1      0" +
             "2       1      0" +
             "0       0      0" +
             "0 0000000      0" +
             "0              0" +
             "0002222222200000"; // our game map

        public Map(int _width, int _height)
        {
            this.width = _width;
            this.height = _height;
        }

        public int Get(int i, int j)
        {
            return (int)(map[i + j * width] - '0');
        }

        public bool IsEmpty(int i, int j)
        {
            return map[i + j * width] == ' ';
        }
    }
}
