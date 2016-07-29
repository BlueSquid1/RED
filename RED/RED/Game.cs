using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RED
{
    class Game
    {
        protected Physics physicsEngine;
        protected OpenTK.GameWindow game;

        protected float frameTime;
        protected int fps;
        private float angle = 0.0f;

        float[] normals = new float[] {0,0,1,  0,0,1,  0,0,1,  0,0,1,
            1,0,0,  1,0,0,  1,0,0, 1,0,0,
            0,1,0,  0,1,0,  0,1,0, 0,1,0,
            -1,0,0,  -1,0,0, -1,0,0,  -1,0,0,
            0,-1,0,  0,-1,0,  0,-1,0,  0,-1,0,
            0,0,-1,  0,0,-1,  0,0,-1,  0,0,-1};

        byte[] indices = {0,1,2,3,
            4,5,6,7,
            8,9,10,11,
            12,13,14,15,
            16,17,18,19,
            20,21,22,23};

        float[] vertices = new float[] {1,1,1,  -1,1,1,  -1,-1,1,  1,-1,1,
            1,1,1,  1,-1,1,  1,-1,-1,  1,1,-1,
            1,1,1,  1,1,-1,  -1,1,-1,  -1,1,1,
            -1,1,1,  -1,1,-1,  -1,-1,-1,  -1,-1,1,
            -1,-1,-1,  1,-1,-1,  1,-1,1,  -1,-1,1,
            1,-1,-1,  -1,-1,-1,  -1,1,-1,  1,1,-1};




        public Game()
        {
            //create a window
            this.game = new OpenTK.GameWindow(800,600, new OpenTK.Graphics.GraphicsMode(), "RED");
            this.game.VSync = VSyncMode.Off;

            //initalize Physics engine
            this.physicsEngine = new Physics();

            this.game.Load += LoadResources;
            this.game.UpdateFrame += UpdateFrame;
            this.game.Unload += Unload;
            this.game.RenderFrame += RenderFrame;
            this.game.Resize += windowResize;
        }

        void LoadResources(object sender, EventArgs e)
        {
            //add the ground
            physicsEngine.AddPlane();

            //add a sphere
            BulletSharp.Math.Matrix transformSphere = BulletSharp.Math.Matrix.Identity;
            transformSphere.M42 = 20;
            physicsEngine.AddSphere(1.0f, transformSphere, 1.0f);
        }

        void windowResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, game.Width, game.Height);
        }



        protected void UpdateFrame(object sender, FrameEventArgs e)
        {
            //handle events
            physicsEngine.Update((float)e.Time);


            OpenTK.Input.KeyboardState state = OpenTK.Input.Keyboard.GetState();
            if (state.IsKeyDown(OpenTK.Input.Key.Escape))
            {
                //quit
                this.game.Exit();
            }
            else
            {
                //state.
            }

            //proccess data

        }

        protected void RenderFrame(object sender, FrameEventArgs e)
        {
            //update title
            frameTime += (float)e.Time;
            fps++;
            if (frameTime >= 1)
            {
                frameTime = 0;
                this.game.Title = "RED, FPS = " + fps.ToString();
                fps = 0;
            }

            
            GL.Viewport(0, 0, game.Width, game.Height);

            float aspect_ratio = (float)game.Width / (float)game.Height;
            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 0.1f, 100);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            OpenTK.Matrix4 lookat = OpenTK.Matrix4.LookAt(new OpenTK.Vector3(10, 20, 30), OpenTK.Vector3.Zero, OpenTK.Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);

            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
            angle += (float)e.Time * 100;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            InitCube();

            foreach (BulletSharp.RigidBody body in physicsEngine.world.CollisionObjectArray)
            {
                OpenTK.Matrix4 modelLookAt = Convert(body.MotionState.WorldTransform) * lookat;
                GL.LoadMatrix(ref modelLookAt);
                /*
                if ("Ground".Equals(body.UserObject))
                {
                    //DrawCube(Color.Green, 50.0f);
                    continue;
                }
                */

                if (body.ActivationState == BulletSharp.ActivationState.ActiveTag)
                    DrawCube2(Color.Orange, 1);
                else
                    DrawCube2(Color.Red, 1);
            }

            UninitCube();

            this.game.SwapBuffers();
        }



        protected void Unload(object sender, System.EventArgs e)
        {
            physicsEngine.ExitPhysics();
        }

        public void Run(double fps)
        {
            this.game.Run(fps);
        }

        private void DrawCube2(Color color, float size)
        {
            GL.Color3(color);
            GL.DrawElements(PrimitiveType.Quads, 24, DrawElementsType.UnsignedByte, indices);
        }


        private void InitCube()
        {
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.NormalPointer(NormalPointerType.Float, 0, normals);
            GL.VertexPointer(3, VertexPointerType.Float, 0, vertices);
        }

        private void UninitCube()
        {
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
        }

        private static Matrix4 Convert(BulletSharp.Math.Matrix m)
        {
            return new Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }

    }
}
