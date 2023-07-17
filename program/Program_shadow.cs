using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace OpenTK_sample
{
    public struct color
    {
        public float r;
        public float g;
        public float b;
        public color(int non)
        {
            r = 0.5f;
            g = 0.5f;
            b = 0.5f;
        }
    }
    internal class Program_shadow
    {
        private int[] carShadow_vao = new int[2];
        public int[][] carShadow_vbo = new int[4][];

        private int[] carShadowProgram = new int[2];
        private int histogramLocation;

        private float exposure = 1.025f;
        private float gamma = 1.025f;
        private float weight = 0.55f;
        private float nmax = 0.9412f;
        private float cmax = 0.9098f;
        public int windows = 1;
        public int[] textureId;

        Bitmap[] rawimage;

        public int imgId;
        public int imgWindows;

        public color shadow_color = new color(0);

        public readonly string[] itemname = { "original", "hdr" };

        string fragmentShaderSource =
            @"  #version 330 core
                precision highp float;
                out vec4 fragColor;

                in vec2 TexCoord;
                uniform sampler2D Texture;
                uniform int windows;
                uniform float histogram[256];
                uniform float exposure, gamma;
                uniform float weight, nmax, cmax;

                float findMin(float a, float b, float c)
                {
                    return min(a, min(b, c));
                }

                void main(void)
                {
                    mediump vec4 rgbTex;
                    mediump vec3 enhTex;
                    if(windows == 0)
                    {
                        rgbTex = texture(Texture, TexCoord);
                        fragColor = vec4(rgbTex);
                    }
                    else if(windows == 1)
                    {
                        rgbTex = texture(Texture, TexCoord);
                        float gray = dot(rgbTex.rgb, vec3(0.299, 0.587, 0.114));
                        float cumulativeHist[256], adjustedHist[256];
                        cumulativeHist[0] = histogram[0];
                        for (int i = 1; i < 256; i++)
                        {
                            cumulativeHist[i] = cumulativeHist[i - 1] + histogram[i];
                            adjustedHist[i] = pow(cumulativeHist[i], 1.0 / exposure) * gamma;
                        }
                        float equalizedHistR = adjustedHist[int(rgbTex.r * 255.0)];
                        float equalizedHistG = adjustedHist[int(rgbTex.g * 255.0)];
                        float equalizedHistB = adjustedHist[int(rgbTex.b * 255.0)];

                        float transdata = 1.0 - weight * findMin(equalizedHistR, equalizedHistG, equalizedHistB);
                        if (transdata < 0.1)
                            transdata = 0.1;
                        float t_c = float(cmax) * transdata - float(nmax);
                        float dehazedR = (equalizedHistR + t_c) / transdata;
                        float dehazedG = (equalizedHistG + t_c) / transdata;
                        float dehazedB = (equalizedHistB + t_c) / transdata;

                        dehazedR = clamp(dehazedR, 0.0, 1.0);
                        dehazedG = clamp(dehazedG, 0.0, 1.0);
                        dehazedB = clamp(dehazedB, 0.0, 1.0);

                        fragColor = mix(rgbTex, vec4(dehazedR, dehazedG, dehazedB, rgbTex.a), 0.25);
                    }
                }
            ";

        string vertexShaderSource =
            @"  #version 330 core
                in vec3 Position;
                in vec2 InTexCoord;

                out vec2 TexCoord;

                void main(void)
                {
                    gl_Position = vec4(Position, 1.0);
                    TexCoord = InTexCoord;
                }
            ";

        public Program_shadow()
        {
            carShadow_vbo[0] = new int[2];
            rawimage = new Bitmap[2];
            textureId = new int[2];
            imgId = 0;
            imgWindows = 0;
        }

        public void init_2D_image_texture()
        {
            int i = 0;
            foreach (var item in itemname)
            {
                rawimage[i] = new Bitmap("input_img/" + item + ".png");
                textureId[i] = GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, textureId[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, rawimage[i].Width, rawimage[i].Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

                BitmapData data = rawimage[i].LockBits(new Rectangle(0, 0, rawimage[i].Width, rawimage[i].Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, rawimage[i].Width, rawimage[i].Height,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                rawimage[i].UnlockBits(data);

                // 設定紋理參數
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                //rawimage[i].Dispose();
                i++;
            }
        }

        public void init_carShadow()
        {
            init_2D_image_texture();
            GL.GenVertexArrays(1, carShadow_vao);

            float[] vertexCoordView2d = new float[18]
    {

                -1.000000F ,    -1.000000F,       0.100000F,
                 1.000000F ,    -1.000000F,       0.100000F,
                 1.000000F ,     1.000000F,        0.100000F,

                1.000000F ,  1.000000F,           0.100000F,
                -1.000000F  ,1.000000F,         0.100000F,
                -1.000000F  ,-1.000000F,        0.100000F,
   };

            float[] fragmentCoordView2d = new float[12]
               {
                0.00000F,  1.00000F,    1.00000F,
                1.00000F,  1.00000F,    0.00000F,
                1.00000F,  0.00000F,    0.00000F,
                0.00000F,  0.00000F,    1.00000F,
           };

            int glAttrAvmVertex = GL.GetAttribLocation(carShadowProgram[0], "Position");
            int glAttrAvmTexture = GL.GetAttribLocation(carShadowProgram[0], "InTexCoord");



            GL.GenBuffers(3, carShadow_vbo[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, carShadow_vbo[0][0]);
            GL.BufferData(BufferTarget.ArrayBuffer, 6 * 3 * sizeof(float), vertexCoordView2d, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, carShadow_vbo[0][1]);
            GL.BufferData(BufferTarget.ArrayBuffer, 6 * 2 * sizeof(float), fragmentCoordView2d, BufferUsageHint.StaticDraw);


            GL.BindVertexArray(carShadow_vao[0]);

            GL.EnableVertexAttribArray(glAttrAvmVertex);
            GL.BindBuffer(BufferTarget.ArrayBuffer, carShadow_vbo[0][0]);
            GL.VertexAttribPointer(glAttrAvmVertex, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(glAttrAvmTexture);
            GL.BindBuffer(BufferTarget.ArrayBuffer, carShadow_vbo[0][1]);
            GL.VertexAttribPointer(glAttrAvmTexture, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void init_program()
        {
            carShadowProgram[0] = glShaderUtils.create_program(vertexShaderSource, fragmentShaderSource);
            if (carShadowProgram[0] < 0) MessageBox.Show("Create Program[0] fail!!");
        }

        public void SettingImgId(int id, int windows)
        {
            imgId = id;
            imgWindows = windows;
        }
        private void GetImageSize(Bitmap image, out int width, out int height)
        {
            width = image.Width;
            height = image.Height;
        }

        public void draw_Shadow()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.UseProgram(carShadowProgram[0]);
            GL.Uniform1(GL.GetUniformLocation(carShadowProgram[0], "Texture"), imgId);
            GL.Uniform1(GL.GetUniformLocation(carShadowProgram[0], "windows"), imgWindows);
            GL.Uniform1(GL.GetUniformLocation(carShadowProgram[0], "exposure"), exposure);
            GL.Uniform1(GL.GetUniformLocation(carShadowProgram[0], "gamma"), gamma);
            GL.Uniform1(GL.GetUniformLocation(carShadowProgram[0], "weight"), weight);
            GL.Uniform1(GL.GetUniformLocation(carShadowProgram[0], "NMAX"), nmax);
            GL.Uniform1(GL.GetUniformLocation(carShadowProgram[0], "CMAX"), cmax);

            float[] histogram = new float[256];
            for (int k = 0; k < 256; k++)
            {
                histogram[k] = 0;
            }
            if (imgWindows == 1)
            {
                for (int y = 0; y < rawimage[1].Height; y++)
                {
                    for (int x = 0; x < rawimage[1].Width; x++)
                    {
                        Color color = rawimage[1].GetPixel(x, y);  
                        int gray = (color.R + color.G + color.B) / 3;
                        histogram[gray]++;
                    }
                }
                int pixelCount = rawimage[1].Width * rawimage[1].Height;
                for (int i = 0; i < histogram.Length; i++)
                {
                    histogram[i] /= pixelCount;
                }
            }
            for (int i = 0; i < histogram.Length; i++)
            {
                GL.Uniform1(histogramLocation + i, histogram[i]);
            }
            GL.Uniform2(GL.GetUniformLocation(carShadowProgram[0], "textureSize"), new Vector2(rawimage[1].Width, rawimage[1].Height));
            GL.ActiveTexture(TextureUnit.Texture0 + imgId);
            GL.BindTexture(TextureTarget.Texture2D, textureId[imgId]);

            GL.BindVertexArray(carShadow_vao[0]);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
    }
}
