using System;
using Silk.NET.OpenGL;

namespace silkgl;

public class Shader
{
    private uint shaderID;
    private GL gl;

    public Shader(uint shaderId)
    {
        shaderID = shaderId;
    }

    private uint initShader(string vertexShaderSource, string fragmentShaderSource)
    {
        //debug feature for shader compilation
        Action<uint, string> CheckShaderLog = delegate(uint shader, string shaderSource)
#if DEBUG
        {
            gl.GetShaderInfoLog(shader, out string infoLog);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling vertex shader {shaderSource}: {infoLog}");
            }
        };
#else
        {};
#endif
        uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
        uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        
        gl.ShaderSource(vertexShader, fragmentShaderSource);
        gl.ShaderSource(fragmentShader, fragmentShaderSource);
        
        gl.CompileShader(vertexShader);
        CheckShaderLog(vertexShader, vertexShaderSource);
        gl.CompileShader(fragmentShader);
        CheckShaderLog(fragmentShader, fragmentShaderSource);
        
        uint shaderProgram = gl.CreateProgram(); 
        gl.AttachShader(shaderProgram, vertexShader);
        gl.AttachShader(shaderProgram, fragmentShader);
        gl.LinkProgram(shaderProgram);
#if DEBUG
        gl.GetProgramInfoLog(shaderProgram, out string infoLog);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Console.WriteLine($"Error linking shaders {vertexShaderSource} and {fragmentShaderSource}: {infoLog}");
        }
#endif
        
        gl.DetachShader(shaderProgram, vertexShader);
        gl.DetachShader(shaderProgram, fragmentShader);
        gl.DeleteShader(fragmentShader);
        gl.DeleteShader(vertexShader);
        
        return shaderProgram;
    }
    
    
}