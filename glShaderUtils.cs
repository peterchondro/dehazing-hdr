using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_sample
{
    public enum texture_unit_t
    {
        FRONT_CAMERA = 4, //program_main
        BACK_CAMERA = 5,
        LEFT_CAMERA = 6,
        RIGHT_CAMERA = 7,
        MAIN_ALPHA = 8,
        MAIN_COLOR = 9,
        MAIN_COLOR2 = 10,

        CAR_TEXTURE = 11,   //program_car
        CAR_GLOSSISESS = 12,
        CAR_SPECULAR = 13,

        AVM_2D_TEXTURE = 14,   //2D avm fbo
        RGBA_TEXTURE = 15, //program_rgba

        AVM_FBO = 16,  //rgba to uyvy fbo
        SEE_THROUGH = 17,  //see through - fbo3 fbo
        SEE_THROUGH_RESULT = 18,  //see through - fbo_seethrough fbo
        RGBA_2_Y = 19,  //mod(rgba to y) fbo

        FORMAT_TEX = 20,
        FBO_ADAS = 22,
        FBO_ADAS_Y = 23,

        LEFT_BSD_CAMERA = 24,
        RIGHT_BSD_CAMERA = 25,

        MSAA_FBO = 26,
        CAR_NORMAL = 26,
        CAR_SKY = 27,

        // FBO_FISH = 20,  //stop&go fbo
        // FBO_BSD = 21,  //bsd fbo

    }
    ;

    class glShaderUtils
    {
        static int create_shader(ShaderType shaderType, string shader)
        {
            int isShaderCompiled;
            int shaderValue = GL.CreateShader(shaderType);
            // 檢查 shader是否創建成功。失敗為0
            if (shaderValue == 0)
            {
                MessageBox.Show("glCreateShader failure type =  " + shaderType.ToString() + ", value = " + shaderValue);
                return -1;
            }


            GL.ShaderSource(shaderValue, shader);
            GL.CompileShader(shaderValue);
            // Check compilation
            GL.GetShader(shaderValue, ShaderParameter.CompileStatus, out isShaderCompiled);
            if (isShaderCompiled != 1)
            {
                int infoLenght;
                GL.GetShader(shaderValue, ShaderParameter.InfoLogLength, out infoLenght);
                if (infoLenght > 0)
                {
                    string vShaderInfo = GL.GetShaderInfoLog(shaderValue);
                    if (vShaderInfo.Length > 0)
                    {
                        MessageBox.Show("Create shader VertexShader error : \n" + vShaderInfo);
                        return -1;
                    }
                }
            }

            return shaderValue;
        }

        internal static int create_program(string v, string f)
        {
            // AVM_LOGI("createProgram v = %s, f = %s\n", v, f);
            int vetexShader = create_shader(ShaderType.VertexShader, v);
            if (vetexShader < 0) return -1;

            int fragShader = create_shader(ShaderType.FragmentShader, f);
            if (fragShader < 0) return -1;

            int program = GL.CreateProgram();
            GL.AttachShader(program, vetexShader);
            GL.AttachShader(program, fragShader);
            GL.LinkProgram(program);

            return program;
        }

    }
}
