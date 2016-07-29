using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace RED
{
    class Renderer
    {
        static void RenderSphere(BulletSharp.RigidBody sphere)
        {
            //check if a sphere
            if (sphere.CollisionShape.ShapeType != BulletSharp.BroadphaseNativeType.SphereShape)
            {
                return;
            }

            //red
            GL.Color3(1, 0, 0);

            //get radius
            float r = ((BulletSharp.SphereShape)sphere.CollisionShape).Radius;

            //get transform
            BulletSharp.Math.Matrix trans;
            sphere.MotionState.GetWorldTransform( out trans );

            //trans.M11

            //OpenTK.Matrix4
            
            /*
            GL.PushMatrix();
            GL.MultMatrix();
            GL.PopMatrix();
            */
        }
    }
}
