using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRaycaster
{
    internal class FrameBuffer
    {
        private byte[] buffer;
        private int renderWidth;
        private int renderHeight;

        public byte[] Buffer { get { return buffer; } }


        public FrameBuffer(int _renderWidth, int _renderHeight)
        {
            renderWidth = _renderWidth;
            renderHeight = _renderHeight;

            buffer = new byte[_renderWidth * _renderHeight * 4]; //4 bytes per pixel
        }

        public void Clear()
        {
            Array.Clear(buffer, 0, buffer.Length);
        }

        public void SetPixel(int x, int y, Raylib_cs.Color pixel_color)
        {
            int idx = (y * renderWidth + x) * 4;

            buffer[idx] = pixel_color.R;
            buffer[idx + 1] = pixel_color.G;
            buffer[idx + 2] = pixel_color.B;
            buffer[idx + 3] = 255;
        }

        public void DrawRectangle(int x, int y, int w, int h, Raylib_cs.Color color)
        {

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int cx = x + i;
                    int cy = y + j;

                    if (cx >= renderWidth || cy >= renderHeight) continue;

                    SetPixel(cx, cy, color);
                }
            }
        }
    }
}
