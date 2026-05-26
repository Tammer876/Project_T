using System;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace silkgl;

public class Renderer
{
    
    public static unsafe uint[] InitVertexArray(ref GL gl, ref float[] vertexArr, ref uint[] indexArr)
    {
        uint vao = gl.GenVertexArray();
        gl.BindVertexArray(vao);
        
        uint vbo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
        fixed (void* ptr = vertexArr)
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (uint) (vertexArr.Length * sizeof(float)), ptr,
                BufferUsageARB.StaticDraw);
        }
        
        uint ebo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
        fixed (void* ptr = indexArr)
        {
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(indexArr.Length * sizeof(uint)), ptr,
                BufferUsageARB.StaticDraw);
        }

        return new [] { vao, vbo, ebo };
    }
    
    
}