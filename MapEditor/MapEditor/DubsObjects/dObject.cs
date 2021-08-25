using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MapEditor
{
    class dObject
    {
        private Vector3 _Location;

        public Vector3 Location { get => _Location; set => _Location = value; }
        public float LocationX { get => _Location.X; set => _Location.X = value; }
        public float LocationY { get => _Location.Y; set => _Location.Y = value; }
        public float LocationZ { get => _Location.Z; set => _Location.Z = value; }

    }
}
