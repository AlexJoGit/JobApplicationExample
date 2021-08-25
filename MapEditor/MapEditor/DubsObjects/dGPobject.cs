using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.IO;
using System.Windows.Forms;

namespace MapEditor
{
    class dGPobject : dGobject
    {
        private bool _PhysicsEnabled;
        private float _Mass;
        private Vector3 _Velocity;
        private Vector3 _Momentum;
        private Vector3 _AngularMomentum;
        private float _Density;


        public bool PhysicsEnabled { get => _PhysicsEnabled; set => _PhysicsEnabled = value; }
        public float Mass { get => _Mass; set => _Mass = value; }
        public Vector3 Velocity { get => _Velocity; set => _Velocity = value; }
        public Vector3 Momentum { get => _Momentum; set => _Momentum = value; }
        public Vector3 AngularMomentum { get => _AngularMomentum; set => _AngularMomentum = value; }
        public float Density { get => _Density; set => _Density = value; }

        
        public dGPobject(dGobject _base, bool _physicsEnabled, float _mass, Vector3 _velocity, float _density) : base(_base.Vertices, _base.Location, _base.Size, _base.Rotation)
        {
            PhysicsEnabled = _physicsEnabled;
            Mass = _mass;
            Velocity = _velocity;
            Density = _density;

            Momentum = Mass * Velocity;
        }

        public dGPobject(float[] _v, uint[] i, ref Shader _shad) : base (_v, i, ref _shad)
        {

        }

        public dGPobject(KeyValuePair<int,int[]> kv) : base(kv)
        {
            //AngularMomentum = new Vector3(0.01f, 0.01f, 0.01f);
        }

        private bool canJump = false;
        private float jmpCDR = 0;

        public float JmpCDR { get => jmpCDR; set => jmpCDR = value; }
        public bool CanJump { get => canJump; set => canJump = value; }


        public void Jump()
        {
            if (CanJump && JmpCDR == 0)
            {
                _Velocity.Y += 5.0f;
                JmpCDR = 40;
                CanJump = false;
            }
        }
    }
}
