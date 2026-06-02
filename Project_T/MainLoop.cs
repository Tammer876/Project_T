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
        private static Shader shaderObject;

        private static Vector2D<int> screenSize = new(800, 600);


        private static List<VertexArray<uint>> savedTriangles = new();

        private enum Direction
        {
            Up, Down, Left, Right
        }
        private static VertexArray<uint>[] outlines = new VertexArray<uint>[4];
        private static Direction currentDirection = Direction.Up;

        private static IInputContext input;


        private static Vector2D<float> mousePos;


        private static float[] OutlineUpV =
        {
            //X    Y      Z
            0f,  0.1f, 0.0f,
            0.1f, 0f, 0.0f,
            -0.1f, 0f, 0.0f
        };
        private static float[] OutlineDownV =
        {
            //X    Y      Z
            0f,  -0.1f, 0.0f,
            -0.1f, 0f, 0.0f,
            0.1f, 0f, 0.0f
        };
        private static float[] OutlineLeftV =
        {
            //X    Y      Z
            0f,  0.1f, 0.0f,
            -0.1f, 0f, 0.0f,
            0f, -0.1f, 0.0f
        };
        private static float[] OutlineRightV =
        {
            //X    Y      Z
            0f,  0.1f, 0.0f,
            0.1f, 0f, 0.0f,
            0f, -0.1f, 0.0f
        };

        private static uint[] Indices =
        {
            0, 1, 2
        };
        
        
        public static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default with
            {
                Size = screenSize,
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

            outlines[0] = new(ref gl, OutlineUpV.AsSpan(), Indices.AsSpan(), BufferUsageARB.StaticDraw, PrimitiveType.Triangles);
            outlines[1] = new(ref gl, OutlineDownV.AsSpan(), Indices.AsSpan(), BufferUsageARB.StaticDraw, PrimitiveType.LineLoop);
            outlines[2] = new(ref gl, OutlineLeftV.AsSpan(), Indices.AsSpan(), BufferUsageARB.StaticDraw, PrimitiveType.LineLoop);
            outlines[3] = new(ref gl, OutlineRightV.AsSpan(), Indices.AsSpan(), BufferUsageARB.StaticDraw, PrimitiveType.LineLoop);

            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            //gl.ClearColor(1f, 0f, 0f, 1f);
            gl.Clear(ClearBufferMask.ColorBufferBit);

           

            shaderObject = new Shader("Shaders/VertexShaderTest.glsl", 
               "Shaders/FragmentShaderTest.glsl",
               gl);
           
            
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);
        }

        public static void OnUpdate(double obj)
        {
            var halfSize = (Vector2D<float>)(screenSize / new Vector2D<int>(2, 2));
            mousePos = new Vector2D<float>(input.Mice[0].Position.X, input.Mice[0].Position.Y);
            mousePos -= halfSize;
            mousePos = mousePos / halfSize * new Vector2D<float>(1f, -1f);
        }
        
        public static unsafe void OnRender(double obj)
        {
           
            gl.Clear(ClearBufferMask.ColorBufferBit);
            


            VertexArray<uint> movedOutline = new(outlines[(uint)currentDirection], mousePos);

            movedOutline.Bind();
            shaderObject.Use();
            gl.DrawElements(PrimitiveType.Triangles, movedOutline.IndexCount, DrawElementsType.UnsignedInt, null);

            foreach (var va in savedTriangles)
            {
                va.Bind();
                shaderObject.Use();
                gl.DrawElements(PrimitiveType.Triangles, va.IndexCount, DrawElementsType.UnsignedInt, null);
            }

            //movedOutline.Dispose();
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            Console.WriteLine("Closing...");
            
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

                case Key.Escape:
                    savedTriangles.Clear();
                    break;

                case Key.Z:
                    if (keyboard.IsKeyPressed(Key.ControlLeft))
                        savedTriangles.RemoveRange(savedTriangles.Count - 1, 1);
                    break;
            }
        }

        private static void AddTriangle()
        {
            savedTriangles.Add(new VertexArray<uint>(outlines[(uint)currentDirection], mousePos));
        }
    }
}
