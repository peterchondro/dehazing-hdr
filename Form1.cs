using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Claims;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Runtime.InteropServices;



namespace OpenTK_sample
{
    public partial class Form1 : Form
    {
        Program_shadow program_preprocess= new Program_shadow();
        public Form1()
        {
            InitializeComponent();
           
        }


        private void glControl_Load(object sender, EventArgs e)
        {
            Initgl();
            glControl.MakeCurrent();
        }

        private void Initgl()
        {
            program_preprocess.init_program();
            program_preprocess.init_carShadow();
        }


        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            routine();
            glControl.SwapBuffers();
        }

        public void routine()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            GL.Viewport(0, 0, glControl.Width/2, glControl.Height);
            program_preprocess.SettingImgId(0, 0);
            program_preprocess.draw_Shadow();
            GL.Viewport(glControl.Width/2 + 1, 0, glControl.Width/2, glControl.Height);
            program_preprocess.SettingImgId(0, 1);
            program_preprocess.draw_Shadow();
        }

    }
}
