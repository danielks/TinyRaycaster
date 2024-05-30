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
        public float A { get; set; } //angle the player is facing (the angle between the view direction and the x axis).
        public float FOV { get; set; } //field of view

        public Player(float x, float y, float a, float fOV)
        {
            X = x;
            Y = y;
            A = a;
            FOV = fOV;
        }
    }
}
