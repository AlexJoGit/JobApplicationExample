using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MapEditor
{
    class PhysicsHandler
    {
        Vector3 gravity = new Vector3(0.0f, 0.03f, 0.0f);

        public void ApplyGravity(dGPobject obj)
        {
            if (obj.PhysicsEnabled)
            {
                if (obj.LocationY + obj.Velocity.Y > 0)
                {
                    obj.Velocity -= gravity;
                }
                else
                {
                    obj.LocationY = 0;
                }
            }

           
            
        }


        public void checkIntersections()
        {

        }

        public void ApplyAngularMomentum(dGPobject obj)
        {
            if (!obj.PhysicsEnabled)
            {
                if (obj.Rotation.X + obj.AngularMomentum.X < 359)
                {
                    obj.RotationX += obj.AngularMomentum.X;
                }
                else
                {
                    obj.RotationX = obj.AngularMomentum.X;
                }

                if (obj.Rotation.Y + obj.AngularMomentum.Y < 359)
                {
                    obj.RotationY += obj.AngularMomentum.Y;
                }
                else
                {
                    obj.RotationY = obj.AngularMomentum.Y;
                }

                if (obj.Rotation.Z + obj.AngularMomentum.Z < 359)
                {
                    obj.RotationZ += obj.AngularMomentum.Z;
                }
                else
                {
                    obj.RotationZ = obj.AngularMomentum.Z;
                }
            }
        }

        public void ApplyUniversalAirResistance(dGPobject obj)
        {
            float off = 0.01f;
            float offX;
            float offY;
            float offZ;

            if(obj.Velocity.X > 0.01f)
            {
                offX = obj.Velocity.X - off;
            }
            else if(obj.Velocity.X < -0.01f)
            {
                offX = obj.Velocity.X + off;
            }
            else
            {
                offX = 0;
            }
            if (obj.Velocity.Y > 0.01f)
            {
                offY = obj.Velocity.Y - off;
            }
            else if(obj.Velocity.Y < -0.01f)
            {
                offY = obj.Velocity.Y + off;
            }
            else
            {
                offY = 0;
            }
            if (obj.Velocity.Z > 0.01f)
            {
                offZ = obj.Velocity.Z - off;
            }
            else if(obj.Velocity.Z < -0.01f)
            {
                offZ = obj.Velocity.Z + off;
            }
            else
            {
                offZ = 0;
            }


            obj.Velocity = new Vector3(offX, offY, offZ);
        }

        public void ApplyVelocity(dGPobject obj)
        {
            obj.Location += obj.Velocity;
        }
    }
}
