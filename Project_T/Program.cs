using System;
using Silk.NET.GLFW;
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace silkgl
{
    public class MainLoop
    {
        private static IWindow window;
        private static GL gl;

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

        public static void OnLoad()
        {
            gl = GL.GetApi(window);
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
            
            gl.ClearColor(1f, 0.0f, 0.0f, 0.0f);
            gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public static void OnUpdate(double obj)
        {
        
        }
        
        public static void OnRender(double obj)
        {
            gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            Console.WriteLine("Closing...");
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int arg)
        {
            switch (key)
            {
                case Key.G:
                    gl.ClearColor(0.0f, 1f, 0.0f, 1.0f);
                    break;
                case Key.R:
                    gl.ClearColor(1f, 0.0f, 0.0f, 1.0f);
                    break;
                case Key.C:
                    gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                    break;
                case Key.Escape:
                    window.Close();
                    break;
            }
        }
    }
}
