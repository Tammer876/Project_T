using System;
using Silk.NET.OpenGL;

namespace silkgl;

public class VertexArray<TVertex, TIndex> : IDisposable
    
{
    private uint vao;
    private Buffer<TVertex> vbo;
    private Buffer<TIndex> ebo;
    private PrimitiveType primitiveType;
    private GL gl;

    public VertexArray(ref GL gl_ref, ref Span<TVertex> vertexArr, ref Span<TIndex> indexArr, BufferUsageARB bufferUsage, PrimitiveType primitive)
    {
        gl = gl_ref;
        primitiveType = primitive;
        // vao = initVertexArray();
    }
    
    

    public VertexArray(ref GL gl_ref, Buffer<TVertex> vbo_ref, Buffer<TIndex> ebo_ref, PrimitiveType primitive)
    {
        gl = gl_ref;
        primitiveType = primitive;
        vao = gl.GenVertexArray();
        vbo = vbo_ref;
        ebo = ebo_ref;
        Bind();
        vbo.Bind();
        ebo.Bind();
    }
    
    private unsafe uint initVertexArray(ref Span<TVertex> vertexArr, ref Span<TIndex> indexArr, BufferUsageARB bufferUsage)
    {
        uint vArray = gl.GenVertexArray();
        Bind();
        vbo = new(ref gl, BufferTargetARB.ArrayBuffer, bufferUsage, ref vertexArr);
        ebo = new(ref gl, BufferTargetARB.ElementArrayBuffer, bufferUsage, ref indexArr);
        
        return vArray;
    }

    public void Bind()
    {
        gl.BindVertexArray(vao);
    }

    public unsafe void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, uint stride, void* pointer)
    {
        gl.VertexAttribPointer(index, size, type, normalized, stride, pointer);
        gl.EnableVertexAttribArray(index);
    }

    public uint IndexCount => ebo.Length;
    public void Dispose()
    {
        gl.DeleteVertexArray(vao);
    }
}