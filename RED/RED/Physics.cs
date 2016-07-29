using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BulletSharp; //for physic bodies

namespace RED
{
    class Physics
    {
        private CollisionConfiguration collisionConf;
        public DiscreteDynamicsWorld world;
        private Dispatcher dispatcher;
        private BroadphaseInterface broadphase;

        //list with all bodies
        public static List<RigidBody> bodies = new List<RigidBody> { };

        //constructor
        public Physics()
        {
            collisionConf = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConf);

            broadphase = new DbvtBroadphase();
            world = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
            world.Gravity = new BulletSharp.Math.Vector3(0, -10, 0);
        }

        public RigidBody AddPlane()
        {
            //place plane at 0,0,0 with angles along global axis
            BulletSharp.Math.Matrix transform = BulletSharp.Math.Matrix.Identity;
            //set shape of ground (infident in all dirrections)
            //face y dirrection
            float distFromOrigin = -1.0f;
            StaticPlaneShape plane = new StaticPlaneShape(new BulletSharp.Math.Vector3(0, 1, 0), distFromOrigin);
            //assign motion state to plane (for setting the location)
            MotionState motion = new DefaultMotionState(transform);


            //set physic properties
            float planeMass = 0.0f; //zero means its a static object
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(planeMass, motion, plane);

            //create the body
            RigidBody planeBody = new RigidBody(info);
            //add it to the world
            world.AddRigidBody(planeBody);
            //add it to a list so I can access all its info
            bodies.Add(planeBody);

            return planeBody;
        }

        public RigidBody AddSphere(float rad, BulletSharp.Math.Matrix startTransform, float mass)
        {

            //rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = mass != 0.0f;
            SphereShape sphere = new SphereShape(rad);
            DefaultMotionState motionState = new DefaultMotionState(startTransform);
            //motionState.StartWorldTrans = startTransform;

            BulletSharp.Math.Vector3 inertial = new BulletSharp.Math.Vector3(0, 0, 0);

            if (isDynamic)
            {
                sphere.CalculateLocalInertia(mass, out inertial);
            }

            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, motionState, sphere, inertial);
            RigidBody body = new RigidBody(info);
            info.Dispose();
            world.AddRigidBody(body);
            bodies.Add(body);

            return body;
        }

        public virtual void Update(float elapsedTime)
        {
            world.StepSimulation(elapsedTime);
        }

        public void ExitPhysics()
        {
            //remove constraints
            for (int i = world.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = world.GetConstraint(i);
                //remove from world
                world.RemoveConstraint(constraint);
                //release memory
                constraint.Dispose();
            }

            //remove rigidbodies
            for (int i = world.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = world.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                //remove from world
                world.RemoveCollisionObject(obj);
                //release memory
                obj.Dispose();
            }

            //release collision shapes
            foreach (RigidBody body in bodies)
            {
                body.Dispose();
            }
            bodies.Clear();

            //delete world
            world.Dispose();
            broadphase.Dispose();
            dispatcher?.Dispose();
            collisionConf.Dispose();
        }

    }
}
