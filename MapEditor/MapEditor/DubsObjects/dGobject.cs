using Microsoft.Win32.SafeHandles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;



namespace MapEditor
{
    class dGobject : dObject
    {
        private float[] _vertices;
        private uint[] _indices;
        private int IndicesLength;

        private int _VAO;
        private int _VBO;
        private int _EBO;

        private Texture _DiffuseMap;
        public Texture DiffuseMap { get => _DiffuseMap; set => _DiffuseMap = value; }

        private Texture _SpecularMap;
        public Texture SpecularMap { get => _SpecularMap; set => _SpecularMap = value; }

        private Vector3 _Size;
        private Vector3 _Rotation;

        public int MatDif = 0;
        public int MatSpec = 1;
        public Vector3 MatSpecv3 = new Vector3(0.5f, 0.5f, 0.5f);
        public float MatShin = 32.0f;

        //public Vector3 Location { get => _Location; set => _Location = value; }
        public Vector3 Size { get => _Size; set => _Size = value; }
        public Vector3 Rotation { get => _Rotation; set => _Rotation = value; }
        public float RotationX { get => _Rotation.X; set => _Rotation.X = value; }
        public float RotationY { get => _Rotation.Y; set => _Rotation.Y = value; }
        public float RotationZ { get => _Rotation.Z; set => _Rotation.Z = value; }

        public float[] Vertices { get => _vertices; set => _vertices = value; }
        public int VAO { get => _VAO; set => _VAO = value; }
        public int VBO { get => _VBO; set => _VBO = value; }
        public uint[] Indices { get => _indices; set => _indices = value; }
        public int EBO { get => _EBO; set => _EBO = value; }

        public dGobject(float[] v, Vector3 _location, Vector3 _size, Vector3 _rotation)
        {
            _vertices = v;
            Location = _location;
            Size = _size;
            Rotation = _rotation;
        }

        public dGobject(float[] v, uint[] i, ref Shader shad)
        {
            DiffuseMap = new Texture("Resources/WoodBox.png");
            SpecularMap = new Texture("Resources/container2_specular.png");

            Vertices = v;
            Indices = i;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);


            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);



            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            var positionLocation = shad.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = shad.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = shad.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            //GL.BindVertexArray(0);

        }

        public dGobject(KeyValuePair<int,int[]> kv)
        {
            DiffuseMap = new Texture("Resources/WoodBox3d.png");
            SpecularMap = new Texture("Resources/container2_specular.png");

            VAO = kv.Key;
            VBO = kv.Value[0];
            EBO = kv.Value[1];
            IndicesLength = kv.Value[2];
        }

        public void Draw(ref Shader shad, double tim)
        {
            DiffuseMap.Use();
            SpecularMap.Use(TextureUnit.Texture1);

            shad.Use();

            GL.BindVertexArray(VAO);
            

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);

            shad.SetInt("material.diffuse", MatDif);
            shad.SetInt("material.specular", MatSpec);
            shad.SetVector3("material.specular", MatSpecv3);
            shad.SetFloat("material.shininess", MatShin);

            var model = Matrix4.Identity;
            model *= Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(tim));
            model *= Matrix4.CreateRotationX(Rotation.X);
            model *= Matrix4.CreateRotationY(Rotation.Y);
            model *= Matrix4.CreateRotationZ(Rotation.Z);
            model *= Matrix4.CreateScale(Size);
            model *= Matrix4.CreateTranslation(Location);

            shad.SetMatrix4("model", model);

            //GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length/8);

            GL.DrawElements(PrimitiveType.Triangles, IndicesLength, DrawElementsType.UnsignedInt, 0);
            
            //GL.DrawElements(BeginMode.Quads, indecies.count, DrawElementsType.UnsignedInt, 8)

        }
    }
}
