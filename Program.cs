using Raylib_cs;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;




const int RENDER_WIDTH = 1000;
const int RENDER_HEIGHT = 500;
const int WINDOW_WIDTH = RENDER_WIDTH;
const int WINDOW_HEIGHT = RENDER_HEIGHT;

float player_x = 3.456f;
float player_y = 2.345f;
float player_a = 1.523f; //angle the player is facing (the angle between the view direction and the x axis).
const float fov = MathF.PI / 3.0f;

string wallTexturesFilePath = @"C:\Users\danie\source\repos\TinyRaycaster\resources\textures.bmp";

var wallTextures = Bitmap.FromFile(wallTexturesFilePath);

int wallTextureSize = wallTextures.Height; //square
int wallTexturesCount = wallTextures.Width / wallTextures.Height;





const int map_w = 16; // map width
const int map_h = 16; // map height
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

Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Tiny Raycaster");

Byte[] buffer = new byte[RENDER_WIDTH * RENDER_HEIGHT * 4]; //4 bytes per pixel

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
    Array.Clear(buffer, 0, buffer.Length);

    const int rect_w = RENDER_WIDTH / (map_w * 2); //render map on half the screen
    const int rect_h = RENDER_HEIGHT / map_h;

    for (int j = 0; j < map_h; j++)
    {
        for (int i = 0; i < map_w; i++)
        {
            char cell = map[i + j * map_w];
            if (cell == ' ') continue;

            int rect_x = i * rect_w;
            int rect_y = j * rect_h;

            int cellInt = (int)cell - (int)'0';

            var color = To_Raylib_Color(((Bitmap)wallTextures).GetPixel(cellInt * wallTextureSize, 0));

            draw_rectangle(rect_x, rect_y, rect_w, rect_h, color);
        }
    }

    player_a += 0.0005f;

    //draw the player on the map
    draw_rectangle(Convert.ToInt32(player_x * rect_w), Convert.ToInt32(player_y * rect_h), 5, 5, new Raylib_cs.Color(255, 255, 255, 255));

    //draw the visibility cone
    for (int i = 0; i < RENDER_WIDTH / 2; i++)
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
            write_color(pix_x, pix_y, new Raylib_cs.Color(160, 160, 160, 255));

            char cell = map[float_to_int(cx) + float_to_int(cy) * map_w];

            

            //our ray touches a wall, so draw the vertical column to create an illusion of 3D.
            if (cell != ' ')
            {
                int cellInt = (int)cell - (int)'0';
                int column_height = float_to_int(RENDER_HEIGHT / (t * MathF.Cos(angle - player_a)));

                var color = To_Raylib_Color(((Bitmap)wallTextures).GetPixel(cellInt  * wallTextureSize, 0));
                    

                //divide por 2 pois comeca a desenhar somente na metade da tela. a primeira metade é o mapa.
                draw_rectangle(
                    WINDOW_WIDTH / 2 + i,
                    WINDOW_HEIGHT / 2 - column_height / 2,
                    1,
                    column_height,
                    color);

                break;
            }
        }
    }

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
        fixed (byte* bPtr = &buffer[0])
        {
            Raylib.UpdateTexture(t2, bPtr);
            Raylib.DrawTexture(t2, 0, 0, Raylib_cs.Color.White);            
        }
    }
}


void write_color(int x, int y, Raylib_cs.Color pixel_color)
{   
    int idx = (y * RENDER_WIDTH + x) * 4;

    buffer[idx] = pixel_color.R;
    buffer[idx + 1] = pixel_color.G;
    buffer[idx + 2] = pixel_color.B;
    buffer[idx + 3] = 255;
}

void draw_rectangle(int x, int y, int w, int h, Raylib_cs.Color color)
{
    
    for (int i = 0; i < w; i++)
    {
        for (int j = 0; j < h; j++)
        {
            int cx = x + i;
            int cy = y + j;

            if (cx >= RENDER_WIDTH || cy >= RENDER_HEIGHT) continue;
            
            write_color(cx, cy, color);            
        }
    }
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