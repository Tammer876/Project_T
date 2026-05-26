using System;
using System.Numerics;
using Silk.NET.OpenGL;

namespace silkgl;

public class Shader
{
    private uint shaderID;
    private GL gl;

    public Shader(string vShaderSource, string fShaderSource, ref GL gl_ref)
    {
        shaderID = initShader(vShaderSource, fShaderSource);
        gl = gl_ref;
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

    public void Use()
    {
        gl.UseProgram(shaderID);
    }

    public void Destroy()
    {
        gl.DeleteProgram(shaderID);
    }

    public void SetFloat(ref string name, float value)
    {
        gl.Uniform1(gl.GetUniformLocation(shaderID, name), value);
    }

    public void SetInt(ref string name, int value)
    {
        gl.Uniform1(gl.GetUniformLocation(shaderID, name), value);
    }

    public void SetBool(ref string name, bool value)
    {
        gl.Uniform1(gl.GetUniformLocation(shaderID, name), value ? 1 : 0);
    }

    public void SetVector(ref string name, Vector4 value)
    {
        gl.Uniform4(gl.GetUniformLocation(shaderID, name), value);
    }
    
}