using Raylib_cs;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using TinyRaycaster;
using static System.Net.Mime.MediaTypeNames;




const int RENDER_WIDTH = 3000;
const int RENDER_HEIGHT = 1500;
const int WINDOW_WIDTH = RENDER_WIDTH;
const int WINDOW_HEIGHT = RENDER_HEIGHT;

float player_x = 3.456f;
float player_y = 2.345f;
float player_a = 1.523f; //angle the player is facing (the angle between the view direction and the x axis).
const float fov = MathF.PI / 3.0f;

string wallTexturesFilePath = @"C:\Users\danie\source\repos\TinyRaycaster\resources\textures.bmp";

var wallTextures = Bitmap.FromFile(wallTexturesFilePath);

int wallTextureSize = wallTextures.Height; //square

FrameBuffer frameBuffer = new FrameBuffer(RENDER_WIDTH, RENDER_HEIGHT);



int wallTexturesCount = wallTextures.Width / wallTextures.Height;

List<Raylib_cs.Color[,]> wallTextures2 = new List<Raylib_cs.Color[,]>();

for (int i = 0; i < wallTexturesCount; i++)
{
    var tex = new Raylib_cs.Color[wallTextureSize, wallTextureSize];

    for (int x = 0; x < wallTextureSize; x++)
    {
        for (int y = 0; y < wallTextureSize; y++)
        {
            tex[x, y] = To_Raylib_Color(((Bitmap)(wallTextures)).GetPixel(x + (i * wallTextureSize), y));
        }
    }

    wallTextures2.Add(tex);
}



const int map_w = 16; // map width
const int map_h = 16; // map height
Map map = new Map(map_w, map_h);

Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Tiny Raycaster");

long frameCount = 0;

Stopwatch sw = new Stopwatch();
double frameTime = sw.Elapsed.TotalMilliseconds;


while (!Raylib.WindowShouldClose())
{
    sw.Restart();

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Raylib_cs.Color.White);

    render();

    Raylib.EndDrawing();

    sw.Stop();
    frameTime = sw.Elapsed.TotalMilliseconds;

    if (frameCount % 10 == 0) //a cada 50 frames mostra o ultimo frametime
    {
        Raylib.SetWindowTitle(string.Format("{0}ms. FPS: {1}", frameTime, 1000.0f / frameTime));
    }

    frameCount++;
}

Raylib.CloseWindow();

void render()
{
    frameBuffer.Clear();

    const int rect_w = RENDER_WIDTH / (map_w * 2); //render map on half the screen
    const int rect_h = RENDER_HEIGHT / map_h;

    for (int j = 0; j < map_h; j++)
    {
        for (int i = 0; i < map_w; i++)
        {
            
            if (map.IsEmpty(i, j)) continue;

            int cell = map.Get(i, j);

            int rect_x = i * rect_w;
            int rect_y = j * rect_h;            

            var color = To_Raylib_Color(((Bitmap)wallTextures).GetPixel(cell * wallTextureSize, 0));

            frameBuffer.DrawRectangle(rect_x, rect_y, rect_w, rect_h, color);
        }
    }

    player_a += 0.005f;

    //draw the player on the map
    frameBuffer.DrawRectangle(Convert.ToInt32(player_x * rect_w), Convert.ToInt32(player_y * rect_h), 5, 5, new Raylib_cs.Color(255, 255, 255, 255));

    //draw the visibility cone
    for (int i = 0; i < RENDER_WIDTH / 2; i++)
    //Parallel.For(0, RENDER_WIDTH / 2, (i) =>
    {
        float angle = player_a - fov / 2 + fov * i / ((float)(RENDER_WIDTH / 2));
        //float angle = player_a - fov / 2 + fov * i / ((float)(RENDER_WIDTH));
        //makes a line in the direction the player is facing. stops if hits an obstacle.
        for (float t = 0.0f; t < 20.0f; t += 0.05f)
        {
            float cx = player_x + t * MathF.Cos(angle);
            float cy = player_y + t * MathF.Sin(angle);

            //if (map[float_to_int(cx) + float_to_int(cy) * map_w] != ' ') break;

            int pix_x = Convert.ToInt32(cx * rect_w);
            int pix_y = Convert.ToInt32(cy * rect_h);


            //this draws the visibility cone
            frameBuffer.SetPixel(pix_x, pix_y, new Raylib_cs.Color(160, 160, 160, 255));

            //our ray touches a wall, so draw the vertical column to create an illusion of 3D.
            if (!map.IsEmpty(float_to_int(cx), float_to_int(cy)))
            {
                int cell = map.Get(float_to_int(cx), float_to_int(cy));
                int texid = cell;

                int column_height = float_to_int(RENDER_HEIGHT / (t * MathF.Cos(angle - player_a)));                

                float hitx = cx - MathF.Floor(cx + 0.5f); // hitx and hity contain (signed) fractional parts of cx and cy,
                float hity = cy - MathF.Floor(cy + 0.5f); // they vary between -0.5 and +0.5, and one of them is supposed to be very close to 0
                int x_texcoord = float_to_int(hitx * wallTextureSize);

                if (MathF.Abs(hity) > MathF.Abs(hitx)) // we need to determine whether we hit a "vertical" or a "horizontal" wall (w.r.t the map)
                {
                    x_texcoord = float_to_int(hity * wallTextureSize);
                }

                if (x_texcoord < 0) x_texcoord += wallTextureSize; //do not forget x_texcoord can be negative, fix that

                var column = texture_column(texid, x_texcoord, column_height);

                pix_x = RENDER_WIDTH / 2 + i;

                for (int j = 0; j < column_height; j++)
                {
                    pix_y = j + RENDER_HEIGHT / 2 - column_height / 2;
                    if (pix_y < 0 || pix_y >= RENDER_HEIGHT) continue;
                    frameBuffer.SetPixel(pix_x, pix_y, column[j]);
                }


                //divide por 2 pois comeca a desenhar somente na metade da tela. a primeira metade é o mapa.
                /*draw_rectangle(
                    WINDOW_WIDTH / 2 + i,
                    WINDOW_HEIGHT / 2 - column_height / 2,
                    1,
                    column_height,
                    color);*/

                break;
            }
        }
    }
    //});

    Raylib_cs.Image i2 = new Raylib_cs.Image
    {
        Format = PixelFormat.UncompressedR8G8B8A8,
        Width = RENDER_WIDTH,
        Height = RENDER_HEIGHT,
        Mipmaps = 1
    };

    Texture2D t2 = Raylib.LoadTextureFromImage(i2);

    unsafe
    {
        fixed (byte* bPtr = &frameBuffer.Buffer[0])
        {
            Raylib.UpdateTexture(t2, bPtr);
            Raylib.DrawTexture(t2, 0, 0, Raylib_cs.Color.White);            
        }
    }
}




Raylib_cs.Color[] texture_column(int texid, int texcoord, int column_height)
{
    Raylib_cs.Color[] column = new Raylib_cs.Color[column_height];

    for (int y = 0; y < column_height; y++)
    {
        /*int  pix_x = texid * wallTextureSize + texcoord;
        int pix_y = (y * wallTextureSize) / column_height;

        column[y] = To_Raylib_Color(((Bitmap)wallTextures).GetPixel(pix_x, pix_y));*/

        int pix_x = texcoord;
        int pix_y = (y * wallTextureSize) / column_height;

        column[y] = wallTextures2[texid][pix_x, pix_y];
    }

    return column;
}

int float_to_int(float num)
{
    //acredito que esse seja o comportamento da versao do artigo
    return Convert.ToInt32(MathF.Floor(num));
}

Raylib_cs.Color To_Raylib_Color(System.Drawing.Color color)
{   
    return new Raylib_cs.Color(color.R, color.G, color.B, color.A);
}