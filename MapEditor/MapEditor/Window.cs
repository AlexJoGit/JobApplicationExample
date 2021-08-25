using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    //main game window
    public class Window : GameWindow
    {
        public int selectedID = 0;

        private int VBOLamp;
        private int VAOLamp;


        private Shader _lampShader;

        private Shader _lightingShader;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private double _time;

        PhysicsHandler physHandler = new PhysicsHandler();

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title){}

        private readonly Vector3[] _cubePositions =
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(2.0f, 5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3(2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f, 3.0f, -7.5f),
            new Vector3(1.3f, -2.0f, -2.5f),
            new Vector3(1.5f, 2.0f, -2.5f),
            new Vector3(1.5f, 0.2f, -1.5f),
            new Vector3(-1.3f, 1.0f, -1.5f)
        };

        private readonly Vector3[] _pointLightPositions =
{
            new Vector3(0f, 100f, 0f),
            new Vector3(0f, 100f, 0f),
            //new Vector3(0f, 10f, 0f),
            //new Vector3(0f, 10f, 0f)
        };

        List<dGPobject> cubes = new List<dGPobject>();
        List<dLightSource> PointLights = new List<dLightSource>();

        private float value = 0;

        public void ChangeValueX(float f)
        {
            //cubes[selectedID].Rotation = new Vector3(f / 10, cubes[selectedID].Rotation.Y, cubes[selectedID].Rotation.Z);
            cubes[selectedID].Size = new Vector3( f / 10, cubes[selectedID].Size.Y, cubes[selectedID].Size.Z);
        }
        public void ChangeValueY(float f)
        {
            //cubes[selectedID].Rotation = new Vector3(cubes[selectedID].Rotation.X, f / 10, cubes[selectedID].Rotation.Z);
            cubes[selectedID].Size = new Vector3(cubes[selectedID].Size.X, f / 10, cubes[selectedID].Size.Z);
        }
        public void ChangeValueZ(float f)
        {
            //cubes[selectedID].Rotation = new Vector3(cubes[selectedID].Rotation.X, cubes[selectedID].Rotation.Y, f / 10);
            cubes[selectedID].Size = new Vector3(cubes[selectedID].Size.X, cubes[selectedID].Size.Y, f / 10);
        }


        List<float[]> verts = new List<float[]>();
        List<uint[]> inds = new List<uint[]>();


        List<string> objects = new List<string>()
        {
            "Resources/cube.obj",
            "Resources/cube2.obj",
            "Resources/cylinder.obj",
            "Resources/monkey.obj",
            "Resources/sphere.obj",
            "Resources/torus.obj",
            "Resources/floor.obj",
        };

        Dictionary<int, int[]> VAOs = new Dictionary<int, int[]>();

        void GenBuffers(ref Shader shad)
        {
            Reader rd = new Reader();

            int VAO;
            int VBO;
            int _EBO;


            foreach (string s in objects)
            {
                rd.GetData(s);

                VAO = GL.GenVertexArray();
                GL.BindVertexArray(VAO);

                VBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, rd.Vertices.Length * sizeof(float), rd.Vertices, BufferUsageHint.StaticDraw);

                _EBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, rd.Indices.Length * sizeof(uint), rd.Indices, BufferUsageHint.StaticDraw);

                var positionLocation = shad.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                var normalLocation = shad.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                var texCoordLocation = shad.GetAttribLocation("aTexCoords");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));


                VAOs.Add(VAO, new int[3] { VBO, _EBO, rd.Indices.Length });

            }
        }

        protected override void OnLoad(EventArgs e)
        {

            //Reader rd = new Reader();


            //foreach (string s in objects)
            //{
            //    rd.GetData(s);

            //    verts.Add(rd.Vertices);
            //    inds.Add(rd.Indices);
            //}
            
            GL.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");


            GenBuffers(ref _lightingShader);


            for (int i = 0; i < _cubePositions.Length; i++)
            {

                cubes.Add(new dGPobject(VAOs.ElementAt(i%VAOs.Count))
                {
                    Location = _cubePositions[i],

                    Size = Vector3.One,

                    Rotation = Vector3.Zero,

                    PhysicsEnabled = false,

                    Mass = 1,

                    Velocity = Vector3.Zero,

                    Density = 1

                });
            }

            //cubes[selectedID].PhysicsEnabled = true;



            foreach (Vector3 vloc in _pointLightPositions)
            {
                PointLights.Add(new dLightSource()
                {
                    Location = vloc,

                });
            }



            //VAOLamp = GL.GenVertexArray();
            //GL.BindVertexArray(VAOLamp);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, VBOLamp);

            //var positionLocation = _lampShader.GetAttribLocation("aPos");
            //GL.EnableVertexAttribArray(positionLocation);
            //GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);


            _camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);
            _camera.Fov = 90;

            CursorVisible = false;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";

            _time += 4.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //////////////////////////////////////////


            _lightingShader.Use();

            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _lightingShader.SetVector3("viewPos", _camera.Position);


            /*
               Here we set all the uniforms for the 5/6 types of lights we have. We have to set them manually and index
               the proper PointLight struct in the array to set each uniform variable. This can be done more code-friendly
               by defining light types as classes and set their values in there, or by using a more efficient uniform approach
               by using 'Uniform buffer objects', but that is something we'll discuss in the 'Advanced GLSL' tutorial.
            */
            // Directional light
            _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            _lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            _lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

            // Point lights
            for (int i = 0; i < _pointLightPositions.Length; i++)
            {
                _lightingShader.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
                _lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(1.05f, 1.05f, 1.05f));
                _lightingShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                _lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                _lightingShader.SetFloat($"pointLights[{i}].constant", 0.01f);
                _lightingShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                _lightingShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }

            // Spot light
            _lightingShader.SetVector3("spotLight.position", _camera.Position);
            _lightingShader.SetVector3("spotLight.direction", _camera.Front);
            _lightingShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            _lightingShader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            _lightingShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            _lightingShader.SetFloat("spotLight.constant", 1.0f);
            _lightingShader.SetFloat("spotLight.linear", 0.09f);
            _lightingShader.SetFloat("spotLight.quadratic", 0.032f);
            _lightingShader.SetFloat("spotLight.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(0.0f)));
            _lightingShader.SetFloat("spotLight.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(12.5f)));

            //////////////////////////////////////////


            foreach (dGPobject cube in cubes)
            {
                cube.Draw(ref _lightingShader, e.Time);
            }


            //////////////////////////////////////////

            //GL.BindVertexArray(VAOModel);

            //_lampShader.Use();

            //_lampShader.SetMatrix4("view", _camera.GetViewMatrix());
            //_lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            //// We use a loop to draw all the lights at the proper position
            //for (int i = 0; i < _pointLightPositions.Length; i++)
            //{
            //    Matrix4 lampMatrix = Matrix4.Identity;
            //    lampMatrix *= Matrix4.CreateScale(0.0f);
            //    lampMatrix *= Matrix4.CreateTranslation(_pointLightPositions[i]);

            //    _lampShader.SetMatrix4("model", lampMatrix);

            //    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //}


            /////////////////////////


            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused) // check to see if the window is focused
            {
                return;
            }

            foreach(dGPobject cube in cubes)
            {
                physHandler.ApplyGravity(cube);

                physHandler.ApplyAngularMomentum(cube);

                physHandler.ApplyUniversalAirResistance(cube);
                physHandler.ApplyVelocity(cube);
            }

            _pointLightPositions[1] = _camera.Position;

            HandleKeyboard((float)e.Time);

            HandleMouse();
            

            base.OnUpdateFrame(e);
        }

        private void HandleKeyboard(float Time)
        {
            KeyboardState input = Keyboard.GetState();

            float playerSpeed = 3.0f;


            if (input.IsKeyDown(Key.LShift))
            {
                playerSpeed = 7.0f;
            }
            else if (input.IsKeyUp(Key.LShift))
            {
                playerSpeed = 3.0f;
            }



            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * playerSpeed * Time; // Forward
                //player.Velocity += playerSpeed * Time * new Vector3(-_camera.Right.Z, _camera.Right.Y, _camera.Right.X);
            }
            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Front * playerSpeed * Time; // Backwards
                //player.Velocity += playerSpeed * Time * new Vector3(-_camera.Right.Z, _camera.Right.Y, _camera.Right.X)*-1;
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * playerSpeed * Time; // Left
                //player.Velocity += playerSpeed * Time * _camera.Right * -1;
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * playerSpeed * Time; // Left
                //player.Velocity += playerSpeed * Time * _camera.Right;
            }

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.AltLeft))
            {
                foct = true;
                CursorVisible = false;
            }
            else
            {
                foct = false;
                CursorVisible = true;
            }

        }

        private void HandleMouse()
        {
            MouseState mouse = Mouse.GetState();

            float sensitivity = 0.2f;


            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                if (foct)
                {
                    // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
                }
                

                
            }

        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            //if(e.Key == Key.Escape)
            //{
            //    foct = false;
            //}

            base.OnKeyDown(e);
        }

        public bool foct = true;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused && foct)
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                //dGPs[selectedID].Velocity.Y += 10.0f;
                
            }

            base.OnMouseDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.AspectRatio = Width / (float)Height;

            base.OnResize(e);
        }

        public void unloadd()
        {
            OnUnload(EventArgs.Empty);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (dGPobject cube in cubes)
            {
                GL.DeleteBuffer(cube.VBO);
                GL.DeleteVertexArray(cube.VAO);
                GL.DeleteVertexArray(cube.EBO);
                GL.DeleteTexture(cube.DiffuseMap.Handle);
                GL.DeleteTexture(cube.SpecularMap.Handle);
            }

            GL.DeleteVertexArray(VAOLamp);

            GL.DeleteProgram(_lampShader.Handle);
            GL.DeleteProgram(_lightingShader.Handle);

   
            base.OnUnload(e);
        }
    }
}