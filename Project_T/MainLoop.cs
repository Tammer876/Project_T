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
        private static uint vao;
        private static uint shader;
        private static uint[] objectArray = new uint[3];
        
        private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

        //Fragment shaders are run on each fragment/pixel of the geometry.
        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);
        }
        ";

        //Vertex data, uploaded to the VBO.
        private static readonly float[] Vertices =
        {
            //X    Y      Z
            0.5f,  0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f
        };

        //Index data, uploaded to the EBO.
        private static readonly uint[] Indices =
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
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
            
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            gl.Clear(ClearBufferMask.ColorBufferBit);

            fixed (void* verticesPtr = &Vertices[0], indexPtr = &Indices[0])
            {
               uint[] objectArray = Renderer.initVertexArray(gl, verticesPtr, indexPtr, (uint) Vertices.Length, (uint) Indices.Length);
               vao = objectArray[0];
            }
            
            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, VertexShaderSource);
            gl.CompileShader(vertexShader);
            
            string infoLog = gl.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling vertex shader {infoLog}");
                window.Close();
            }
            
            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, FragmentShaderSource);
            gl.CompileShader(fragmentShader);
            
            infoLog = gl.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling vertex shader {infoLog}");
                window.Close();
            }

            shader = gl.CreateProgram();
            gl.AttachShader(shader, vertexShader);
            gl.AttachShader(shader, fragmentShader);
            gl.LinkProgram(shader);
            
            gl.GetProgram(shader, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {gl.GetProgramInfoLog(shader)}");
            } 
            
            gl.DetachShader(shader, vertexShader);
            gl.DetachShader(shader, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
            
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
            gl.UseProgram(shader);
            
            gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
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
            gl.DeleteProgram(shader);
        }

        private static unsafe void KeyDown(IKeyboard keyboard, Key key, int arg)
        {
            switch (key)
            {
                case Key.W:
                    gl.DeleteBuffer(objectArray[1]);
                    gl.DeleteBuffer(objectArray[2]);
                    gl.DeleteVertexArray(vao);
                    
                    vao = gl.GenVertexArray();
                    gl.BindVertexArray(vao);
                    
                    objectArray[1] = gl.GenBuffer();
                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, objectArray[1]);
                    float[] expandedVertices =
                    {
                        //X    Y      Z
                        0.5f, 1.0f, 0.0f,
                        0.5f, -0.5f, 0.0f,
                        -0.5f, -0.5f, 0.0f,
                        -0.5f, 1.0f, 0.5f
                    };
                    fixed (void* verticesPtr = &expandedVertices[0])
                    {
                        gl.BufferData(BufferTargetARB.ArrayBuffer, (uint) (expandedVertices.Length * sizeof(uint)), verticesPtr, BufferUsageARB.StaticDraw);
                    }
                    
                    objectArray[2] = gl.GenBuffer();
                    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, objectArray[2]);
                    fixed (void* indicesPtr = &Indices[0])
                    {
                        gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint) (Indices.Length * sizeof(uint)), indicesPtr, BufferUsageARB.StaticDraw);
                    }
                    
                    gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
                    gl.UseProgram(shader);
                    gl.EnableVertexAttribArray(0);
                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                    break;
                case Key.S:
                    gl.DeleteBuffer(objectArray[1]);
                    gl.DeleteBuffer(objectArray[2]);
                    gl.DeleteVertexArray(vao);
                    
                    vao = gl.GenVertexArray();
                    gl.BindVertexArray(vao);
                    
                    objectArray[1] = gl.GenBuffer();
                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, objectArray[1]);
                    fixed (void* verticesPtr = &Vertices[0])
                    {
                        gl.BufferData(BufferTargetARB.ArrayBuffer, (uint) (Vertices.Length * sizeof(uint)), verticesPtr, BufferUsageARB.StaticDraw);
                    }
                    
                    objectArray[2] = gl.GenBuffer();
                    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, objectArray[2]);
                    fixed (void* indicesPtr = &Indices[0])
                    {
                        gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint) (Indices.Length * sizeof(uint)), indicesPtr, BufferUsageARB.StaticDraw);
                    }
                    
                    gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
                    gl.UseProgram(shader);
                    gl.EnableVertexAttribArray(0);
                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                    break;
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
