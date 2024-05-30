using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRaycaster
{
    internal class Player
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float A { get; set; } //view direction
        public float FOV { get; set; } //field of view
    }
}
