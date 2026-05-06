using Silk.NET.GLFW;
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace silkgl
{
    public class MainLoop
    {
        public static IWindow window;
        
        public static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Project_T",
            };
            
            window = Window.Create(windowOptions);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            
            window.Run();
            
            window.Dispose();
            
        }

        public static void OnLoad()
        {
            GL gl = window.CreateOpenGL();
            gl = window.CreateOpenGL();
            IInputContext input = window.CreateInput();
            gl.Viewport(window.FramebufferSize);
        }

        public static void OnUpdate(double obj)
        {

        }

        public static void OnRender(double obj)
        {

        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {

        }
    }
}
