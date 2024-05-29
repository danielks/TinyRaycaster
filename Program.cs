using Raylib_cs;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

const int RENDER_WIDTH = 512;
const int RENDER_HEIGHT = 512;
const int WINDOW_WIDTH = RENDER_WIDTH;
const int WINDOW_HEIGHT = RENDER_WIDTH;

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

Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Tiny Raytracer");

Byte[] buffer = new byte[RENDER_WIDTH * RENDER_HEIGHT * 4]; //4 bytes per pixel

long frameCount = 0;

Stopwatch sw = new Stopwatch();
double frameTime = sw.Elapsed.TotalMilliseconds;


while (!Raylib.WindowShouldClose())
{
    sw.Restart();

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);

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
    for (int j = 0; j < RENDER_HEIGHT; j++)
    //Parallel.For(0, renderHeight, new ParallelOptions { MaxDegreeOfParallelism = 6 }, ty =>

    {
        for (int i = 0; i < RENDER_WIDTH; i++)
        //Parallel.For(0, renderWidth, x =>    
        {
            //o y no Raylib comeca de cima pra baixo. no GLSL comeca de baixo pra cima. entao trocamos aqui.


            Color pixel_color = Color.Red;

            byte r = Convert.ToByte(255.0f * ((float)j / (float)RENDER_HEIGHT)); // varies between 0 and 255 as j sweeps the vertical
            byte g = Convert.ToByte(255.0f * ((float)i / (float)RENDER_WIDTH)); // varies between 0 and 255 as i sweeps the horizontal
            byte b = 0;
            pixel_color = new Color(r, g, b, (byte)255);

            //framebuffer[i + j * win_w] = pack_color(r, g, b);

            write_color(i, j, pixel_color);
        }
        //});
    }
    //});

    const int rect_w = RENDER_WIDTH / map_w;
    const int rect_h = RENDER_HEIGHT / map_h;

    for (int j = 0; j < map_h; j++)
    {
        for (int i = 0; i < map_w; i++)
        {
            if (map[i + j * map_w] == ' ') continue;

            int rect_x = i * rect_w;
            int rect_y = j * rect_h;

            draw_rectangle(rect_x, rect_y, rect_w, rect_h, new Color(0, 255, 255, 255));
        }
    }

    //for 

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
            Raylib.DrawTexture(t2, 0, 0, Color.White);            
        }
    }
}


void write_color(int x, int y, Color pixel_color)
{   
    int idx = (y * RENDER_WIDTH + x) * 4;

    buffer[idx] = pixel_color.R;
    buffer[idx + 1] = pixel_color.G;
    buffer[idx + 2] = pixel_color.B;
    buffer[idx + 3] = 255;
}

void draw_rectangle(int x, int y, int w, int h, Color color)
{
    
    for (int i = 0; i < w; i++)
    {
        for (int j = 0; j < h; j++)
        {
            int cx = x + i;
            int  cy = y + j;
            
            write_color(cx, cy, color);            
        }
    }
}