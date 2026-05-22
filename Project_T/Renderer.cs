using System;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace silkgl;

public class Renderer
{
    
    public static unsafe uint[] initVertexArray(GL gl, void* vertexArrPtr, void* indexArrPtr, uint vertexArrLength, uint indexArrLength)
    {
        uint vao = gl.GenVertexArray();
        gl.BindVertexArray(vao);
        
        uint vbo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
        gl.BufferData(BufferTargetARB.ArrayBuffer,(vertexArrLength * sizeof(uint)), vertexArrPtr,  BufferUsageARB.StaticDraw);
       
        
        uint ebo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
        gl.BufferData(BufferTargetARB.ElementArrayBuffer,(indexArrLength * sizeof(uint)), indexArrPtr,  BufferUsageARB.StaticDraw);
        
        
        return new  uint[] { vao, vbo, ebo };
    }
    
    
}