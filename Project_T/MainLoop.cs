using System;
using Silk.NET.GLFW;
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using System.Collections.Generic;

namespace silkgl
{
    public class MainLoop
    {
        private static IWindow window;
        private static GL gl;
        private static uint vao;
        private static Shader shaderObject;
        private static uint[] objectArray = new uint[3];


        private static List<VertexArray<float, uint>> savedTriangles;

        private enum Direction
        {
            Up, Down, Left, Right
        }
        private static VertexArray<float, uint>[] outlines = new VertexArray<float, uint>[4];
        private static Direction currentDirection = Direction.Up;

        private static IInputContext input;


        //Vertex data, uploaded to the VBO.
        private static float[] Vertices =
        {
            //X    Y      Z
            0.5f,  0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f
        };

        //Index data, uploaded to the EBO.
        private static uint[] Indices =
        {
            0, 1, 2,
            3, 0, 2
        };
        
        
        public static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Project_T",
                API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 1))
            };
            
            window = Window.Create(windowOptions);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;
            
            window.Run();
            
            window.Dispose();
            
        }

        public static unsafe void OnLoad()
        {
            gl = GL.GetApi(window);
            input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
            
            
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            gl.Clear(ClearBufferMask.ColorBufferBit);

            
           objectArray = Renderer.InitVertexArray(ref gl, ref Vertices, ref Indices);
           vao = objectArray[0];

           shaderObject = new Shader("Shaders/VertexShaderTest.glsl", 
               "Shaders/FragmentShaderTest.glsl",
               gl);
           
            
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);
        }

        public static void OnUpdate(double obj)
        {
            
        }
        
        public static unsafe void OnRender(double obj)
        {
            gl.Clear(ClearBufferMask.ColorBufferBit);
            
            gl.BindVertexArray(vao);
            shaderObject.Use();
            
            gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);


            VertexArray<float, uint> movedOutline = new VertexArray(outlines[(uint)currentDirection], new Vector2D<float>(input.Mice[0].Position.X, input.Mice[0].Position.Y));
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            Console.WriteLine("Closing...");
            
            gl.DeleteBuffer(objectArray[1]);
            gl.DeleteBuffer(objectArray[2]);
            gl.DeleteVertexArray(vao);
            shaderObject.Destroy();
        }


        private static void KeyDown(IKeyboard keyboard, Key key, int arg)
        {
            switch (key)
            {
                case Key.W:
                    currentDirection = Direction.Up;
                    break;
                case Key.S:
                    currentDirection = Direction.Down;
                    break;
                case Key.A:
                    currentDirection = Direction.Left;
                    break;
                case Key.D:
                    currentDirection = Direction.Right;
                    break;

                case Key.Space:
                    AddTriangle();
                    break;
            }
        }

        private static void AddTriangle()
        {
            savedTriangles.Add(new VertexArray(outlines[(uint)currentDirection], new Vector2D<float>(input.Mice[0].Position.X, input.Mice[0].Position.Y)));
        }
    }
}
