using System;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;

namespace silkgl;

public class Shader
{
    private uint shaderID; 
    private GL gl;

    public Shader(string vShaderSource, string fShaderSource, GL gl_ref)
    {
        gl = gl_ref;
        shaderID = initShader(vShaderSource, fShaderSource);
    }

    private uint initShader(string vertexShaderSource, string fragmentShaderSource)
    {
        uint vertexShader = LoadShader(ShaderType.VertexShader,  vertexShaderSource);
        uint fragmentShader = LoadShader(ShaderType.FragmentShader, fragmentShaderSource);
        
        uint shaderProgram = gl.CreateProgram();
        gl.AttachShader(shaderProgram, vertexShader);
        gl.AttachShader(shaderProgram, fragmentShader);
        gl.LinkProgram(shaderProgram);
#if DEBUG
        gl.GetProgram(shaderProgram, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Error linking shaders {vertexShaderSource} and {fragmentShaderSource}: {gl.GetProgramInfoLog(shaderProgram)}");
        }
#endif
        
        gl.DetachShader(shaderProgram, vertexShader);
        gl.DetachShader(shaderProgram, fragmentShader);
        gl.DeleteShader(fragmentShader);
        gl.DeleteShader(vertexShader);
        
        return shaderProgram;
    }
    
    private uint LoadShader(ShaderType type, string path)
    {
        string src = File.ReadAllText(path);
        uint handle = gl.CreateShader(type);
        gl.ShaderSource(handle, src);
        gl.CompileShader(handle);
        
#if DEBUG
        string infoLog = gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }
#endif
        return handle;
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