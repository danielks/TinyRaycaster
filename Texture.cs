using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TinyRaycaster
{
    internal class Texture
    {
        List<Raylib_cs.Color[,]> textures;
        //textures must be squares. size of the side.
        private int size;
        private int count;

        public int Size { get { return size; } }

        public int Count { get { return count; } }

        public Texture(string fileName)
        {

            textures = new List<Raylib_cs.Color[,]>();

            Bitmap image = (Bitmap)Bitmap.FromFile(fileName);

            size = image.Height;

            count = image.Width / image.Height;



            for (int i = 0; i < count; i++)
            {
                var tex = new Raylib_cs.Color[size, size];

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        tex[x, y] = Util.To_Raylib_Color(image.GetPixel(x + (i * size), y));
                    }
                }

                textures.Add(tex);
            }

        }


        /// <summary>
        /// Get the pixel (i,j) from the textrue idx
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Raylib_cs.Color Get(int x, int y, int idx)
        {
            return textures[idx][x, y];
        }

        /// <summary>
        /// Retrieve one column (tex_coord) from the texture texture_id and scale it to the destination size
        /// </summary>
        /// <param name="texid"></param>
        /// <param name="texcoord"></param>
        /// <param name="column_height"></param>
        /// <returns></returns>
        public Raylib_cs.Color[] GetScaledColumn(int texid, int texcoord, int column_height)
        {
            Raylib_cs.Color[] column = new Raylib_cs.Color[column_height];

            for (int y = 0; y < column_height; y++)
            {
                /*int  pix_x = texid * wallTextureSize + texcoord;
                int pix_y = (y * wallTextureSize) / column_height;

                column[y] = To_Raylib_Color(((Bitmap)wallTextures).GetPixel(pix_x, pix_y));*/

                int pix_x = texcoord;
                int pix_y = (y * size) / column_height;

                column[y] = Get(pix_x, pix_y, texid);
            }

            return column;
        }
    }

    
}
